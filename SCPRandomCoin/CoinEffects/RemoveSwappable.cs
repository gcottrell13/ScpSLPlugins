using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(RemoveSwappable))]
public class RemoveSwappable : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache)
    {
        var readySwap = GoingToSwapCoroutine.ReadyToSwap.Contains(playerInfoCache.Player);
        return readySwap;
    }

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        GoingToSwapCoroutine.ReadyToSwap.Remove(playerInfoCache.Player);
        hintLines.Add(translation.StabilizedSwap);
    }
}