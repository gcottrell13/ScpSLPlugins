using CommandSystem;
using System;
using System.Linq;

namespace SCPTeleporter.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class DestroyTeleporterCommand : ICommand
{
    public string Command => "rm-tp";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Remove a teleporter by id";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count != 1) 
        {
            response = "Arguments are: [teleporter id]";
            return false;
        }
        var id = int.Parse(arguments.ElementAt(0));
        if (EventHandlers.DestroyTeleporterById(id))
        {
            response = "Destroyed teleporter";
            return true;
        }

        response = "Could not find teleporter";
        return false;
    }
}
