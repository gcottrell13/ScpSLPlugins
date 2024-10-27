using CommandSystem;
using CustomGameModes.GameModes.Normal;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CustomGameModes.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class ScaleCommand : ICommand
    {
        public string Command => "scale";
        public bool SanitizeResponse => false;

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Change player scale. [player] [x] [y] [z]";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<ReferenceHub> target = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var newargs);

            if (target.Count == 0)
            {
                response = "Please specify players";
                return false;
            }

            int len = arguments.Count;
            float x = float.Parse(arguments.ElementAt(len - 3));
            float y = float.Parse(arguments.ElementAt(len - 2));
            float z = float.Parse(arguments.ElementAt(len - 1));

            foreach (var hub in target)
            {
                var player = Player.Get(hub);
                player.Scale = new UnityEngine.Vector3(x, y, z);
            }

            response = "Set player scale";
            return true;
        }
    }
}
