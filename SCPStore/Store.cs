using Exiled.API.Features;
using Exiled.API.Features.Items;
using SCPStore.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SCPStore;

public class Store
{
    /// <summary>
    /// Short description of how to access the store
    /// </summary>
    public static string AccessMethod => SCPStore.Singleton?.Config.StoreAccessMethod ?? "";

    public static Dictionary<Player, Store> ByPlayerDict = new();

    internal static Dictionary<string, StoreItem> RegisteredStoreItems = new();

    public Store(Player player)
    {
        Player = player;
        PlayerHintMenu = new PlayerHintMenu(player)
        {
            OnHide = Dispose,
        };
    }

    public int MaxLinesPerColumn { get; set; } = 6;

    private int itemsAddedInThisColumn = 0;

    public Player Player { get; }
    public PlayerHintMenu PlayerHintMenu { get; }

    public Func<Player, string, int> GetCredits;
    public Action<Player, string, int> AddCredits;
    public Action? OnHide;

    public void AddShowBalance(params string[] currencies)
    {
        AddStoreItem(new HintMenuItem(
            text: () =>
            {
                var balance = string.Join(", ", currencies.Select(c => string.Format(c, GetCredits(Player, c))));
                return $"""
                    Your balance: {balance}
                    -
                    """;
            }));
    }

    public void AddStoreItem(ItemType name, int cost, string currency, string? displayName = null, Action<Item>? onBuy=null)
    {
        displayName ??= name.ToString();

        AddStoreItem(new HintMenuItem(
            actionName: $"Buy {displayName}",
            text: () =>
            {
                var credits = GetCredits(Player, currency);
                Func<string, string> color = credits >= cost ? ColorHelper.Green : ColorHelper.Red;
                var creditDisplay = color(string.Format(currency, cost));
                var text = $"{displayName} - {creditDisplay}</color>";
                return text;
            },
            onSelect: () =>
            {
                if (Player.IsInventoryFull)
                {
                    return ColorHelper.Red($"Inventory full, could not buy {displayName}");
                }
                var credits = GetCredits(Player, currency);
                if (credits >= cost)
                {
                    AddCredits(Player, currency, - cost);
                    var newItem = Player.AddItem(name);
                    onBuy?.Invoke(newItem);
                    if (!Player.IsScp)
                        return ColorHelper.Green($"Bought {displayName}");
                    Player.CurrentItem = newItem;
                    return ColorHelper.Green($"Bought {displayName} - {AccessMethod}");
                }
                return ColorHelper.Red($"Insufficient funds for {displayName}");
            }
        ));
    }

    public void AddStoreItem(string name, int cost, string currency)
    {
        if (RegisteredStoreItems.TryGetValue(name, out var customItem))
        {
            AddStoreItem(customItem.BaseItem, cost, currency, displayName: customItem.DisplayName, onBuy: customItem.OnBuy);
        }
        else if (Enum.TryParse<ItemType>(name, out var type))
        {
            AddStoreItem(type, cost, currency);
        }
        else
        {
            throw new ArgumentException($"unknown item name: '{name}'");
        }
    }

    public void AddInventoryDisplay()
    {
        AddStoreItem(new HintMenuItem(text: () => "Your Inventory:\n-")
        {
            ColumnBreakAfter = false,
        });
        if (Player.CurrentItem != null)
        {
            AddStoreItem(new HintMenuItem(
                actionName: $"Stow {Player.CurrentItem.Type}",
                text: () => ColorHelper.BlueGreen($"Stow {Player.CurrentItem.Type}"),
                onSelect: () =>
                {
                    Player.CurrentItem = null;
                    return $"Put Away Item";
                }
            )
            {
                ColumnBreakAfter = false,
            });
            AddStoreItem(new HintMenuItem(
                actionName: $"Drop {Player.CurrentItem.Type}",
                text: () => ColorHelper.BlueGreen($"Drop {Player.CurrentItem.Type}"),
                onSelect: () =>
                {
                    Player.DropItem(Player.CurrentItem);
                    return $"Dropped Item";
                }
            )
            {
                ColumnBreakAfter = false,
            });
        }
        foreach (Item item in Player.Items)
        {
            var thisItem = item;
            if (item == Player.CurrentItem)
                continue;
            AddStoreItem(new HintMenuItem(
                actionName: $"Select {thisItem.Type}",
                text: () => ColorHelper.Yellow(thisItem.Type.ToString()),
                onSelect: () =>
                {
                    Player.CurrentItem = thisItem;
                    return $"Switched to {thisItem.Type}";
                }
            )
            {
                ColumnBreakAfter = false,
            });
            //list.Add(new HintMenuItem(
            //    actionName: $"Drop {thisItem.Type}",
            //    text: () => $"Drop {thisItem.Type}",
            //    onSelect: () =>
            //    {
            //        ev.Player.DropItem(thisItem);
            //        return $"Dropped Item";
            //    }
            //));
        }
        PlayerHintMenu.Items.Last().ColumnBreakAfter = true;
    }

    public void AddStoreItem(HintMenuItem item)
    {
        itemsAddedInThisColumn++;
        if (item.ColumnBreakAfter == null && itemsAddedInThisColumn >= MaxLinesPerColumn)
            item.ColumnBreakAfter = true;
        PlayerHintMenu.AddItem(item);
    }

    public void DisplayAndCountdown(string headerText, float seconds)
    {
        PlayerHintMenu.CountdownToSelect(headerText, seconds);
    }

    /// <summary>
    /// Returns false if a store does not exist. if false, the out var will be the new store that you can add items to.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool TryCycleStoreIfExist(Player player, string storeCycleInstructions, out Store newStore)
    {
        if (ByPlayerDict.TryGetValue(player, out newStore))
        {
            var timeToWaitForSelection = 1f;
            newStore.PlayerHintMenu.Next();
            var current = newStore.PlayerHintMenu.GetCurrent();
            if (current != null)
                newStore.PlayerHintMenu.CountdownToSelect($"{storeCycleInstructions}{current.ActionName} in: ", timeToWaitForSelection);
            return true;
        }
        newStore = new Store(player);
        ByPlayerDict.Add(player, newStore);
        return false;
    }

    protected void Dispose()
    {
        ByPlayerDict.Remove(Player);
        OnHide?.Invoke();
    }

    public static void RegisterCustomItem(StoreItem item)
    {
        RegisteredStoreItems.Add(item.Identifier, item);
    }

    public static void UnregisterCustomItem(StoreItem item)
    {
        RegisteredStoreItems.Remove(item.Identifier);
    }
}

public abstract class StoreItem
{
    public string Identifier { get; }
    public string DisplayName { get; }
    public ItemType BaseItem { get; }

    public StoreItem(string identifier, string displayName, ItemType baseItem)
    {
        Identifier = identifier;
        DisplayName = displayName;
        BaseItem = baseItem;
    }

    public abstract void OnBuy(Item item);
}