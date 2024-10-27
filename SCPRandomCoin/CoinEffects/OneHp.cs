using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(OneHp))]
public class OneHp : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => !playerInfoCache.IsScp && playerInfoCache.Player.Health > 1;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        playerInfoCache.Player.Health = 1;
        hintLines.Add(translation.OneHp);
    }
}