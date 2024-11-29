using Exiled.API.Extensions;
using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(LoseItem))]
public class LoseItem : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => !playerInfoCache.Player.IsScp && playerInfoCache.Player.Items.Count > 0;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var player = playerInfoCache.Player;
        var item = player.Items.Where(item => item != player.CurrentItem).GetRandomValue();
        player.RemoveItem(item);
        hintLines.Add(translation.LoseItem.Format("item", item.Type));
    }
}