using Exiled.API.Extensions;
using Exiled.API.Features.Pickups;
using InventorySystem;
using Scp914.Processors;
using Scp914;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features.Items;
using UnityEngine;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp914;

namespace CustomGameModes.GameModes
{
    public static class UpgradeHelper
    {
        public static bool Upgrade(UpgradingInventoryItemEventArgs ev, out Item? newItem)
        {
            newItem = null;

            if (_tryUpgradeCustom(ev.Item.Type, ev.KnobSetting, out var newItemType))
            {
                if (newItemType != ev.Item.Type)
                    newItem = Item.Create(newItemType);
                else
                    newItem = ev.Item;
                return true;
            }

            if (InventoryItemLoader.AvailableItems.TryGetValue(ev.Item.Type, out var value) && value.TryGetComponent<Scp914ItemProcessor>(out var processor))
            {
                var newItemBase = processor.UpgradeInventoryItem(ev.KnobSetting, ev.Item.Base);

                if (newItemBase.ResultingItems.Length == 0)
                {
                    newItem = ev.Item;
                    return true;
                }

                newItem = Item.Get(newItemBase.ResultingItems[0]);
                return true;
            }

            return false;
        }

        public static bool Upgrade(UpgradingPickupEventArgs ev, out Pickup newPickup) 
        {
            newPickup = null;

            if (_tryUpgradeCustom(ev.Pickup.Type, ev.KnobSetting, out var newItemType))
            {
                if (newItemType != ev.Pickup.Type)
                    newPickup = Pickup.Create(newItemType);
                else
                    newPickup = ev.Pickup;
                return true;
            }

            if (InventoryItemLoader.AvailableItems.TryGetValue(ev.Pickup.Type, out var value) && value.TryGetComponent<Scp914ItemProcessor>(out var processor))
            {
                var newPickupBase = processor.UpgradePickup(ev.KnobSetting, ev.Pickup.Base);

                if (newPickupBase.ResultingPickups.Length == 0)
                {
                    newPickup = ev.Pickup;
                    return true;
                }

                newPickup = Pickup.Get(newPickupBase.ResultingPickups[0]);
                return true;
            }

            return false;
        }

        private static bool _tryUpgradeCustom(ItemType item, Scp914KnobSetting setting, out ItemType newItem)
        {
            if (item.IsKeycard() && setting >= Scp914KnobSetting.Fine)
            {
                newItem = item switch
                {
                    ItemType.KeycardFacilityManager => ItemType.KeycardO5,
                    _ => ItemType.KeycardFacilityManager,
                };
                return true;
            }
            else if (item == ItemType.Flashlight)
            {
                // don't take their flashlight =(
                newItem = ItemType.Flashlight;
                return true;
            }

            newItem = ItemType.None;
            return false;
        }
    }
}
