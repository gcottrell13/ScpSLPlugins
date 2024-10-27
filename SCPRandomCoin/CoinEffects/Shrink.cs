using Exiled.API.Enums;
using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(Shrink))]
public class Shrink : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => !playerInfoCache.IsScp;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var player = playerInfoCache.Player;
        player.Scale = new(0.5f, 0.5f, 0.5f);
        hintLines.Add(translation.FeelFunny);
        player.EnableEffect(EffectType.Slowness, 25);
    }
}