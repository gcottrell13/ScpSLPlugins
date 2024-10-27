using Exiled.API.Enums;
using Exiled.API.Extensions;
using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(RandomEffect))]
public class RandomEffect : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => true;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var ef = Effects.GetRandomValue();
        playerInfoCache.Player.EnableEffect(ef.Key, 10, ef.Value, false);
        hintLines.Add(translation.GetItem.Format("item", ef.Key));
    }

    public static readonly Dictionary<EffectType, float> Effects = new()
    {
        { EffectType.Blinded, 60 },
        { EffectType.Flashed, 5 },
        { EffectType.SinkHole, 10 },
        { EffectType.Stained, 10 },

        { EffectType.Invigorated, 60 },
        { EffectType.Invisible, 999 },
        { EffectType.Scp207, 60 },
        { EffectType.Vitality, 60 },
    };
}