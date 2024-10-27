using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(BecomeSwappable))]
public class BecomeSwappable : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache)
    {
        var gonnaSwap = GoingToSwapCoroutine.GoingToSwap.Contains(playerInfoCache.Player);
        var readySwap = GoingToSwapCoroutine.ReadyToSwap.Contains(playerInfoCache.Player);
        return !readySwap && !gonnaSwap;
    }

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        GoingToSwapCoroutine.ReadyToSwap.Add(playerInfoCache.Player);
        hintLines.Add(translation.DestabilizedSwap);
    }
}