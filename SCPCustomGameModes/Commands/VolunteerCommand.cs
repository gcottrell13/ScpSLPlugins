using CommandSystem;
using CustomGameModes.GameModes;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.Commands
{

    [CommandHandler(typeof(ClientCommandHandler))]
    internal class VolunteerCommand : ICommand
    {
        public string Command => "volunteer";
        public bool SanitizeResponse => false;

        public string[] Aliases => Array.Empty<string>();

        public string Description => "volunteer to do something";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "Please specify something to volunteer for.";
                return false;
            }

            var e = arguments.ElementAt(0);

            switch (e) {
                case "scp5000":
                    {
                        if (EventHandlers.CurrentGame is Scp5000Test game)
                        {
                            if (game.Volunteer(Player.Get(sender)))
                                response = "You volunteered to get SCP 5000 in the next round.";
                            else
                                response = "You un-volunteered for SCP 5000";
                            return true;
                        }
                        break;
                    }
            }

            response = $"'{e}' is not a thing you can volunteer for right now.";
            return false;
        }
    }
}
