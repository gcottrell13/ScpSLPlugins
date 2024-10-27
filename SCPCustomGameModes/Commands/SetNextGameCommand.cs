using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class SetNextGameCommand : ICommand
    {
        public string Command => "set-next-game";
        public bool SanitizeResponse => false;

        public string[] Aliases => new[] { "sng" };

        public string Description => $"Sets the next Game mode. Options: {GameList}";

        public string GameList => string.Join(", ", EventHandlers.GameList.Keys);

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"""
                    Usage: {Command} <gamemode>
                    Games Include:
                    {string.Join("\n", EventHandlers.GameList.Keys)}
                    """;

            if (arguments.Count != 1)
            {
                return false;
            }

            var name = arguments.ElementAt(0);
            if (!EventHandlers.GameList.TryGetValue(name, out var cons))
            {
                return false;
            }

            EventHandlers.SetNextGame(cons);
            response = $"Set current game: {name}";
            return true;
        }
    }
}
