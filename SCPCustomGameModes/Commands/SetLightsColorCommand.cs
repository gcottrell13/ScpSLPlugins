using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class SetLightsColorCommand : ICommand
    {
        public string Command => "light-color";
        public bool SanitizeResponse => false;

        public string[] Aliases => new[] { "lc" };

        public string Description => "Set lights color";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Color scolor;
            float scale = 255f;

            if (arguments.Count == 1)
            {
                scolor = Color.FromName(arguments.ElementAt(0));
            }
            else if (arguments.Count == 3
                && int.TryParse(arguments.ElementAt(0), out var red)
                && int.TryParse(arguments.ElementAt(1), out var green)
                && int.TryParse(arguments.ElementAt(2), out var blue)
                )
            {
                scolor = Color.FromArgb(red, green, blue);
            }
            else
            {
                response = "Usage: 1 color name, or 3 RGB int components";
                return false;
            }

            var color = new UnityEngine.Color(scolor.R / scale, scolor.G / scale, scolor.B / scale);
            foreach (var room in Room.List)
            {
                room.Color = color;
            }

            response = $"Set color to {color.r} {color.g} {color.b}";
            return true;
        }
    }
}
