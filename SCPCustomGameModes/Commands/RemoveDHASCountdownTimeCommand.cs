using CommandSystem;
using CustomGameModes.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class RemoveDHASCountdownTimeCommand : ICommand
    {
        public string Command => "dhas-remove-time";
        public bool SanitizeResponse => false;

        public string[] Aliases => new[] { "dhasrt" };

        public string Description => "Remove time from the countdown";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 1 || !int.TryParse(arguments.ElementAt(0), out var time))
            {
                response = "Usage: dhasrt <time: int>";
                return false;
            }

            if (DogHideAndSeek.Manager == null)
            {
                response = "Game not currently active";
                return false;
            }

            DogHideAndSeek.Manager?.OnRemoveTime(time);
            response = "Removed time";
            return true;
        }
    }
}
