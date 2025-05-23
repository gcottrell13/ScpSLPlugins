﻿namespace WaitAndChillReborn;

using System;
using Exiled.API.Features;
using HarmonyLib;
using Configs;
using Config = global::WaitAndChillReborn.Configs.Config;
using static API.API;
using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using MapEvent = Exiled.Events.Handlers.Map;
using Scp106Event = Exiled.Events.Handlers.Scp106;
using Exiled.Events.EventArgs.Interfaces;

public class WaitAndChillReborn : Plugin<Config, Translation>
{
    public static WaitAndChillReborn Singleton;

    private Harmony _harmony;

    public override void OnEnabled()
    {
        Singleton = this;
        RegisterEvents();
        _harmony = new Harmony($"michal78900.wacr-{DateTime.Now.Ticks}");
        _harmony.PatchAll();
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        UnRegisterEvents();
        Singleton = null;
        _harmony.UnpatchAll();
        base.OnDisabled();
    }

    internal static void RegisterEvents()
    {
        ServerEvent.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;

        PlayerEvent.Verified += PlayerEventHandlers.OnVerified;
        PlayerEvent.Spawned += PlayerEventHandlers.OnSpawned;
        PlayerEvent.Dying += PlayerEventHandlers.OnDying;
        PlayerEvent.Died += PlayerEventHandlers.OnDied;
        PlayerEvent.TogglingNoClip += PlayerEventHandlers.OnNoclip;
        PlayerEvent.Escaping += OnDeniableEvent;
        PlayerEvent.Left += PlayerEventHandlers.OnDisconnect;

        MapEvent.AnnouncingScpTermination += OnDeniableEvent;
        MapEvent.ChangingIntoGrenade += OnDeniableEvent;

        PlayerEvent.SpawningRagdoll += PlayerEventHandlers.OnSpawnRagdoll;
        PlayerEvent.IntercomSpeaking += OnDeniableEvent;
        PlayerEvent.DroppingItem += OnDeniableEvent;
        PlayerEvent.DroppingAmmo += OnDeniableEvent;
        PlayerEvent.InteractingDoor += PlayerEventHandlers.OnInteractingDoor;
        PlayerEvent.InteractingElevator += OnDeniableEvent;
        PlayerEvent.InteractingLocker += OnDeniableEvent;

        Scp106Event.Teleporting += OnDeniableEvent;

        ServerEvent.RoundStarted += EventHandlers.OnRoundStarted;
    }

    internal static void UnRegisterEvents()
    {
        ServerEvent.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;

        PlayerEvent.Verified -= PlayerEventHandlers.OnVerified;
        PlayerEvent.Spawned -= PlayerEventHandlers.OnSpawned;
        PlayerEvent.Dying -= PlayerEventHandlers.OnDying;
        PlayerEvent.Died -= PlayerEventHandlers.OnDied;
        PlayerEvent.TogglingNoClip -= PlayerEventHandlers.OnNoclip;
        PlayerEvent.Escaping -= OnDeniableEvent;
        PlayerEvent.Left -= PlayerEventHandlers.OnDisconnect;

        MapEvent.AnnouncingScpTermination -= OnDeniableEvent;
        MapEvent.ChangingIntoGrenade -= OnDeniableEvent;

        PlayerEvent.SpawningRagdoll -= PlayerEventHandlers.OnSpawnRagdoll;
        PlayerEvent.IntercomSpeaking -= OnDeniableEvent;
        PlayerEvent.DroppingItem -= OnDeniableEvent;
        PlayerEvent.DroppingAmmo -= OnDeniableEvent;
        PlayerEvent.InteractingDoor -= PlayerEventHandlers.OnInteractingDoor;
        PlayerEvent.InteractingElevator -= OnDeniableEvent;
        PlayerEvent.InteractingLocker -= OnDeniableEvent;

        Scp106Event.Teleporting -= OnDeniableEvent;

        ServerEvent.RoundStarted -= EventHandlers.OnRoundStarted;
    }


    private static void OnDeniableEvent(IDeniableEvent ev)
    {
        if (!IsLobby)
            return;
        ev.IsAllowed = false;
    }

    public override string Name => "WaitAndChillReborn";
    public override string Author => "GCOTTRE";
    public override Version Version => new Version(1, 0, 3);
    public override Version RequiredExiledVersion => new Version(9, 6, 0);
}
