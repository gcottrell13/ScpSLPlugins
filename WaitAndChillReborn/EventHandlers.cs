namespace WaitAndChillReborn;

using Exiled.API.Features.Pickups;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using Mirror;
using UnityEngine;
using static API.API;
using Log = Exiled.API.Features.Log;
using Object = UnityEngine.Object;
using Player = Exiled.API.Features.Player;
using Exiled.API.Features.Roles;
using System.Collections.Generic;
using System.Linq;
using global::WaitAndChillReborn.LobbyRooms;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Features;

internal static class EventHandlers
{

    public static void ForceStart()
    {
        foreach (Player player in Player.List)
        {
            if (player.IsAlive)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.Spectator);
            }
        }

        if (ReadyCheckHandle.IsRunning)
        {
            Timing.KillCoroutines(ReadyCheckHandle);
        }

        Round.IsLobbyLocked = false;
        Timing.CallDelayed(2f, () => { CharacterClassManager.ForceRoundStart(); });
    }

    public static void OnRoundPrepare()
    {
        //var a = LabApi.Features.Wrappers.InteractableToy.Create(networkSpawn: false);
        //NetworkServer.connections.Values.ToList().ForEach(c => a.Base.netIdentity.Add);
        
        PlayerEventHandlers.STARTING_ROUND = true;
        foreach (BaseLobbyRoom room in LobbyAvailableRooms)
            room.OnRoundPrepare();
    }

    private static void OnWaitSetupServer()
    {
        Reset();
        Log.Debug("WaitAndChillReborn.Singleton.Config.DisplayWaitingForPlayersScreen");
        if (!WaitAndChillReborn.Singleton.Config.DisplayWaitingForPlayersScreen)
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;

        Log.Debug("LobbyTimer.IsRunning");
        if (LobbyTimer.IsRunning)
            Timing.KillCoroutines(LobbyTimer);

        Log.Debug("ReadyCheckHandle.IsRunning");
        if (ReadyCheckHandle.IsRunning)
            Timing.KillCoroutines(ReadyCheckHandle);

        Log.Debug("Server.FriendlyFire");
        if (Server.FriendlyFire)
            new FriendlyFireDetectorCommand().Execute(new(["friendlyfiredetector", "pause"]), null, out var _);

        Log.Debug("WaitAndChillReborn.Singleton.Config.DisplayWaitMessage");
        if (WaitAndChillReborn.Singleton.Config.DisplayWaitMessage)
            LobbyTimer = Timing.RunCoroutine(Methods.LobbyTimer());

        Log.Debug("Config.UseReadyCheck");
        if (Config.UseReadyCheck)
            ReadyCheckHandle = Timing.RunCoroutine(ReadyCheck());

        Log.Debug("Clear turned players");
        Scp173Role.TurnedPlayers.Clear();
        Scp096Role.TurnedPlayers.Clear();

        Log.Debug("Server.FriendlyFire");
        Methods.SetupSpawnPoints();
    }

    public static void OnWaitingForPlayers()
    {
        Log.Warn("Waiting players");
        OnWaitSetupServer();
        Timing.CallDelayed(
            1f,
            () =>
            {
                foreach (Pickup pickup in Pickup.List)
                {
                    LockedPickups.Add(pickup);
                    try
                    {
                        if (!pickup.IsLocked)
                        {
                            PickupSyncInfo info = pickup.Base.NetworkInfo;
                            info.Locked = true;
                            pickup.Base.NetworkInfo = info;

                            pickup.Base.GetComponent<Rigidbody>().isKinematic = true;
                        }
                    }
                    catch (System.Exception)
                    {
                        // ignored
                    }
                }

                foreach (Ragdoll ragdoll in Ragdoll.List)
                {
                    LockedRagdolls.Add(ragdoll);
                }
            });
    }

    public static void OnRoundStarted()
    {
        Log.Info("Round started");
        foreach (ThrownProjectile throwable in Object.FindObjectsByType<ThrownProjectile>(FindObjectsSortMode.None))
        {
            if (throwable.TryGetComponent(out Rigidbody rb) && rb.linearVelocity.sqrMagnitude <= 1f)
                continue;

            throwable.transform.position = Vector3.zero;
            Timing.CallDelayed(1f, () => NetworkServer.Destroy(throwable.gameObject));
        }

        foreach (Player player in Player.List)
        {
            player.DisableAllEffects();
            PlayerRoles.Voice.Intercom.TrySetOverride(player.ReferenceHub, false);
            PlayerEventHandlers.removeReadyInName(player);
        }

        if (Server.FriendlyFire)
            new FriendlyFireDetectorCommand().Execute(new(["friendlyfiredetector", "unpause"]), null, out var _);

        if (LobbyTimer.IsRunning)
            Timing.KillCoroutines(LobbyTimer);

        if (ReadyCheckHandle.IsRunning)
        {
            Log.Warn("Killing Ready Check coroutine");
            Timing.KillCoroutines(ReadyCheckHandle);
        }

        foreach (Pickup pickup in Pickup.List)
        {
            if (LockedPickups.Contains(pickup))
            {
                try
                {
                    PickupSyncInfo info = pickup.Base.NetworkInfo;
                    info.Locked = false;
                    pickup.Base.NetworkInfo = info;

                    pickup.Base.GetComponent<Rigidbody>().isKinematic = false;
                }
                catch (System.Exception)
                {
                    // ignored
                }
            }
            else if (Config.CleanupPickups)
            {
                pickup.Destroy();
            }
        }

        if (Config.CleanupRagdolls)
        {
            foreach (Ragdoll ragdoll in Ragdoll.List)
            {
                if (!LockedRagdolls.Contains(ragdoll))
                {
                    ragdoll.Destroy();
                }
            }
        }

        if (Config.TurnedPlayers)
        {
            Scp096Role.TurnedPlayers.Clear();
            Scp173Role.TurnedPlayers.Clear();
        }

        Reset();
    }

    public static IEnumerator<float> ReadyCheck()
    {
        while (!Round.IsStarted)
        {
            int numPlayers = validPlayers.Count;

            if (numPlayers > 0)
            {
                List<Player> ready = [.. ReadyPlayers.Intersect(validPlayers)];
                IsReadyToStartGame = Config.ReadyCheckPercent <= (ready.Count * 100 / numPlayers);
                Round.IsLobbyLocked = !IsReadyToStartGame;
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }
}