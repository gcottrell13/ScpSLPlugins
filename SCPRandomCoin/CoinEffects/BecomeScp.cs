using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using SCPRandomCoin.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(BecomeScp))]
public class BecomeScp : ICoinEffectDefinition
{
    public int turnedScps = 0;

    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) =>
        playerInfoCache.OngoingEffect == null && !playerInfoCache.IsScp && turnedScps < 2;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var roles = new List<RoleTypeId> { RoleTypeId.Scp049, RoleTypeId.Scp096, RoleTypeId.Scp3114, RoleTypeId.Scp106, RoleTypeId.Scp939, RoleTypeId.Scp173 };
        if (Player.Get(Team.SCPs).Count() > 0)
            roles.Add(RoleTypeId.Scp079);

        var scp = roles.GetRandomValue();
        playerInfoCache.Player.Role.Set(scp, RoleSpawnFlags.AssignInventory);
        turnedScps++;
    }
}