﻿
namespace DankDitties;

using System;
using Exiled.API.Features;
using Configs;
using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;

internal class DankDittiesPlugin : Plugin<Config, Translation>
{
    public static DankDittiesPlugin? Singleton;

    public override void OnEnabled()
    {
        Singleton = this;

        ServerEvent.RoundStarted += EventHandlers.OnRoundStart;

        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Singleton = null;

        ServerEvent.RoundStarted -= EventHandlers.OnRoundStart;

        base.OnDisabled();
    }


    public override string Name => "DankDitties";
    public override string Author => "GCOTTRE";
    public override Version Version => new Version(1, 0, 0);
    public override Version RequiredExiledVersion => new Version(8, 13, 1);
}