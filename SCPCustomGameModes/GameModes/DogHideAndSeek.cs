using CustomGameModes.API;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerEvent = Exiled.Events.Handlers.Player;
using Scp914Handler = Exiled.Events.Handlers.Scp914;
using ServerEvent = Exiled.Events.Handlers.Server;
using LightContainmentZoneDecontamination;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.EventArgs.Scp914;
using Exiled.API.Features.Pickups;
using Scp914;
using PlayerRoles;
using Exiled.API.Features.Items;
using CustomGameModes.Configs;
using VoiceChatModifyHook;
using VoiceChatModifyHook.Events;
using VoiceChat;

namespace CustomGameModes.GameModes
{
    internal class DogHideAndSeek : IGameMode
    {
        public string Name => "DogHideAndSeek";

        public string PreRoundInstructions => "";

        static int gamesPlayed = 0;

        Door? beastDoor;
        bool DidTimeRunOut = false;
        List<Door> LCZDoors = new();
        public static DhasRoleManager? Manager;
        bool cassieBeastEscaped = false;

        HashSet<Door> DoorsReopenAfterClosing = new();

        CoroutineHandle roundHandlerCO;

        int CountdownTime = 35;
        int BaseRoundTime = 5 * 60;

        bool FinalCountdown = false;
        bool FiveMinuteWarning = false;
        bool ReleaseOneMinuteWarning = false;
        bool ReleaseCountdown = false;

        DHASConfig config => (CustomGameModes.Singleton?.Config ?? new()).DogHideAndSeek;

        public void OnRoundStart()
        {
            // -------------------------------------------------------------
            // Event Handlers
            // -------------------------------------------------------------
            PlayerEvent.InteractingDoor += OnInteractDoor;
            PlayerEvent.Hurting += OnHurting;
            PlayerEvent.SearchingPickup += OnSearchingPickup;
            PlayerEvent.DroppingAmmo += DeniableEvent;
            PlayerEvent.InteractingElevator += DeniableEvent;
            PlayerEvent.PlayerDamageWindow += PlayerDamagingWindow;
            PlayerEvent.Spawned += Spawned;

            ServerEvent.EndingRound += OnEndingRound;
            ServerEvent.RespawningTeam += DeniableEvent;

            Scp914Handler.UpgradingPickup += UpgradePickup;
            Scp914Handler.UpgradingInventoryItem += UpgradeInventory;

            ModifyVoiceChat.OnVoiceChatListen += OnVoiceChatListen;
            // -------------------------------------------------------------
            // -------------------------------------------------------------

            DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;

            setupGame();
            roundHandlerCO = Timing.RunCoroutine(_wrapRoundHandle());

            DidTimeRunOut = false;
            FinalCountdown = false;
            FiveMinuteWarning = false;
            ReleaseOneMinuteWarning = false;
            ReleaseCountdown = false;
            cassieBeastEscaped = false;

            SCPRandomCoin.API.CoinEffectRegistry.DisableAll();
            SCPRandomCoin.API.CoinEffectRegistry.EnableEffects(config.EnableCoinEffects.ToArray());
        }

        public void OnRoundEnd()
        {
            // -------------------------------------------------------------
            // Event Handlers
            // -------------------------------------------------------------
            PlayerEvent.InteractingDoor -= OnInteractDoor;
            PlayerEvent.Hurting -= OnHurting;
            PlayerEvent.SearchingPickup -= OnSearchingPickup;
            PlayerEvent.DroppingAmmo -= DeniableEvent;
            PlayerEvent.InteractingElevator -= DeniableEvent;
            PlayerEvent.PlayerDamageWindow -= PlayerDamagingWindow;
            PlayerEvent.Spawned -= Spawned;

            ServerEvent.EndingRound -= OnEndingRound;
            ServerEvent.RespawningTeam -= DeniableEvent;

            Scp914Handler.UpgradingPickup -= UpgradePickup;
            Scp914Handler.UpgradingInventoryItem -= UpgradeInventory;

            ModifyVoiceChat.OnVoiceChatListen -= OnVoiceChatListen;
            // -------------------------------------------------------------
            // -------------------------------------------------------------

            if (roundHandlerCO.IsRunning)
            {
                // If the SCP kills everyone, the coroutine will still be running. Kill it.
                Timing.KillCoroutines(roundHandlerCO);
            }
            // else, the Class-D survived long enough.

            if (Manager != null)
            {
                Manager.PlayerDied -= onPlayerRoleDied;
                Manager.PlayerCompleteAllTasks -= onPlayerCompleteAllTasks;
                Manager.StopAll();
            }

            SCPRandomCoin.API.CoinEffectRegistry.EnableAll();
            CountdownHelper.Stop();
        }

        public void OnWaitingForPlayers()
        {
            DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.None;
        }

        #region Event Handlers

        private void OnVoiceChatListen(VoiceChatListenEvent ev)
        {
            var speaker = ev.Speaker;
            var listener = ev.Listener;
            var channel = ev.VoiceChatChannel;
            if (speaker == listener)
                return;
            if (channel == VoiceChatChannel.Mimicry)
                return;

            if (listener.IsScp && !speaker.IsAlive)
            {
                ev.VoiceChatChannel = VoiceChatChannel.RoundSummary;
            }
            else if (speaker.IsScp && !listener.IsAlive)
            {
                ev.VoiceChatChannel = VoiceChatChannel.Spectator;
            }
        }

        private void Spawned(SpawnedEventArgs ev)
        {
            if (Manager == null)
                return;
            if (Manager.PlayerRoles.TryGetValue(ev.Player, out var role) && role is not SpectatorRole)
                return;
            if (Round.ElapsedTime > TimeSpan.FromSeconds(1))
                Manager.ApplyNextRole(ev.Player);
        }

        private void OnEndingRound(EndingRoundEventArgs ev)
        {
            if (Manager == null)
                return;
            if (Round.IsLocked) 
                return;

            if (DidTimeRunOut || Manager?.Beast().Count == 0)
            {
                ev.LeadingTeam = LeadingTeam.FacilityForces;
                ev.IsAllowed = true;
            }
            if (Manager?.Humans().Count == 0)
            {
                ev.LeadingTeam = LeadingTeam.Anomalies;
                ev.IsAllowed = true;
            }
        }

        private void PlayerDamagingWindow(DamagingWindowEventArgs ev)
        {
            if (Manager == null) 
                return;

            if (ev.Window.Room.Type == RoomType.LczGlassBox)
            {
                if (!Manager.BeastReleased)
                {
                    DeniableEvent(ev);
                }
                else if (!cassieBeastEscaped)
                {
                    cassieBeastEscaped = true;
                    var (cassie, caption) = beastName();
                    Cassie.MessageTranslated($"{cassie} has escaped containment", $"{caption} Has Escaped Containment");
                }
            }
        }

        private void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (Manager == null)
            {
                return;
            }
            
            if (Manager.ClaimedPickups.TryGetValue(ev.Pickup, out var assignedPlayer))
            {
                if (ev.Player != assignedPlayer)
                    DeniableEvent(ev);
            }
        }

        private void OnInteractDoor(InteractingDoorEventArgs ev)
        {
            if (Manager == null)
            {
                return;
            }
            else if (ev.Door == beastDoor || ev.Door.Zone != ZoneType.LightContainment || ev.Door.IsElevator)
            {
                DeniableEvent(ev);
            }
            else if (ev.Door.Room.Type == RoomType.LczArmory && ev.Door.IsOpen)
            {
                DeniableEvent(ev);
            }
            else if (ev.Door.Type == DoorType.Airlock)
            {
                // do nothing
            }
            else
            {
                if (Manager.BeastSickoModeActivate)
                {
                    // everyone can open all the LCZ doors once time is almost out
                    ev.Door.IsOpen = true;
                    DeniableEvent(ev);
                }
                else
                {
                    if (DoorsReopenAfterClosing.Contains(ev.Door))
                    {
                        Timing.CallDelayed(5f, () =>
                        {
                            ev.Door.IsOpen = true;
                        });
                    }
                }

            }
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (Manager == null)
                return;
            // allow the Class-D to hurt/kill the beast after they win
            // disallow the beast to hurt the Class-D after it loses

            if (ev.Player == ev.Attacker)
                return;

            if (ev.Player.IsHuman == ev.Attacker?.IsHuman)
                DeniableEvent(ev);

            if (DidTimeRunOut && ev.Attacker?.Role.Team == Team.SCPs)
                DeniableEvent(ev);
        }

        private void UpgradeInventory(UpgradingInventoryItemEventArgs ev)
        {
            if (Manager == null)
                return;

            if (UpgradeHelper.Upgrade(ev, out Item newItem))
            {
                DeniableEvent(ev);
                if (ev.Item != newItem)
                {
                    ev.Player.AddItem(newItem);
                    ev.Player.RemoveItem(ev.Item);
                }
            }
            else
            {
                DeniableEvent(ev);
            }
        }

        private void UpgradePickup(UpgradingPickupEventArgs ev)
        {
            if (Manager == null)
                return;

            if (UpgradeHelper.Upgrade(ev, out Pickup newPickup))
            {
                DeniableEvent(ev);
                if (newPickup != ev.Pickup)
                {
                    newPickup.Spawn(ev.OutputPosition, ev.Pickup.Rotation, previousOwner: ev.Pickup.PreviousOwner);
                    ev.Pickup.Destroy();
                }
                else
                {
                    ev.Pickup.Position = ev.OutputPosition;
                }
            }
            else
            {
                DeniableEvent(ev);
            }
        }


        public void DeniableEvent(IDeniableEvent ev)
        {
            if (Manager == null)
                return;
            ev.IsAllowed = false;
        }

        #endregion

        #region GameHandle

        private IEnumerator<float> _wrapRoundHandle()
        {
            var r = _roundHandle();
            var d = true;
            while (d)
            {
                try
                {
                    d = r.MoveNext();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    break;
                }
                if (d)
                    yield return r.Current;
            }
        }

        private void setupGame()
        {
            Log.Debug("DHAS - Starting a new game");
            gamesPlayed++;
            Manager = new DhasRoleManager();

            // ----------------------------------------------------------------------------------------------------------------
            // ----------------------------------------------------------------------------------------------------------------
            #region Set up Doors
            Log.Debug("DHAS - set up doors");
            foreach (var outsideDoor in Door.Get(door => door.Zone != ZoneType.LightContainment))
            {
                outsideDoor.IsOpen = false;
            }

            LCZDoors = Door.Get(door => door.Zone == ZoneType.LightContainment).ToList();

            beastDoor = Room.Get(RoomType.LczGlassBox).Doors.First(d => d.Rooms.Count == 1 && !d.IsGate);

            var doorsDoNotOpen = new HashSet<Door>()
            {
                Room.Get(RoomType.Lcz173).Doors.First(d => d.IsGate),
                Room.Get(RoomType.LczArmory).Doors.First(d => d.Rooms.Count == 1),
                Door.Get(DoorType.CheckpointLczA),
                Door.Get(DoorType.CheckpointLczB),
                beastDoor,
            };

            foreach (var door in LCZDoors)
            {
                if (door.Rooms.Count == 1 && door.Room.RoomName == MapGeneration.RoomName.LczCheckpointA) doorsDoNotOpen.Add(door);
                if (door.Rooms.Count == 1 && door.Room.RoomName == MapGeneration.RoomName.LczCheckpointB) doorsDoNotOpen.Add(door);
                if (door.Rooms.Count == 2 && door.DoorLockType == DoorLockType.None) DoorsReopenAfterClosing.Add(door);
                if (door.Room.Type == RoomType.Lcz330 && door.Type == DoorType.Scp330Chamber) doorsDoNotOpen.Add(door);
            }

            foreach (var lczdoor in LCZDoors)
            {
                lczdoor.IsOpen = !doorsDoNotOpen.Contains(lczdoor);
            }

            foreach (var shouldBeClosed in doorsDoNotOpen)
            {
                shouldBeClosed.IsOpen = false;
            }

            #endregion
            // ----------------------------------------------------------------------------------------------------------------
            // ----------------------------------------------------------------------------------------------------------------
            #region Set up Players
            Log.Debug("DHAS - set up players");

            List<Player> players = Player.List.ToList();
            players.ShuffleListSecure();

            // make sure any computers aren't left alone causing the overcharge animation to trigger
            foreach (var scp079 in players.Where(x => x.Role == RoleTypeId.Scp079)) 
                scp079.Role.Set(RoleTypeId.Scientist);

            Round.IsLocked = true;

            var iterator = 0;
            while (iterator < players.Count)
            {
                var index = (iterator + gamesPlayed) % players.Count;
                var player = players[index];
                Manager.ApplyNextRole(player);
                iterator++;
            }

            Round.IsLocked = false;

            #endregion
            // ----------------------------------------------------------------------------------------------------------------
            // ----------------------------------------------------------------------------------------------------------------
            #region Lights
            Log.Debug("DHAS - set up lights");

            // turn off all the lights in each room except for those that have players in them
            {
                var playerRooms = Manager.Humans().Select(role => role.player.CurrentRoom);
                foreach (var room in Room.Get(ZoneType.LightContainment))
                {
                    if (playerRooms.Contains(room)) continue;
                    room.TurnOffLights(9999f);
                }
            }

            Exiled.API.Features.Toys.Light.Create(Scp914Controller.Singleton.IntakeChamber.position);
            Exiled.API.Features.Toys.Light.Create(Scp914Controller.Singleton.OutputChamber.position);

            #endregion
            // ----------------------------------------------------------------------------------------------------------------
            // ----------------------------------------------------------------------------------------------------------------
        }

        private IEnumerator<float> _roundHandle()
        {
            Log.Debug("DHAS - round coroutine started");

            var timerTotalSeconds = CountdownTime;
            var timerStartedTime = DateTime.Now;

            var getBroadcast = (DhasRole p) => p.CountdownBroadcast;

            int elapsedTime() => (int)(DateTime.Now - timerStartedTime).TotalSeconds;
            void setTimer(int seconds)
            {
                foreach (var playerRole in Manager.ActiveRoles)
                    CountdownHelper.AddCountdown(playerRole.player, getBroadcast(playerRole), TimeSpan.FromSeconds(seconds));
            }
            void removeTime(int seconds)
            {
                timerTotalSeconds -= seconds;
                var remainingSeconds = Math.Max(0, timerTotalSeconds - elapsedTime());
                setTimer(remainingSeconds);
            }

            Manager.StartAll();
            removeTime(0);

            Log.Debug("DHAS - starting escape timer");

            while (elapsedTime() < timerTotalSeconds)
            {
                teleportPlayersOutOfHcz();
                var t = timerTotalSeconds - elapsedTime();
                if (!ReleaseOneMinuteWarning && t <= 30)
                {
                    ReleaseOneMinuteWarning = true;
                    var (cassie, _) = beastName();
                    CassieCountdownHelper.SayTimeReminder(t, $"until {cassie} escapes");
                }
                if (!ReleaseCountdown && t <= 10)
                {
                    ReleaseCountdown = true;
                    CassieCountdownHelper.SayCountdown(t, t);
                    setTimer(t);
                }
                yield return Timing.WaitForSeconds(1);
            }

            // ----------------------------------------------------------------------------------------------------------------
            // ----------------------------------------------------------------------------------------------------------------

            Cassie.Clear();

            Log.Debug("DHAS - starting main timer");
            Manager.BeastReleased = true;
            timerTotalSeconds = BaseRoundTime + Manager.TotalTimeRemovedByTasks();
            timerStartedTime = DateTime.Now;
            getBroadcast = (p) => p.MainGameBroadcast;

            Manager.RemovingTime += removeTime;
            Manager.PlayerDied += onPlayerRoleDied;
            Manager.PlayerCompleteAllTasks += onPlayerCompleteAllTasks;

            removeTime(0);

            void ActivateBeastSickoMode()
            {
                if (Manager == null || Manager.BeastSickoModeActivate == true)
                    return;

                Manager.BeastSickoModeActivate = true;
                foreach (var room in Room.Get(ZoneType.LightContainment))
                {
                    room.TurnOffLights(0);
                    room.Color = Color.red;
                }
            }

            while (elapsedTime() < timerTotalSeconds)
            {
                teleportPlayersOutOfHcz();
                var t = timerTotalSeconds - elapsedTime();

                if (Manager.Humans().Count == 1 && Manager.Spectators().Count > 0)
                {
                    ActivateBeastSickoMode();
                }

                if (!FiveMinuteWarning && t <= 300 && t % 30 == 0)
                {
                    FiveMinuteWarning = true;
                    CassieCountdownHelper.SayTimeReminder(t, "left in the game");
                }

                if (Manager.BeastSickoModeActivate == false && t <= 70)
                {
                    CassieCountdownHelper.SayTimeReminder(t, "left in the game");
                    ActivateBeastSickoMode();
                }

                if (!FinalCountdown && t <= 10)
                {
                    FinalCountdown = true;
                    CassieCountdownHelper.SayCountdown(t, t);
                    setTimer(t);
                }

                yield return Timing.WaitForSeconds(1);
            }

            Manager.RemovingTime -= removeTime;
            Manager.PlayerDied -= onPlayerRoleDied;
            Manager.PlayerCompleteAllTasks -= onPlayerCompleteAllTasks;

            // ----------------------------------------------------------------------------------------------------------------
            // Time Ran out, humans win!
            // ----------------------------------------------------------------------------------------------------------------

            foreach (var room in Room.Get(ZoneType.LightContainment))
            {
                room.TurnOffLights(0);
                room.Color = Color.white;
            }

            Cassie.Clear();
            Cassie.GlitchyMessage("Game Over", 0.5f, 0f);

            var stillAlive = Manager.Humans().Count;
            if (Player.Get(player => player.Role.Side == Side.Mtf).Count() > 0)
                Round.EscapedScientists = stillAlive;
            else
                Round.EscapedDClasses = stillAlive * 2; // *2 so that it doesn't end in a draw

            DidTimeRunOut = true;

            foreach (var classD in Manager.Humans())
            {
                classD.player.Health = 9999;
            }

            Manager.StopAll();
        }

        #endregion

        void onPlayerRoleDied(Player ev)
        {
            if (Manager == null) return;
            if (Manager.PlayerRoles[ev] is BeastRole) { return; }

            var specRole = Manager.ApplyRoleToPlayer(ev, SpectatorRole.name);
            specRole.Start();

            Timing.CallDelayed(UnityEngine.Random.Range(0, 2), () =>
            {
                if (Manager.Humans().Count == 0) return;

                Cassie.Clear();
                var alive = Manager.Humans().Count;
                Cassie.DelayedMessage($"{alive} personnel are alive", 1f, isNoisy: false);
            });
        }

        private void onPlayerCompleteAllTasks(Player ev)
        {
            Cassie.MessageTranslated("Personnel Has Completed All Tasks", $"{ev.DisplayNickname} has completed all tasks.", isNoisy: false);
        }

        private void teleportPlayersOutOfHcz()
        {
            foreach (Player player in Player.List)
            {
                if (player.Zone != ZoneType.LightContainment)
                {
                    player.Position = RoleTypeId.Scientist.GetRandomSpawnLocation().Position;
                }
            }
        }

        private (string cassie, string caption) beastName() => Manager?.Beast().FirstOrDefault()?.RoleType switch
        {
            RoleTypeId.Scp939 => ("s c p 9 3 9", "SCP-939"),
            RoleTypeId.Scp096 => ("s c p 0 9 6", "SCP-096"),
            RoleTypeId.Scp049 => ("s c p 0 4 9", "SCP-049"),
            RoleTypeId.Scp106 => ("s c p 1 0 6", "SCP-106"),
            RoleTypeId.Scp173 => ("s c p 1 7 3", "SCP-173"),
            RoleTypeId.Scp3114 => ("s c p 3 1 1 4", "SCP-3114"),
            _ => ("", ""),
        };
    }
}
