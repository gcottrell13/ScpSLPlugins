namespace SCPTeleporter;

using System;
using Exiled.API.Features;
using Configs;
using ServerEvent = Exiled.Events.Handlers.Server;
using PlayerEvent = Exiled.Events.Handlers.Player;


internal class SCPTeleporter : Plugin<Config, Translation>
{
    public static SCPTeleporter? Singleton;

    public override void OnEnabled()
    {
        Singleton = this;
        ServerEvent.RoundStarted += EventHandlers.OnRoundStarted;
        ServerEvent.RoundEnded += EventHandlers.OnRoundEnded;
        PlayerEvent.ChangedItem += EventHandlers.OnSwitchItem;
        PlayerEvent.UsingItemCompleted += EventHandlers.OnUseItem;

        SCPStore.Store.RegisterCustomItem(new TeleporterStoreItem());
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Singleton = null;
        ServerEvent.RoundStarted -= EventHandlers.OnRoundStarted;
        ServerEvent.RoundEnded -= EventHandlers.OnRoundEnded;
        PlayerEvent.ChangedItem -= EventHandlers.OnSwitchItem;
        PlayerEvent.UsingItemCompleted -= EventHandlers.OnUseItem;
        SCPStore.Store.UnregisterCustomItem(new TeleporterStoreItem());
        base.OnDisabled();
    }


    public override string Name => "Teleporter";
    public override string Author => "GCOTTRE";
    public override Version Version => new Version(1, 0, 1);
    public override Version RequiredExiledVersion => new Version(8, 13, 1);
}
