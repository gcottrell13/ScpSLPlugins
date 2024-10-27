using Exiled.API.Enums;
using Exiled.API.Features;
using SCPRandomCoin.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(StopWarhead))]
public class StopWarhead : ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => StartWarhead.CoinActivatedWarhead != null && Warhead.IsInProgress;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        Warhead.Status = WarheadStatus.NotArmed;
    }
}