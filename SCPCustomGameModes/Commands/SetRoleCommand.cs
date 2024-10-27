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
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class SetRoleCommand : ICommand
    {
        public string Command => "set-dhas-role";
        public bool SanitizeResponse => false;

        public string[] Aliases => new[] { "dhassr" };

        public string Description => "set dhas role";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var roleName = arguments.First();

            var player = Player.Get(sender);

            var role = DogHideAndSeek.Manager?.ApplyRoleToPlayer(player, roleName);
            role.Start();

            response = "";
            return true;
        }
    }
}
