using Exiled.API.Features;
using PlayerEvent = Exiled.Events.Handlers.Player;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Extensions;
using Exiled.API.Enums;
using System;
using Exiled.Events.EventArgs.Interfaces;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;

namespace CustomGameModes.GameModes.Normal
{
    internal class SCP1392Handler
    {
        public static List<SCP1392Handler> Instances { get; } = new();

        public static void UnsubscribeAll()
        {
            foreach (var instance in Instances)
            {
                instance.UnsubscribeEventHandlers();
            }
            Instances.Clear();
        }

        Player Owner = null;
        RoleTypeId OwnerRole;
        DateTime lastLethalEvent;

        public SCP1392Handler()
        {
        }

        ~SCP1392Handler()
        {
            UnsubscribeEventHandlers();
            Instances.Remove(this);
        }

        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------

        public void SubscribeEventHandlers()
        {
        }

        public void SubscribeOnPlayerGive()
        {
            PlayerEvent.ReceivingEffect += ReceivingEffect;
            PlayerEvent.Dying += Dying;
            PlayerEvent.FailingEscapePocketDimension += FailingEscapePocketDimension;
            PlayerEvent.Hurt += Hurt;
        }

        public void UnsubscribeEventHandlers()
        {
            PlayerEvent.ReceivingEffect -= ReceivingEffect;
            PlayerEvent.Dying -= Dying;
            PlayerEvent.FailingEscapePocketDimension -= FailingEscapePocketDimension;
            PlayerEvent.Hurt -= Hurt;
            Instances.Remove(this);
        }

        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------

        public void SetupPlayer(Player player)
        {
            if (Owner != null) return;

            Owner = player;
            OwnerRole = player.Role;
            SubscribeOnPlayerGive();
        }

        private bool CheckOwner(Player player)
        {
            if (Owner != player) return false;
            if (player.Role != OwnerRole)
            {
                UnsubscribeEventHandlers();
                Instances.Remove(this);
                return false;
            }
            return true;
        }

        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------

        private IEnumerator<float> ReceivingEffect(ReceivingEffectEventArgs ev)
        {
            if (!CheckOwner(ev.Player)) yield break;

            switch (ev.Effect.GetEffectType())
            {
                case EffectType.CardiacArrest:
                    {
                        yield return Timing.WaitForSeconds(2f);
                        Owner.DisableEffect(EffectType.CardiacArrest);
                        break;
                    }
            }
        }

        private void Dying(DyingEventArgs ev)
        {
            if (!CheckOwner(ev.Player)) return;

            if (DateTime.Now - lastLethalEvent > TimeSpan.FromSeconds(10))
            {
                ev.IsAllowed = false;
                ev.Player.PlayBeepSound();
                ev.Player.ArtificialHealth = 100;
                ev.Player.Health = 1;
                lastLethalEvent = DateTime.Now;
            }
            else
            {
                UnsubscribeEventHandlers();
            }
        }

        private void FailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev)
        {
            if (!CheckOwner(ev.Player)) return;
            ev.IsAllowed = false;
            ev.Player.Position = Player.Get(Team.SCPs).Where(scp => scp.Role != RoleTypeId.Scp079).GetRandomValue().Position;
        }

        private IEnumerator<float> Hurt(HurtEventArgs ev)
        {
            if (!CheckOwner(ev.Player)) yield break;
            yield return Timing.WaitForSeconds(3);
            Owner.Heal(ev.Amount * 0.5f);
        }
    }
}
