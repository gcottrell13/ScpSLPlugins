using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Structs;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomGameModes.GameModes
{
    internal abstract class DhasRole
    {
        public delegate IEnumerator<float> dhasTask();

        public Player player { get; private set; }
        public DhasRoleManager Manager { get; }
        public abstract List<dhasTask> Tasks { get; }
        public abstract RoleTypeId RoleType { get; }

        #region Running State

        public Player? AlreadyAcceptedCooperativeTasks;

        private CoroutineHandle _runningCoroutine;

        public Pickup? MyTargetPickup { get; private set; }

        private List<dhasTask> runningTaskList;

        public string CurrentTaskHint { get; private set; } = "";

        public int CurrentTaskNum { get; private set; }
        public dhasTask? CurrentTask { get
            {
                try
                {
                    return runningTaskList[CurrentTaskNum];
                }
                catch (ArgumentOutOfRangeException)
                {
                    DoneAllTasks = true;
                    return null;
                }
            } }

        public bool DoneAllTasks { get; protected set; }

        protected bool IsRunning => _runningCoroutine.IsRunning;

        #endregion

        public DhasRole(Player player, DhasRoleManager manager)
        {
            this.player = player;
            Manager = manager;
            runningTaskList = Tasks;
        }

        public virtual string CountdownBroadcast => "Releasing the Beast from GR-18 in: ❤";
        public virtual string SickoModeBroadcast => "The Beast can Smell You Now! Run!";
        public virtual string MainGameBroadcast => "Do your tasks and Survive!";
        public virtual string RoundEndBroadcast => "You Win!\nKill The Beast!";

        public virtual void GetPlayerReadyAndEquipped()
        {
            var item = player.AddItem(ItemType.Flashlight);
            player.CurrentItem = item;
            EnsureFirearm(FirearmType.Com15, AttachmentIdentifier.Get(FirearmType.Com15, InventorySystem.Items.Firearms.Attachments.AttachmentName.Flashlight));
            player.AddAmmo(FirearmType.Com15, 10);
        }


        /// <summary>
        /// Idempotent Stop
        /// </summary>
        public void Stop()
        {
            if (_runningCoroutine.IsRunning)
            {
                Timing.KillCoroutines(_runningCoroutine);
            }
            if (MyTargetPickup != null && Manager.ClaimedPickups.ContainsKey(MyTargetPickup))
            {
                Manager.ClaimedPickups.Remove(MyTargetPickup);
            }
            OnStop();
        }

        /// <summary>
        /// idempotent stop()
        /// </summary>
        public abstract void OnStop();

        public void Start()
        {
            // idempotent stop()
            Stop();

            // then start
            _runningCoroutine = Timing.RunCoroutine(_coroutine());
        }

        private IEnumerator<float> _coroutine()
        {
            while (CurrentTask != null && !DoneAllTasks)
            {
                // start next task
                var nextTask = CurrentTask();
                var d = true;

                while (d)
                {
                    try
                    {
                        d = nextTask.MoveNext();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        break;
                    }
                    if (player.Role.Type == RoleTypeId.Spectator && RoleType != RoleTypeId.Spectator)
                    {
                        // dead =(
                        Manager.OnPlayerDied(player);
                        goto Dead;
                    }

                    if (d)
                        yield return nextTask.Current;
                }

                Log.Debug($"{player.DisplayNickname} completed task {CurrentTask.Method.Name}");

                MyTargetPickup = null;
                Manager.OnPlayerCompleteOneTask(player);

                RemoveTime();

                ShowTaskCompleteMessage(3);
                yield return Timing.WaitForSeconds(3);

                // increment task
                CurrentTaskNum++;
            }

            Dead:

            if (CurrentTaskNum >= runningTaskList.Count || DoneAllTasks)
                OnCompleteAllTasks();

            Log.Debug($"Player {player.DisplayNickname} - Role coroutine completed");
        }

        public virtual void OnCompleteAllTasks() => Manager.OnPlayerCompleteAllTasks(player);

        public virtual void ShowTaskCompleteMessage(float duration)
        {
            var d = taskDifficulty.DifficultyTime();
            var message = $"{TaskSuccessMessage} (-{d}s)";
            CurrentTaskHint = message;
            player.ShowHint(message, duration);
        }

        private TaskDifficulty taskDifficulty => CurrentTask.GetMethodInfo().GetCustomAttribute<CrewmateTaskAttribute>().Difficulty;

        private void RemoveTime()
        {
            Log.Debug($"Removed time: {taskDifficulty.DifficultyTime()}s");
            Manager.OnRemoveTime(taskDifficulty.DifficultyTime());
        }

        public void FormatTask(string message, string compass)
        {
            // empty lines push it down from the middle of the screen
            var taskMessage = $"""







                <b>Task {CurrentTaskNum+1}</b>:
                {message}
                {compass}
                """;
            CurrentTaskHint = taskMessage;
            player.ShowHint(taskMessage);
        }

        #region Pickups

        private Pickup? _claimNearestPickup(Func<Pickup, bool> predicate)
        {
            var inZone = Pickup.List.Where(
                p => p.Room?.Zone == player.CurrentRoom?.Zone
            );
            var validPickups = new List<Pickup>();

            foreach (var pickup in inZone)
            {
                if (Manager.ClaimedPickups.TryGetValue(pickup, out var owner) && owner != player) continue;
                if (!predicate(pickup)) continue;
                validPickups.Add(pickup);
            }

            var closePickup = validPickups.OrderBy(p => (p.Position - player.Position).magnitude).FirstOrDefault();
            if (closePickup != null)
            {
                if (closePickup != MyTargetPickup
                    && MyTargetPickup != null 
                    && Manager.ClaimedPickups.ContainsKey(MyTargetPickup)
                    && Manager.ClaimedPickups[MyTargetPickup] == player
                    )
                {
                     Manager.ClaimedPickups.Remove(MyTargetPickup);
                }

                Manager.ClaimedPickups[closePickup] = player;
                MyTargetPickup = closePickup;
                return closePickup;
            }
            return null;
        }

        /// <summary>
        /// returns false when done
        /// </summary>
        /// <param name="message"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool GoGetPickup(Func<Pickup, bool> predicate, Action onFail)
        {
            if (MyTargetPickup != null && !NotHasItem(MyTargetPickup.Type)) 
                return false;

            MyTargetPickup = _claimNearestPickup(predicate);
            if (MyTargetPickup == null)
            {
                onFail();
                return false;
            }

            return NotHasItem(MyTargetPickup.Type);
        }

        #endregion

        #region Cooperative Tasks

        /// <summary>
        /// Will add the given tasks to this role's task list after the current task + offset.
        /// If the tasks should be run immediately after the current task, offset should be zero.
        /// </summary>
        /// <param name="offsetFromCurrent"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public bool TryGiveCooperativeTasks(Player friend, uint offsetFromCurrent, params dhasTask[] tasks)
        {
            if (DoneAllTasks) return false;
            if (AlreadyAcceptedCooperativeTasks != null && AlreadyAcceptedCooperativeTasks != friend) return false;
            AlreadyAcceptedCooperativeTasks = friend;

            var insertAt = Math.Min((int)offsetFromCurrent + CurrentTaskNum + 1, runningTaskList.Count - 1);
            runningTaskList.InsertRange(insertAt, tasks);

            Log.Debug($"{player.DisplayNickname} accepted a task from {friend.DisplayNickname}. Current Task: {CurrentTaskNum} - new task scheduled for {insertAt}");

            return true;
        }

        public List<Player> OtherCrewmates => Manager.Humans()
            .Where(r => r.player != player)
            .Select(r => r.player)
            .ToList();

        public Player? Beast => Manager.Beast().FirstOrDefault()?.player;

        protected Player? GetFarthestCrewmate()
        {
            var farthestPlayer = OtherCrewmates
                    .OrderByDescending(p => (p.Position - player.Position).magnitude)
                    .FirstOrDefault();
            if (farthestPlayer == null || farthestPlayer == player) 
                return null;
            return farthestPlayer;
        }

        public Player? GetNearestCrewmate(Func<Player, bool>? filter = null)
        {
            var nearestTeammate = OtherCrewmates
                    .Where(p => filter?.Invoke(p) ?? true)
                    .OrderBy(p => (p.Position - player.Position).magnitude)
                    .FirstOrDefault();
            if (nearestTeammate == null || nearestTeammate == player) return null;
            return nearestTeammate;
        }

        public float DistanceTo(Player? p) => p == null ? float.PositiveInfinity : (p.Position - player.Position).magnitude;
        public float DistanceTo(Vector3 vec) => (player.Position - vec).magnitude;

        public bool IsNear(Player? p, int distance, out string display)
        {
            if (p != null) {
                Vector3 pos = p.Position;
                if (p.IsDead)
                {
                    pos = Ragdoll.Get(p).First().Position;
                }

                if (DistanceTo(pos) < distance)
                {
                    display = strong($"<color=green>{distance}m</color>");
                    return true;
                }
            } 

            display = $"{distance}m";
            return false;
        }

        public bool IsNear(Player? p, int distance) => IsNear(p, distance, out var _);

        #endregion

        #region Compass

        public string CompassToRoom(RoomType roomType)
        {
            if (Room.Get(roomType) is Room room)
            {
                return CompassToRoom(room);
            }
            return "";
        }

        public string CompassToRoom(Room room)
        {
            if (player.CurrentRoom != room && room.Doors.FirstOrDefault(door => door.Rooms.Count > 1) is Door entranceDoor)
            {
                return GetCompass(entranceDoor.Position);
            }
            return "";
        }

        public string CompassToPlayer(Player? p)
        {
            if (p == null)
                return "";
            if (p.IsDead)
            {
                return GetCompass(Ragdoll.Get(p).First().Position);
            }
            if (p.Zone != player.Zone)
            {
                string compass = p.CurrentRoom?.Type switch
                { 
                    RoomType.HczElevatorA => CompassToRoom(Room.Get(RoomType.LczCheckpointA)),
                    RoomType.HczElevatorB => CompassToRoom(Room.Get(RoomType.LczCheckpointB)),
                    _ => "",
                };

                return $"""
                    This player is in {p.Zone}, in {p.CurrentRoom?.Type ?? RoomType.Unknown} room:
                    {compass}
                    """;
            }
            else
            {
                return GetCompass(p.Position);
            }
        }

        public string GetCompass(Vector3 to)
        {
            var delta = player.Transform.InverseTransformPoint(to);
            var deltaDir = Math.Atan2(delta.x, delta.z) * 180 / Math.PI;

            var dashesLeft = "--------------------";
            var dashesRight = dashesLeft;
            var straight = "|";
            var target = "▮";
            var radLeft = "   ";
            var radRight = "   ";

            var combined = $"{dashesLeft}{straight}{dashesRight}";

            var compassDegWidth = 60;

            while (deltaDir > 180)
            {
                deltaDir -= 360;
            }
            while (deltaDir < -180)
            {
                deltaDir += 360;
            }

            if (Math.Abs(deltaDir) > compassDegWidth)
            {
                if (deltaDir < 0) radLeft = ((int)-deltaDir).ToString();
                if (deltaDir > 0) radRight = ((int)deltaDir).ToString();
            }
            else
            {
                var i = (int)((deltaDir + compassDegWidth) / (compassDegWidth * 2) * combined.Length);
                if (i >= combined.Length || i < 0) { }
                else
                {
                    combined = combined.Substring(0, i) + target + combined.Substring(i + 1);
                }
            }
            var compass = $"<{radLeft:03} {combined} {radRight:03}>";
            return compass;
        }

        protected string HotAndCold(Vector3? to)
        {
            if (to == null) return "";
            var distance = (int)DistanceTo(to.Value);
            var size = distance switch
            {
                < 1 => 500,
                < 5 => 400,
                < 10 => 100,
                < 15 => 70,
                < 20 => 50,
                < 30 => 20,
                < 40 => 30,
                < 50 => 40,
                < 60 => 100,
                < 70 => 150,
                < 80 => 200,
                < 100 => 250,
                _ => 500,
            };

            var text = distance switch
            {
                < 5 => "HOT",
                < 20 => "Hotter!",
                < 30 => "Lukewarm",
                < 60 => "Colder",
                < 100 => "Freezing",
                _ => "Far",
            };

            var color = distance switch
            {
                < 10 => "red",
                < 15 => "orange",
                < 30 => "yellow",
                < 60 => "blue",
                _ => "blue",
            };

            return strong($"<color={color}><size={size}>{text}</size></color>");
        }

        public string HotAndColdToBeast() => Beast != null ? HotAndCold(Beast.Position) : "No beast...";


        #endregion

        #region 914

        protected void CanUse914() => Manager.CanUse914(player);
        protected void CannotUse914() => Manager.CannotUse914(player);

        #endregion

        #region Inventory

        protected void CanDropItem(ItemType type)
        {
            Manager.CanDropItem(type, player);
        }

        protected void CannotDropItem(ItemType item) => Manager.CannotDropItem(item, player);


        public bool NotHasItem(ItemType type) => NotHasItem(type, out var _);

        protected bool NotHasItem(ItemType type, out Item item)
        {
            item = player.Items.FirstOrDefault(item => item.Type == type);
            if (item == null) return true;
            return false;
        }

        protected Item EnsureItem(ItemType type)
        {
            var item = player.Items.FirstOrDefault(item => item.Type == type);
            if (item == null)
            {
                return player.AddItem(type);
            }
            return item;
        }

        protected Item EnsureFirearm(FirearmType type, params AttachmentIdentifier[] attachments)
        {
            var item = player.Items.FirstOrDefault(item => item is Firearm firearm && firearm.FirearmType == type);
            if (item == null)
            {
                return player.AddItem(type, attachments);
            }
            return item;
        }

        #endregion

        #region Tasks

        protected ItemType MyKeycardType;
        [CrewmateTask(TaskDifficulty.Medium)]
        protected IEnumerator<float> UpgradeKeycard()
        {
            CanUse914();

            while (NotHasItem(ItemType.KeycardO5, out var item))
            {
                FormatTask("Max Upgrade Your Keycard to <b>O5</b>", CompassToRoom(RoomType.Lcz914));
                yield return Timing.WaitForSeconds(1);
            }

            CannotUse914();
        }


        protected int requiredFriendDistance = 5;
        bool friendCompletedTask = false;
        protected void OnSomeoneCompleteTask(Player p)
        {
            if ((player.Position - p.Position).magnitude < requiredFriendDistance)
                friendCompletedTask = true;
        }

        [CrewmateTask(TaskDifficulty.Medium)]
        protected IEnumerator<float> BeNearWhenTaskComplete()
        {
            Manager.PlayerCompleteOneTask += OnSomeoneCompleteTask;
            friendCompletedTask = false;
            while (!friendCompletedTask)
            {
                var nearest = GetNearestCrewmate(p => !Manager.PlayerRoles[p].DoneAllTasks && Manager.PlayerRoles[p].CurrentTask != BeNearWhenTaskComplete);
                var compass = nearest == null ? "<i>There's nobody nearby</i>" : CompassToPlayer(nearest);

                IsNear(nearest, requiredFriendDistance, out var dist);

                FormatTask($"""
                    Be Near Someone ({dist}) When
                    They Complete a Task
                    """, compass);
                yield return Timing.WaitForSeconds(1);
            }

            Manager.PlayerCompleteOneTask -= OnSomeoneCompleteTask;
        }


        public HashSet<Player> PlayersFound = new();
        [CrewmateTask(TaskDifficulty.Medium)]
        public IEnumerator<float> FindAPlayer()
        {
            var requiredDistance = 5;
            var targetPlayer = OtherCrewmates.Where(x => !IsNear(x, requiredFriendDistance) && !PlayersFound.Contains(x)).GetRandomValue();

            if (targetPlayer == null)
                targetPlayer = Player.List.Where(x => x != player && x.Role.Team != Team.SCPs).GetRandomValue();

            if (targetPlayer != null)
            {
                PlayersFound.Add(targetPlayer);
                while (!IsNear(targetPlayer, requiredDistance, out var dist))
                {
                    var compass = CompassToPlayer(targetPlayer);
                    FormatTask($"Be Within {dist} of {PlayerNameFmt(targetPlayer)}", compass);
                    yield return Timing.WaitForSeconds(0.5f);
                }
            }
            else
            {
                throw new Exception("nobody found");
            }
        }

        #endregion

        public string TaskSuccessMessage => strong("<size=40><color=green>Task Complete!</color></size>");

        public string PlayerNameFmt(Player? player)
        {
            if (player == null) return "";

            RoleTypeId role = player.IsDead ? Ragdoll.Get(player).FirstOrDefault()?.Role ?? RoleTypeId.None : player.Role;
            var color = RoleExtensions.GetTeam(role) switch
            {
                Team.Scientists => "yellow",
                Team.ClassD => "orange",
                Team.FoundationForces => "blue",
                Team.SCPs => "red",
                _ => "white",
            };
            return strong($"<color={color}>{player.DisplayNickname}</color>");
        }

        public string strong(object s) => $"<b>{s}</b>";

        public float WaitHint(string hint, float seconds)
        {
            player.ShowHint(hint, seconds);
            return Timing.WaitForSeconds(seconds);
        }
    }
}
