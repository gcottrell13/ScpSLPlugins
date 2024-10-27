using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace SCPRandomCoin.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class ForceEffectCommand : ICommand
{
    public string Command => "coin-effect";

    public bool SanitizeResponse => false;

    public string[] Aliases => new[] { "coin" };

    public string Description => $"{Command} [CoinEffects] <player | *>\nForces a coin effect on a player.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        List<Player> players = new();
        if (arguments.Count == 1)
        {
            players.Add(Player.Get(sender));
        }
        else if (arguments.Count == 2)
        {
            players.AddRange(
                arguments.ElementAt(1) == "*" ? Player.List : RAUtils.ProcessPlayerIdOrNamesList(arguments, 1, out var newargs).Select(Player.Get)
            );
        }
        else
        {
            response = $"Invalid number of parameters.\n{Description}";
            return false;
        }

        var effect = arguments.ElementAt(0);

        foreach (var player in players)
        {
            EffectHandler.ForceCoinEffect(player, effect);
        }

        response = $"Affected {players.Count} players";
        return true;
    }
}
