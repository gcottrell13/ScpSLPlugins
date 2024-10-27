using Exiled.API.Extensions;
using Exiled.API.Features.Pickups;
using InventorySystem.Items.Usables.Scp330;
using SCPRandomCoin.API;
using System.Collections.Generic;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(GetCandy))]
public class GetCandy : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => !playerInfoCache.IsScp;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var player = playerInfoCache.Player;
        var item = CandyTypes.GetRandomValue();
        var pickup = (Exiled.API.Features.Pickups.Scp330Pickup)Pickup.CreateAndSpawn(ItemType.SCP330, player.Position, default, player);
        pickup.Candies.Add(item);
        hintLines.Add(translation.GetItem.Format("item", $"{item} Candy"));
    }

    public static readonly List<CandyKindID> CandyTypes = new()
    {
        CandyKindID.Rainbow,
        CandyKindID.Green,
        CandyKindID.Green,
        CandyKindID.Green,
        CandyKindID.Green,
        CandyKindID.Blue,
        CandyKindID.Blue,
        CandyKindID.Blue,
        CandyKindID.Red,
        CandyKindID.Red,
        CandyKindID.Red,
        CandyKindID.Purple,
        CandyKindID.Purple,
        CandyKindID.Yellow,
        CandyKindID.Yellow,
        CandyKindID.Pink,
        CandyKindID.Black,
        CandyKindID.Black,
        CandyKindID.Gray,
        CandyKindID.Gray,
        CandyKindID.Orange,
        CandyKindID.Orange,
        CandyKindID.White,
        CandyKindID.White,
        CandyKindID.Evil,
    };
}