using Exiled.API.Enums;
using Exiled.API.Features.Items;
using SCPRandomCoin.API;
using System.Collections.Generic;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(SpawnGrenade))]
public class SpawnGrenade : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache)
    {
        var player = playerInfoCache.Player;
        // just in case they're trapped in the machine or elevator
        return player.CurrentRoom.Type != RoomType.Lcz914 && player.Lift == null;
    }

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
        grenade.FuseTime = 4;
        grenade.SpawnActive(playerInfoCache.Player.Position);
        hintLines.Add(translation.Grenade);
    }
}