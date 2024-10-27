using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.GameModes
{
    internal class Tutorial : IGameMode
    {
        public string Name => "Tutorial";

        public string PreRoundInstructions => "";

        public void OnRoundEnd()
        {
            SCPRandomCoin.API.CoinEffectRegistry.EnableAll();
        }

        public void OnRoundStart()
        {
            Round.IsLocked = true;

            foreach (Player player in Player.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.ClassD);
                player.AddItem(ItemType.Flashlight);
            }
            SCPRandomCoin.API.CoinEffectRegistry.DisableAll();
        }

        public void OnWaitingForPlayers()
        {
        }
    }
}
