using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using PlayerEvent = Exiled.Events.Handlers.Player;
using MapEvent = Exiled.Events.Handlers.Map;
using ServerEvent = Exiled.Events.Handlers.Server;
using Exiled.API.Features;
using MEC;
using Exiled.Events.EventArgs.Interfaces;
using UnityEngine;
using Exiled.API.Features.Items;
using PlayerRoles;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using CustomGameModes.API;
using Exiled.API.Structs;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Pickups;
using CustomGameModes.Configs;

namespace CustomGameModes.GameModes
{
    internal class Infection : IGameMode
    {
        public string Name => "Infection";

        public string PreRoundInstructions => "";

        CoroutineHandle roundLoop;

        RoleTypeId SurvivorRole;
        RoleTypeId InfectedRole;
        RoleTypeId EscapeeRole;
        private Vector3 SCPSpawnPoint => RoleTypeId.Scp939.GetRandomSpawnLocation().Position;

        HashSet<Player> Infected = new HashSet<Player>();
        HashSet<Player> Survivors = new HashSet<Player>();
        HashSet<Player> Escapees = new HashSet<Player>();

        bool ClosedLcz = false;
        bool ClosedHcz = false;

        InfectionConfig config => (CustomGameModes.Singleton?.Config ?? new()).Infection;

        public Infection()
        {
            RoleTypeId parsedRole;
            InfectedRole = Enum.TryParse(config.InfectedScpChance.GetRandom(), out parsedRole) ? parsedRole : RoleTypeId.Scp0492;
            EscapeeRole = Enum.TryParse(config.EscapeeScpChance.GetRandom(), out parsedRole) ? parsedRole : RoleTypeId.ChaosConscript;
            SurvivorRole = Enum.TryParse(config.SurvivorScpChance.GetRandom(), out parsedRole) ? parsedRole : RoleTypeId.ClassD;

            // set this so that the SCPs can properly get a win screen, even if they're all zombies
            Round.EscapedScientists = -1;
            Round.EscapedDClasses = -1;
        }

        public void OnRoundStart()
        {
            PlayerEvent.TriggeringTesla += DeniableEvent;
            PlayerEvent.Escaping += OnEscape;
            PlayerEvent.Dying += OnDied;
            PlayerEvent.InteractingDoor += OnDoor;
            PlayerEvent.ActivatingWarheadPanel += DeniableEvent;
            PlayerEvent.InteractingElevator += OnElevator; 

            ServerEvent.RespawningTeam += DeniableEvent;

            MapEvent.AnnouncingScpTermination += DeniableEvent;

            roundLoop = Timing.RunCoroutine(_roundLoop());
        }
        public void OnRoundEnd()
        {
            PlayerEvent.TriggeringTesla -= DeniableEvent;
            PlayerEvent.Escaping -= OnEscape;
            PlayerEvent.Dying -= OnDied;
            PlayerEvent.InteractingDoor -= OnDoor;
            PlayerEvent.ActivatingWarheadPanel -= DeniableEvent;
            PlayerEvent.InteractingElevator -= OnElevator;

            ServerEvent.RespawningTeam -= DeniableEvent;

            MapEvent.AnnouncingScpTermination -= DeniableEvent;

            if (roundLoop.IsRunning)
                Timing.KillCoroutines(roundLoop);
        }

        public void OnWaitingForPlayers()
        {
            OnRoundEnd();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------

        private IEnumerator<float> _roundLoop()
        {
            var players = Player.List.ToList().ManyRandom();

            for (var i = 0; i < players.Count; i++)
            {
                Action<Player> setup = i switch
                {
                    0 => SetupInfected,
                    1 => SetupSurvivor,
                    2 => SetupInfected,
                    _ => SetupSurvivor,
                };

                setup(players[i]);
            }

            foreach (var hczRoom in Room.Get(ZoneType.HeavyContainment))
            {
                hczRoom.TurnOffLights(9999f);
            }
            foreach (var ezRoom in Room.Get(ZoneType.Entrance))
            {
                ezRoom.TurnOffLights(9999f);
            }
            foreach (var pickup in Pickup.List)
            {
                pickup.Destroy();
            }

            while (Round.InProgress)
            {

                var extraPlayers = Player.List.ToHashSet().Except(Infected.Union(Escapees).Union(Survivors)).ToHashSet();
                if (extraPlayers.Count > 0)
                {
                    foreach (Player extra in extraPlayers)
                    {
                        if (Warhead.IsDetonated)
                        {
                            SetupEscapee(extra);
                        }
                        else if (Map.IsLczDecontaminated)
                        {
                            SetupInfected(extra);
                        }
                        else
                        {
                            SetupSurvivor(extra);
                        }
                    }
                }

                try
                {

                    var survivorsAndEscapees = Survivors.Union(Escapees).ToHashSet();
                    var inLcz = survivorsAndEscapees.Where(p => p.Zone == ZoneType.LightContainment).Count();
                    var inHcz = survivorsAndEscapees.Where(p => p.Zone == ZoneType.HeavyContainment).Count();
                    var inEz = survivorsAndEscapees.Where(p => (p.Zone & ZoneType.Entrance) != 0).Count(); // the HCZ-Entrance checkpoints count as being in both zones
                    var inSurface = survivorsAndEscapees.Where(p => p.Zone == ZoneType.Surface).Count();

                    if (Player.Get(p => p.Zone == ZoneType.LightContainment).Count() == 0) TryCloseDoorsInZone(ZoneType.HeavyContainment, ref ClosedHcz);

                    // SCPHUD:
                    {
                        IEnumerable<string> lines()
                        {
                            if (inLcz > 0) yield return $"<color=orange>LCZ: {inLcz}</color>";
                            if (inHcz > 0) yield return $"<color=yellow>HCZ: {inHcz}</color>";
                            if (inEz > 0) yield return $"<color=green>Entrance: {inEz}</color>";
                            if (inSurface > 0) yield return $"<color=blue>Surface: {inSurface}</color>";
                        }

                        var message = string.Join(" | ", lines());

                        foreach (var scp in Infected)
                        {
                            scp.Broadcast(2, $"""
                            Where to Find Your Targets ({SurvivorStr} and {EscapeeStr}):
                            {message}
                            """, shouldClearPrevious: true);
                        }
                    }

                    // ChaosHUD:
                    {
                        if (Escapees.Count > 0)
                        {
                            var scpMsg = $"{InfectedStr}: {Infected.Count}";
                            var cdMsg = $"{SurvivorStr}: {Survivors.Count}";
                            foreach (var ci in Escapees)
                            {
                                ci.Broadcast(2, $"{scpMsg} - {cdMsg}", shouldClearPrevious: true);
                            }
                        }
                    }
                }
                catch
                {

                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------

        public void SetupInfected(Player player)
        {
            player.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.None);
            player.Teleport(SCPSpawnPoint + Vector3.up);
            Infected.Add(player);
            player.CustomInfo = "<color=#FF6448>Infected</color>";

            if (InfectedRole != RoleTypeId.Scp0492)
            {
                Timing.CallDelayed(0.1f, () => player.ChangeAppearance(InfectedRole));
            }

            if (Escapees.Count > 0)
            {
                Timing.CallDelayed(15, () => ShowEscapedMessage(player));
            }
            else
            {
                Timing.CallDelayed(15, () => ShowInfectedStartupMessage(player));
            }
        }

        public void SetupSurvivor(Player player)
        {
            player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.UseSpawnpoint);
            player.ClearInventory();
            player.CustomInfo = "<color=#FF9966>Survivor</color>";

            player.CurrentItem = player.AddItem(ItemType.KeycardO5);
            player.AddItem(ItemType.Lantern);

            if (SurvivorRole != RoleTypeId.ClassD)
            {
                Timing.CallDelayed(1f, () => player.ChangeAppearance(SurvivorRole));
            }

            Survivors.Add(player);

            Timing.CallDelayed(15, () => ShowSurvivorStartupMessage(player));
        }

        public void SetupEscapee(Player player)
        {
            player.ClearInventory();
            player.Role.Set(RoleTypeId.ChaosConscript);

            player.AddItem(ItemType.GunCOM18);
            AddFlashlightToGun(player);

            Survivors.Remove(player);
            Escapees.Add(player);

            player.CustomInfo = "<color=#32CD32>Escapee</color>";

            if (EscapeeRole != RoleTypeId.ChaosConscript)
            {
                Timing.CallDelayed(1f, () => player.ChangeAppearance(EscapeeRole));
            }

            Timing.CallDelayed(15, () => ShowEscapeeMessage(player));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------

        public string EscapeeStr => $"<b><color=green>{EscapeeRole}</color></b>";
        public string InfectedStr => $"<b><color=red>{InfectedRole}</color></b>";
        public string SurvivorStr => $"<b><color=orange>{SurvivorRole}</color></b>";

        public void ShowEscapedMessage(Player scp)
        {
            scp.ShowHint($"""
                A Survivor {SurvivorStr} has Escaped and returned as {EscapeeStr}!
                Beware!






                """, 20);
        }

        public void ShowSurvivorStartupMessage(Player classd)
        {
            classd.ShowHint($"""
                Escape The Facility!
                Avoid the Infected {InfectedStr}s!
                Tesla Gates are OFF!






                """, 15);
        }

        public void ShowInfectedStartupMessage(Player scp)
        {
            scp.ShowHint($"""
                Tesla Gates are OFF!
                Kill the Survivor {SurvivorStr}s Before They Escape!






                """, 15);
        }

        public void ShowEscapeeMessage(Player esc)
        {
            esc.ShowHint($"""
                Go Kill all the Infected {InfectedStr}s!
                Beware, you can still become Infected!






                """, 15);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------

        public void DeniableEvent(IDeniableEvent ev)
        {
            ev.IsAllowed = false;
        }

        public void OnDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Door.IsGate)
            {
                return;
            }
            if (ev.Door.IsOpen && !ev.Door.IsElevator && ev.Door.Type != DoorType.Airlock)
            {
                // cannot close doors
                DeniableEvent(ev);
            }
            else if (ev.Door.Zone == ZoneType.HeavyContainment && ClosedHcz)
            {
                DeniableEvent(ev);
            }
        }

        public void OnElevator(InteractingElevatorEventArgs ev)
        {
            // the elevators cannot be operated from HCZ.
            // the LCZ elevators effectively become one-way to HCZ.
            // the Nuke and SCP049 elevators become disabled.
            if (ev.Player.Zone == ZoneType.HeavyContainment)
            {
                DeniableEvent(ev);
            }
        }

        public void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (ev.Player.Role == SurvivorRole) DeniableEvent(ev);
        }

        public IEnumerator<float> OnDied(DyingEventArgs ev)
        {
            if (Survivors.Contains(ev.Player) || Escapees.Contains(ev.Player))
            {
                Survivors.Remove(ev.Player);
                Escapees.Remove(ev.Player);
                ev.Player.ClearInventory();

                if (Infected.Contains(ev.Attacker))
                {
                    yield return Timing.WaitForSeconds(0.5f);
                    SetupInfected(ev.Player);
                    ev.Player.Teleport(ev.Attacker.Position);
                }
            }
            else if (Infected.Contains(ev.Player))
            {
                Infected.Remove(ev.Player);
                ev.Player.ClearInventory();

                if (Escapees.Contains(ev.Attacker))
                {
                    yield return Timing.WaitForSeconds(5f);
                    SetupEscapee(ev.Player);
                }
            }
        }

        public void OnEscape(EscapingEventArgs e)
        {
            e.IsAllowed = false;

            if (!Survivors.Contains(e.Player)) 
                return;

            SetupEscapee(e.Player);

            foreach (var scp in Player.List.Where(p => p.Role == InfectedRole))
            {
                ShowEscapedMessage(scp);
            }
        }

        public void AddFlashlightToGun(Player player)
        {
            foreach (Item item in player.Items)
            {
                if (item is not Firearm weapon) continue;

                if (AttachmentIdentifier.Get(weapon.FirearmType, InventorySystem.Items.Firearms.Attachments.AttachmentName.Flashlight) is AttachmentIdentifier att && att.Code != 0)
                    weapon.AddAttachment(att);
            }
        }

        public void TryCloseDoorsInZone(ZoneType zone, ref bool hasClosed)
        {
            if (!hasClosed && !Player.List.Any(p => p.Zone == zone))
            {
                hasClosed = true;
                foreach (Door door in Door.List)
                {
                    if (door.Zone == zone)
                    {
                        door.IsOpen = false;
                    }
                }
            }
        }
    }
}
