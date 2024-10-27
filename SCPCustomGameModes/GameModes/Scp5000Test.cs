using CustomGameModes.GameModes.Normal;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using ServerEvent = Exiled.Events.Handlers.Server;
using PlayerEvent = Exiled.Events.Handlers.Player;
using Scp079Event = Exiled.Events.Handlers.Scp079;
using PlayerRoles.RoleAssign;
using Exiled.Events.EventArgs.Server;
using MEC;
using Exiled.API.Features.Doors;
using CustomGameModes.API;
using Exiled.Events.EventArgs.Scp079;
using Exiled.API.Extensions;
using CustomGameModes.Configs;

namespace CustomGameModes.GameModes
{
    internal class Scp5000Test : IGameMode
    {
        public string Name => "SCP 5000 Test";

        public string PreRoundInstructions => "Press <b><color=blue>~</color></b> and type <b><color=blue>.volunteer scp5000</color></b> to receive SCP 5000+ when the game starts";

        public static HashSet<Player> Volunteers = new();

        public HashSet<Player> DoomSlayers = new();

        SCP5000TestConfigs config => (CustomGameModes.Singleton?.Config ?? new()).Scp5000;

        public Scp5000Test()
        {
        }

        public bool Volunteer(Player player)
        {
            if (Volunteers.Contains(player))
            {
                Volunteers.Remove(player);
                Log.Info($"{player.DisplayNickname} has UN-volunteered for SCP 5000+");
                return false;
            }
            Log.Info($"{player.DisplayNickname} has volunteered for SCP 5000+");
            Volunteers.Add(player);
            return true;
        }

        public void OnRoundEnd()
        {
            PlayerEvent.Hurting -= OnHurting;
            PlayerEvent.Shooting -= Shooting;
            Scp079Event.Pinging -= Pinging;

            ServerEvent.RespawningTeam -= RespawningTeam;
            ServerEvent.SelectingRespawnTeam -= SelectingRespawnTeam;
            SCPRandomCoin.API.CoinEffectRegistry.EnableAll();
        }

        public void OnRoundStart()
        {
            PlayerEvent.Hurting += OnHurting;
            PlayerEvent.Shooting += Shooting;
            Scp079Event.Pinging += Pinging;

            ServerEvent.RespawningTeam += RespawningTeam;
            ServerEvent.SelectingRespawnTeam += SelectingRespawnTeam;
            DoSpawnQueue();
            GiveSCP5000ToHuman();

            foreach (Door door in Door.List)
            {
                if (door.Rooms.Count > 1)
                {
                    door.IsOpen = true;
                }
            }

            SCPRandomCoin.API.CoinEffectRegistry.DisableAll();
            SCPRandomCoin.API.CoinEffectRegistry.EnableEffects(config.EnableCoinEffects.ToArray());
        }

        public void OnWaitingForPlayers()
        {
        }


        private void DoSpawnQueue()
        {
            foreach (var player in Player.List)
            {

                if (player.Role.Team == Team.SCPs && player.Role != RoleTypeId.Scp079)
                    // SCPs can stay as they are, unless they are SCP 079.
                    // if one of the non-079 SCPs get chosen as a volunteer, and in doing so leave 079 alone (even just for a microsecond)
                    // the game will activate the overcharge and kill 079.
                    // So to safely avoid this situation, we remove all 079s while not removing any other SCPs, thus ensuring that a 079 is never alone.
                    // After volunteers are chosen, the game may choose to spawn 079 again.
                    continue;
                player.ClearInventory();
                player.Role.Set(RoleTypeId.Spectator);
            }

            Volunteers = Volunteers.Where(p => p.IsConnected).ToHashSet();

            var volunteerPool = ItemPool.ToPool(Volunteers);
            var count = (Player.List.Count / 10) + 1;

            for (int i = 0; i < count; i++)
            {
                if (volunteerPool.Count > 0)
                {
                    var volunteer = volunteerPool.GetNext(p => !DoomSlayers.Contains(p));
                    if (volunteer != null)
                    {
                        volunteer.Role.Set(RoleTypeId.NtfCaptain);
                        DoomSlayers.Add(volunteer);
                        volunteerPool.Remove(volunteer);
                        continue;
                    }
                }

                HumanSpawner.SpawnHumans(new[] { Team.FoundationForces }, 1);
            }

            var spectatorCount = Player.Get(RoleTypeId.Spectator).Count();
            ScpSpawner.SpawnScps(spectatorCount);

            foreach (var human in Player.Get(Team.FoundationForces))
            {
                human.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 25, 9999f);
            }
        }

        private void GiveSCP5000ToHuman()
        {
            foreach (var human in Player.Get(Team.FoundationForces))
            {
                var scp5000 = new SCP5000Handler(human);
                scp5000.SetupScp5000();

                new SCP1392Handler().SetupPlayer(human);

                DoomSlayers.Add(human);
            }
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (!DoomSlayers.Contains(ev.Attacker)) return;

            if (ev.Player.IsScp)
            {
                var scpsMinusZombies = Player.Get(Team.SCPs).Where(s => s.Role != RoleTypeId.Scp0492).ToList();
                ev.DamageHandler.Damage *= 1 + scpsMinusZombies.Count;
            }
        }

        private void Shooting(ShootingEventArgs ev)
        {
            if (!DoomSlayers.Contains(ev.Player)) return;

            ev.Firearm.Ammo = ev.Firearm.MaxAmmo;
        }

        private IEnumerator<float> RespawningTeam(RespawningTeamEventArgs ev)
        {
            yield return Timing.WaitForSeconds(3);
            foreach (Player player in ev.Players)
            {
                new SCP5000Handler(player).SetupScp5000();
            }
        }

        private void SelectingRespawnTeam(SelectingRespawnTeamEventArgs ev)
        {
            ev.Team = Respawning.SpawnableTeamType.NineTailedFox;
        }

        private void Pinging(PingingEventArgs ev)
        {
            if (ev.Type != Exiled.API.Enums.PingType.Human) return;

            var costToRevealInvisible = ev.Scp079.MaxEnergy / 2;
            foreach (Player targetHuman in Player.Get(x => x.IsHuman))
            {
                if ((targetHuman.Position - ev.Position).magnitude < 4f && ev.Scp079.Energy >= costToRevealInvisible)
                {
                    targetHuman.DisableEffect(Exiled.API.Enums.EffectType.Invisible);
                    targetHuman.PlayBeepSound();
                    ev.AuxiliaryPowerCost = costToRevealInvisible;
                }
            }
        }
    }
}
