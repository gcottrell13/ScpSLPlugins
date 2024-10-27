using CommandSystem;
using CustomGameModes.GameModes;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CustomGameModes.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class TTTAddCreditsCommand : ICommand
{
    public string Command => "ttt-credits";
    public bool SanitizeResponse => false;

    public string[] Aliases => new[] { "ttt-credit" };

    public string Description => "Add credits to the specified players";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (EventHandlers.CurrentGame is not TroubleInLC ttt)
        {
            response = "We are not currently playing this game.";
            return false;
        }

        if (arguments.Count != 2)
        {
            response = $"Usage: {Command} [player or *] [credits: uint]";
            return false;
        }

        var target = arguments.ElementAt(0) == "*" ? Player.List : RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var newargs).Select(Player.Get).ToList();

        if (target.Count == 0)
        {
            response = "Please specify players";
            return false;
        }

        if (!int.TryParse(arguments.ElementAt(1), out var creditAmount) || creditAmount <= 0)
        {
            response = "Credits must be an integer greater than zero";
            return false;
        }

        response = $"Added {creditAmount} credits:";

        foreach (var player in target)
        {
            ttt.AddCredits(player, creditAmount);
            response += $"\n{player.DisplayNickname}";
        }

        return true;
    }
}
