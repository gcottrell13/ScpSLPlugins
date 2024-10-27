using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HarmonyLib.Code;
using Utils;
using Exiled.API.Features;
using CustomGameModes.GameModes.Normal;

namespace CustomGameModes.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class SCP5000Command : ICommand
    {
        public string Command => "scp5000";
        public bool SanitizeResponse => false;

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Gives a person SCP 5000. Will remove it from the current owner (if there is one)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<ReferenceHub> target = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var newargs);

            if (target.Count == 0)
            {
                response = "Please specify players";
                return false;
            }

            foreach (var hub in target)
            {
                var player = Player.Get(hub);
                var scp = new SCP5000Handler(player);
                scp.SetupScp5000();
            }
            response = $"Gave SCP 5000 to {target.Count} players.";
            return true;
        }
    }
}
