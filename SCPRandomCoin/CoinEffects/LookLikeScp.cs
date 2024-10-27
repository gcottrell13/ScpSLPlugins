using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System.Collections.Generic;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(LookLikeScp))]
public class LookLikeScp : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) =>
        Round.IsStarted && playerInfoCache.OngoingEffect == null && !playerInfoCache.IsScp;

    public IEnumerator<float> Coroutine(Player player, int waitSeconds)
    {
        var scp = new[] {
            RoleTypeId.Scp049,
            RoleTypeId.Scp096,
            RoleTypeId.Scp3114,
            RoleTypeId.Scp106,
            RoleTypeId.Scp939,
            RoleTypeId.Scp173,
        }.GetRandomValue();
        player.ChangeAppearance(scp);
        EffectHandler.HasOngoingEffect[player] = this;
        yield return Timing.WaitForSeconds(waitSeconds);
        EffectHandler.HasOngoingEffect.Remove(player);
        if (player.IsAlive)
            player.ChangeAppearance(player.Role);
    }

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        Timing.RunCoroutine(Coroutine(playerInfoCache.Player, 60));
        hintLines.Add(translation.FeelFunny);
    }
}
