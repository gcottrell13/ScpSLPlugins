using Exiled.API.Features;
using PlayerRoles;
using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(ReSpawnSpectators))]
public class ReSpawnSpectators : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) =>
        Round.IsStarted && Player.Get(RoleTypeId.Spectator).Any();

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var player = playerInfoCache.Player;
        var spectators = Player.Get(RoleTypeId.Spectator).Take(5).ToList();
        foreach (Player spectator in spectators)
        {
            spectator.Role.Set(player.Role.Type);
            spectator.Position = player.Position;
            spectator.ShowHint(translation.Respawned.Format("count", spectators.Count), 25);
        }
        hintLines.Add(translation.Respawn);
    }
}