using Exiled.API.Enums;
using Exiled.API.Features;
using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(StartWarhead))]
public class StartWarhead : BaseCoinEffect, ICoinEffectDefinition
{
    public static Player? CoinActivatedWarhead = null;

    public StartWarhead()
    {
        CoinActivatedWarhead = null;
    }

    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => !Warhead.IsInProgress && !Warhead.IsDetonated;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        Warhead.Status = WarheadStatus.InProgress;
        hintLines.Add(translation.Warhead);
        CoinActivatedWarhead = playerInfoCache.Player;
    }
}