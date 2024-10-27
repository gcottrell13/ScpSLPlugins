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

[RandomCoinEffect(nameof(FakeScpDeath))]
public class FakeScpDeath : BaseCoinEffect, ICoinEffectDefinition
{
    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) =>
        Round.IsStarted && Player.Get(Team.SCPs).Any(x => x.Role.Type != RoleTypeId.Scp0492);

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        var scp = Player.Get(x => x.IsScp && x.Role.Type != RoleTypeId.Scp0492).GetRandomValue();
        NineTailedFoxAnnouncer.ConvertSCP(scp.Role.Type, out string withoutSpace, out string withSpace);

        var announcement = $"contained successfully by Automatic Security System";
        var announcementSubtitle = $"contained successfully by Automatic Security System";
        Cassie.MessageTranslated(
            message: $"SCP {withSpace} {announcement}",
            translation: $"SCP-{withoutSpace} {announcementSubtitle}."
        );

        hintLines.Add(translation.FakeScpDeath.Format("scp", $"SCP-{withoutSpace}"));
    }
}