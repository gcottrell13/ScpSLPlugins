using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(Heal))]
public class Heal : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => playerInfoCache.Player.Health < playerInfoCache.Player.MaxHealth;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var player = playerInfoCache.Player;
        player.Heal(Math.Max(player.MaxHealth / 2, 100));
        player.DisableAllEffects();
        hintLines.Add(translation.Heal);
    }
}