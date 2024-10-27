using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Player;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features.Items;

namespace SCPTeleporter;

internal static class EventHandlers
{
    public static List<Teleporter> Teleporters = new();
    public static int TeleportersCount { get; private set; } = 0;

    /// <summary>
    /// The item and how many charges it has
    /// </summary>
    public static Dictionary<Item, int> TeleporterPlacers = new();


    public static IEnumerator<float> OnRoundStarted()
    {
        foreach (Teleporter tp in Teleporters)
        {
            tp.Destroy();
        }

        Teleporters.Clear();

        Dictionary<Player, Teleporter> lastTeleports = new();
        Dictionary<Player, DateTime> lastTeleportTimes = new();

        void _playerTryTp(Player player)
        {
            var usableTeleporters = Teleporters.Where(tp => tp.Active && tp.CanBeUsedBy(player)).ToList();
            foreach (Teleporter tp in usableTeleporters)
            {
                if (lastTeleports.TryGetValue(player, out var lastTp) && lastTp == tp)
                    continue;

                var dy = player.Position.y - tp.Position.y;
                var dHoriz = (player.Position - tp.Position).MagnitudeIgnoreY();
                if (dy > -0.1 && dy < 1 && dHoriz <= 0.4)
                {
                    // teleport to another random teleporter

                    if (usableTeleporters.Except(new[] { tp }).GetRandomValue() is Teleporter target)
                    {
                        if (lastTeleportTimes.TryGetValue(player, out var lastTime) && (DateTime.Now - lastTime).TotalSeconds < 7)
                        {
                            player.ShowHint("You cannot teleport again yet.");
                            return;
                        }
                        lastTeleports[player] = target;
                        tp.SetUsed();
                        target.SetUsed();
                        lastTeleportTimes[player] = DateTime.Now;
                        player.EnableEffect(Exiled.API.Enums.EffectType.Flashed, 1.0f);
                        Timing.CallDelayed(0.5f, () =>
                        {
                            player.Position = target.Position;
                        });
                        return;
                    }
                    else
                    {
                        player.ShowHint("Teleporter has no destinations.");
                    }
                }
            }
            // if you aren't standing on any teleporter
            lastTeleports.Remove(player);
        }

        float dt = 1f / 30f;
        while (!Round.IsEnded)
        {
            yield return Timing.WaitForSeconds(dt);

            foreach (Player player in Player.List)
            {
                 _playerTryTp(player);
            }

            foreach (Teleporter tp in Teleporters)
            {
                tp.Update(dt);
            }
        }
    }


    public static void OnRoundEnded(RoundEndedEventArgs ev)
    {
        foreach (Teleporter tp in Teleporters)
        {
            tp.Destroy();
        }

        Teleporters.Clear();
    }


    public static int CreateTeleporter(Player player)
    {
        var tp = new Teleporter(TeleportersCount++, player);
        tp.SetUsed();
        tp.Position = player.Position + UnityEngine.Vector3.down * 0.5f;
        Teleporters.Add(tp);
        return tp.Id;
    }

    public static bool DestroyTeleporterById(int id)
    {
        foreach (Teleporter tp in Teleporters.ToList())
        {
            if (tp.Id != id) 
                continue;
            tp.Destroy();
            Teleporters.Remove(tp);
            return true;
        }
        return false;
    }

    public static void OnSwitchItem(ChangedItemEventArgs ev)
    {
        if (ev.Item == null || !TeleporterPlacers.TryGetValue(ev.Item, out var charges))
            return;
        ev.Player.ShowHint($"Use this item to place a teleporter!\n{charges} use(s) left.");
    }

    public static void OnUseItem(UsingItemCompletedEventArgs ev)
    {
        if (ev.Item == null || !TeleporterPlacers.TryGetValue(ev.Item, out var charges) || charges <= 0)
            return;

        charges--;
        TeleporterPlacers[ev.Item] = charges;
        CreateTeleporter(ev.Player);
        ev.IsAllowed = false;

        ev.Player.ShowHint($"Use this item to place a teleporter!\n{charges} use(s) left.");

        if (charges <= 0)
        {
            ev.Player.RemoveHeldItem(true);
            TeleporterPlacers.Remove(ev.Item);
        }
    }
}
