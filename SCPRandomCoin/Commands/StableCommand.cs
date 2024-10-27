using CommandSystem;
using Exiled.API.Features;
using SCPRandomCoin.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.Commands;


[CommandHandler(typeof(ClientCommandHandler))]
internal class StableCommand : ICommand
{
    public string Command => "coin-identity-stable";

    public bool SanitizeResponse => false;

    public string[] Aliases => new[] { ShortAlias };

    public const string ShortAlias = "noswap";

    public string Description => "If your identity is about to change, cancel the change.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (GoingToSwapCoroutine.GoingToSwap.Contains(Player.Get(sender)) == false)
        {
            response = "You are not about to swap.";
            return false;
        }

        GoingToSwapCoroutine.GoingToSwap.Remove(Player.Get(sender));
        response = "Successfully cancelled the swap.";
        return true;
    }
}
