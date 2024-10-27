using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using LightContainmentZoneDecontamination;
using PlayerRoles;
using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(TpToRandom))]
public class TpToRandom : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => 
        playerInfoCache.OngoingEffect == null && getPlace() != null;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        if (getPlace() is Vector3 pos)
        {
            playerInfoCache.Player.Position = pos;
            hintLines.Add(translation.Tp);
        }
    }

    public Vector3? getPlace()
    {
        var roles = new List<RoleTypeId>();
        if (!Map.IsLczDecontaminated)
        {
            roles.Add(RoleTypeId.Scientist);
        }
        if (!Warhead.IsDetonated && !Warhead.IsInProgress)
        {
            roles.Add(RoleTypeId.FacilityGuard);
            roles.Add(RoleTypeId.Scp049);
            roles.Add(RoleTypeId.Scp173);
            roles.Add(RoleTypeId.Scp939);
            roles.Add(RoleTypeId.Scp096);
        }
        var role = roles.GetRandomValue();
        if (role == default)
        {
            return null;
        }
        return role.GetRandomSpawnLocation().Position;
    }
}