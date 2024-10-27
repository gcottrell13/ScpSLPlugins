using CommandSystem;
using CustomGameModes.API;
using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class SayCountdownCommand : ICommand
    {
        public string Command => "count";
        public bool SanitizeResponse => false;

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Make CASSIE count down";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var time = 10;
            var downStep = 2;

            var player = Player.Get(sender);

            if (arguments.Count > 0)
            {
                int.TryParse(arguments.ElementAt(0), out time);
            }
            if (arguments.Count > 1)
            {
                int.TryParse(arguments.ElementAt(1), out downStep);
            }

            Timing.RunCoroutine(countdown(time, downStep, player));

            response = "started countdown";
            return true;
        }


        private IEnumerator<float> countdown(int start, int downStep, Player player)
        {
            if (start > 20)
            {
                CassieCountdownHelper.SayTimeReminder(start, "left in the game");
                yield break;
            }
            else
            {
                while (start > 0)
                {
                    CountdownHelper.AddCountdown(player, "Command Countdown", TimeSpan.FromSeconds(start));
                    CassieCountdownHelper.SayCountdown(start, downStep);
                    start -= downStep;
                    yield return Timing.WaitForSeconds(downStep);
                }
            }
        }

    }
}
