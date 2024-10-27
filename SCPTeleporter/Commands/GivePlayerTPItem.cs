using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using System;
using System.Linq;
using Utils;

namespace SCPTeleporter.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class GivePlayerTPItem : ICommand
{
    public string Command => "give-tp";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "gives a player a TP item";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count <= 0)
        {
            response = "must provide a player name";
            return false;
        }

        var itemType = ItemType.Medkit;
        if (arguments.Count == 2 && !Enum.TryParse(arguments.ElementAt(1), out itemType))
        {
            response = "must provide a valid item type";
            return false;
        }

        var charges = 1;
        if (arguments.Count == 3 && (!int.TryParse(arguments.ElementAt(2), out charges) || charges <= 0))
        {
            response = "must provide a valid number of charges";
            return false;
        }

        var targets = arguments.ElementAt(0) == "*" ? Player.List : RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var newargs).Select(Player.Get).ToList();
    
        foreach (var target in targets)
        {
            var item = Item.Create(itemType, target);
            target.AddItem(item);
            EventHandlers.TeleporterPlacers[item] = charges;
        }

        response = $"Gave a TP item to {targets.Count} specified players";
        return true;
    }
}
