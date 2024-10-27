using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(TpToScp))]
public class TpToScp : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) =>
        playerInfoCache.OngoingEffect == null && !playerInfoCache.IsScp && TpToScpSelector(playerInfoCache.Player);

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var scp = Player.Get(TpToScpSelector).GetRandomValue();
        playerInfoCache.Player.Position = scp.Position;
        hintLines.Add(translation.Tp);
    }

    private static bool TpToScpSelector(Player player)
        => player.IsScp && player.Role != RoleTypeId.Scp079;
}