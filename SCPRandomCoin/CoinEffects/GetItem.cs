using Exiled.API.Features.Pickups;
using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(GetItem))]
public class GetItem : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => !playerInfoCache.IsScp;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var player = playerInfoCache.Player;
        var item = config.ItemList.GetRandomKeyByWeight();
        var pickup = Pickup.CreateAndSpawn(item, player.Position, default, player);
        hintLines.Add(translation.GetItem.Format("item", item));
    }
}