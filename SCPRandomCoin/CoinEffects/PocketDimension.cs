using Exiled.API.Enums;
using SCPRandomCoin.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(PocketDimension))]
public class PocketDimension : ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => true;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        playerInfoCache.Player.EnableEffect(EffectType.PocketCorroding, 1, 1);
    }
}