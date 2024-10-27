using MEC;
using SCPRandomCoin.API;
using System.Collections.Generic;
using System.Linq;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(DoSwap))]
public class DoSwap : ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache)
    {
        var gonnaSwap = GoingToSwapCoroutine.GoingToSwap.Contains(playerInfoCache.Player);
        var readySwap = GoingToSwapCoroutine.ReadyToSwap.Contains(playerInfoCache.Player);
        return !readySwap && !gonnaSwap && GoingToSwapCoroutine.ReadyToSwap.Any(x => x != playerInfoCache.Player);
    }

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        Timing.RunCoroutine(GoingToSwapCoroutine.Coroutine(playerInfoCache.Player, 5));
    }
}