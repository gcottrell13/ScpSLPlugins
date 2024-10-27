using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CustomGameModes.Commands;


[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class DisguiseCommand : ICommand
{
    public string Command => "disguise";
    public bool SanitizeResponse => false;

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Disguises the given players as a role";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count != 2)
        {
            response = $"Usage: {Command} [player] [role]";
            return false;
        }

        var targets = arguments.ElementAt(0) == "*" ? Player.List : RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var newargs).Select(Player.Get).ToList();
        
        if (!targets.Any())
        {
            response = "Invalid player selection";
            return false;
        }

        if (!Enum.TryParse<RoleTypeId>(arguments.ElementAt(1), out var role))
        {
            response = "Invalid role";
            return false;
        }

        foreach (var player in targets)
        {
            player.ChangeAppearance(role, Player.Get(x => x != player));
        }

        response = $"Disguised players as {role}";
        return true;
    }
}
