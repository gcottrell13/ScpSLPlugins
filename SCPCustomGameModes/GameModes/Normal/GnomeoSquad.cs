
using PlayerEvent = Exiled.Events.Handlers.Player;
using MapEvent = Exiled.Events.Handlers.Map;
using ServerEvent = Exiled.Events.Handlers.Server;
using Exiled.Events.EventArgs.Map;
using System.Collections.Generic;
using MEC;
using Exiled.API.Features;
using System.Text;
using System.Linq;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;

namespace CustomGameModes.GameModes.Normal;

internal class GnomeoSquad
{
    public const string GNOMEO = "GNOMEO";
    public bool GnomeoSquadPerished = false;

    public GnomeoSquad()
    {
        
    }


    public void SubscribeEventHandlers()
    {
        MapEvent.AnnouncingNtfEntrance += AnnounceNTF;
        PlayerEvent.Died += Died;
    }

    public void UnsubscribeEventHandlers()
    {
        MapEvent.AnnouncingNtfEntrance -= AnnounceNTF;
        PlayerEvent.Died -= Died;
    }

    public IEnumerator<float> AnnounceNTF(AnnouncingNtfEntranceEventArgs ev)
    {
        if (ev.UnitName.ToUpper()[0] != 'R')
            yield break;

        ev.UnitName = GNOMEO;
        ev.IsAllowed = false;
        yield return Timing.WaitForSeconds(1);

        int num = Player.Get(x => x.IsScp && x.Role != RoleTypeId.Scp0492).Count();
        StringBuilder sb = new();

        sb.Append("MTFUNIT EPSILON 11 DESIGNATED ");
        sb.Append("NO ME O ");
        sb.Append(" HASENTERED ALLREMAINING ");
        if (num == 0)
        {
            sb.Append("NOSCPSLEFT");
        }
        else
        {
            sb.Append("AWAITINGRECONTAINMENT ");
            sb.Append(num);
            if (num == 1)
            {
                sb.Append(" SCPSUBJECT");
            }
            else
            {
                sb.Append(" SCPSUBJECTS");
            }
        }

        Cassie.MessageTranslated(sb.ToString(), "MTF Unit Epsilon 11 GNOMEO has entered the facility.");

        float targetScale = 0.5f;
        float scale = 1f;
        float timeStep = 0.5f;
        float secondsToBecomeGnome = 10f;
        while (scale > targetScale)
        {
            scale -= (1 - targetScale) / (secondsToBecomeGnome / timeStep);
            foreach (Player mtf in Player.Get(x => x.UnitName == ev.UnitName))
            {
                mtf.Scale = new(1f, scale, 1f);
            }

            yield return Timing.WaitForSeconds(timeStep);
        }

    }

    public void Died(DiedEventArgs ev)
    {
        if (ev.Player.UnitName != GNOMEO || GnomeoSquadPerished)
            return;

        GnomeoSquadPerished = true;
        if (Player.Get(x => x.UnitName == GNOMEO).Count() == 0)
        {
            Cassie.MessageTranslated("MTFUNIT EPSILON 11 DESIGNATED NO ME O is dead.", $"MTF Unit EPSILON 11 {GNOMEO} is dead.");
        }
    }
}
