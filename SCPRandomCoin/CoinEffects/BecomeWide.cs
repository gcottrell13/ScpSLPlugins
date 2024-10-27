using SCPRandomCoin.API;
using System;
using System.Collections.Generic;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(BecomeWide))]
public class BecomeWide : BaseCoinEffect, ICoinEffectDefinition
{
    public const float wide = 1.1f;
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => Math.Abs(playerInfoCache.Player.Scale.x) < wide;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        playerInfoCache.Player.Scale = new(wide, 1f, wide);
        hintLines.Add(translation.FeelFunny);
    }
}