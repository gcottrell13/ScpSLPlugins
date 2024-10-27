using CommandSystem;
using Exiled.API.Features;
using System;

namespace SCPTeleporter.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class PlaceTeleporterCommand : ICommand
{
    public string Command => "place-tp";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Place a Teleporter at your feet";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var id = EventHandlers.CreateTeleporter(Player.Get(sender));
        response = $"Created teleporter {id}";
        return true;
    }
}
