using Exiled.API.Enums;
using SCPRandomCoin.API;
using System.Collections.Generic;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(ReversedControls))]
public class ReversedControls : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => true;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        playerInfoCache.Player.EnableEffect(EffectType.Slowness, 200, 15, false);
        hintLines.Add(translation.ReversedControls);
    }
}