using Exiled.Events.EventArgs.Interfaces;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerEvent = Exiled.Events.Handlers.Player;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp914;
using Exiled.API.Extensions;
using Exiled.API.Features;

namespace CustomGameModes.GameModes.Normal
{
    internal class ClassDStarterInventory
    {

        ~ClassDStarterInventory()
        {
            UnsubscribeEventHandlers();
        }

        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------

        public void SubscribeEventHandlers()
        {
            PlayerEvent.Spawned += OnSpawn;

            foreach (var cd in Player.Get(PlayerRoles.RoleTypeId.ClassD))
            {
                SetupClassD(cd);
            }
        }

        public void UnsubscribeEventHandlers()
        {
            PlayerEvent.Spawned -= OnSpawn;
        }

        void OnSpawn(SpawnedEventArgs e)
        {
            SetupClassD(e.Player);
        }


        void SetupClassD(Player player)
        {
            if (player.Role.Type == PlayerRoles.RoleTypeId.ClassD)
            {
                player.AddItem(ItemType.Flashlight);
                player.AddItem(ItemType.Coin);
            }
        }
    }
}
