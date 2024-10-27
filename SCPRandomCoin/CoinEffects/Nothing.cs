using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(Nothing))]
public class Nothing : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => true;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        hintLines.Add(translation.Nothing);
    }
}
