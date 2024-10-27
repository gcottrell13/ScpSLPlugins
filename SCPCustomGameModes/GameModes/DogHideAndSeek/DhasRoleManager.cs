using CustomGameModes.API;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.GameModes
{
    internal class DhasRoleManager
    {

        public Dictionary<Pickup, Player> ClaimedPickups = new();
        public Dictionary<Player, HashSet<ItemType>> ItemsToDrop = new();
        public Dictionary<Player, DhasRole> PlayerRoles = new();
        public Dictionary<Player, RoleTypeId[]> HurtRoles = new();
        public List<Player> AllowedToUse914 = new();

        // not to be targeted by the GoGetPickup method
        public List<Pickup> UpgradedPickups = new();

        public List<Func<Player, DhasRole>> RoleList { get; private set; }

        public List<DhasRole> ActiveRoles = new();

        public delegate void RemovingTimeHandlerEvent(int seconds);
        public event RemovingTimeHandlerEvent RemovingTime;

        public delegate void PlayerDiedHandlerEvent(Player player);
        public event PlayerDiedHandlerEvent PlayerDied;

        public delegate void PlayerCompleteAllTasksEvent(Player player);
        public event PlayerCompleteAllTasksEvent PlayerCompleteAllTasks;

        public delegate void PlayerCompleteOneTaskEvent(Player player);
        public event PlayerCompleteOneTaskEvent PlayerCompleteOneTask;

        /// <summary>
        /// Should the beast be granted special powers?
        /// </summary>
        public bool BeastSickoModeActivate = false;

        public bool BeastReleased = false;

        private int roleDistroIndex = 0;
        private int roleLoopPoint = 0;
        public string[] RoleDistribution;

        public DhasRoleManager()
        {
            var A = new[] { DhasRoleClassD.name, DhasRoleScientist.name }.ManyRandom();

            roleDistroIndex = 0;
            roleLoopPoint = 7;
            RoleDistribution = new[]
            {
                A[0],
                BeastRole.name,
                A[1],
                DhasRoleGuardian.name,
                DhasRoleDaredevil.name,
                DhasRoleClinger.name,
                DhasRoleMadman.name,
                DhasRoleClinger.name,
                DhasRoleClassD.name,
                DhasRoleScientist.name,
                DhasRoleClassD.name,
            };
        }

        public void ApplyNextRole(Player player)
        {
            ApplyRoleToPlayer(player, RoleDistribution[roleDistroIndex]);
            roleDistroIndex++;
            if (roleDistroIndex >= RoleDistribution.Length)
                roleDistroIndex = roleLoopPoint;
        }

        public Dictionary<string, Func<Player, DhasRole>> RoleClasses() => new()
        {
            {DhasRoleGuardian.name, (player) => new DhasRoleGuardian(player, this) },
            {DhasRoleClassD.name, (player) => new DhasRoleClassD(player, this) },
            {DhasRoleClinger.name, (player) => new DhasRoleClinger(player, this) },
            {DhasRoleDaredevil.name, (player) => new DhasRoleDaredevil(player, this) },
            {DhasRoleMadman.name, (player) => new DhasRoleMadman(player, this) },
            {DhasRoleScientist.name, (player) => new DhasRoleScientist(player, this) },
            {BeastRole.name, (player) => new BeastRole(player, this) },
            {SpectatorRole.name, (player) => new SpectatorRole(player, this) },
        };

        public DhasRole ApplyRoleToPlayer(Player player, string name)
        {
            if (PlayerRoles.TryGetValue(player, out var existingRole))
            {
                existingRole.Stop();
                ActiveRoles.Remove(existingRole);
            }

            PlayerRoles[player] = null;
            var role = RoleClasses()[name](player);
            PlayerRoles[player] = role;

            player.ClearInventory();
            role.GetPlayerReadyAndEquipped();

            ActiveRoles.Add(role);
            return role;
        }

        public void StopAll()
        {
            foreach (var role in ActiveRoles)
            {
                role.Stop();
            }

            UpgradedPickups.Clear();
            ClaimedPickups.Clear();
            ItemsToDrop.Clear();
            PlayerRoles.Clear();
            HurtRoles.Clear();
            AllowedToUse914.Clear();
            ActiveRoles.Clear();
        }

        public void StartAll()
        {
            foreach (var role in ActiveRoles)
            {
                role.Start();
            }
        }

        public int TotalTimeRemovedByTasks()
        {
            int total = 0;
            foreach (DhasRole role in ActiveRoles)
            {
                total += role.Tasks.Sum(task => task.GetMethodInfo().GetCustomAttribute<CrewmateTaskAttribute>()?.Difficulty.DifficultyTime() ?? 0);
            }
            return total;
        }


        public List<DhasRole> Spectators() => ActiveRoles.Where(x => x.player.IsDead).ToList();

        public List<DhasRole> Humans() => ActiveRoles.Where(role => role.player.IsHuman).ToList();

        public List<DhasRole> Beast() => ActiveRoles.Where(role => role.player.IsScp).ToList();

        public void OnRemoveTime(int seconds) => RemovingTime?.Invoke(seconds);

        public void OnPlayerDied(Player player) => PlayerDied?.Invoke(player);

        public void OnPlayerCompleteAllTasks(Player player) => PlayerCompleteAllTasks?.Invoke(player);

        public void OnPlayerCompleteOneTask(Player player) => PlayerCompleteOneTask?.Invoke(player);

        #region 914

        public void CanUse914(Player player)
        {
            AllowedToUse914.Add(player);
        }
        public void CannotUse914(Player player)
        {
            AllowedToUse914.Remove(player);
        }

        #endregion

        #region Inventory

        public void CanDropItem(ItemType item, Player player)
        {
            if (ItemsToDrop.TryGetValue(player, out var items))
            {
                items.Add(item);
            }
            else
            {
                ItemsToDrop[player] = new() { item };
            }
        }

        public void CannotDropItem(ItemType item, Player player)
        {
            if (ItemsToDrop.ContainsKey(player))
                ItemsToDrop[player].Remove(item);
        }

        #endregion

        #region Hurting

        public void PlayerCanHurtRoles(Player player, params RoleTypeId[] role)
        {
            HurtRoles[player] = role;
        }

        public void PlayerCannotHurt(Player player)
        {
            HurtRoles.Remove(player);
        }

        #endregion

    }
}
