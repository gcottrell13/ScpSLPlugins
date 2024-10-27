using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using SCPRandomCoin.API;
using System.Collections.Generic;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(OneInTheChamber))]
public class OneInTheChamber : BaseCoinEffect, ICoinEffectDefinition
{
    public class PlayerRecord
    {
        public Player Player;
        public Firearm Weapon;
        public int ShotsHit;
        public bool Playing;

        public PlayerRecord(Player player, Firearm weapon)
        {
            Player = player;
            Weapon = weapon;
        }

        public IEnumerator<float> OnShot(ShotEventArgs ev)
        {
            if (ev.Player != Player)
                yield break;

            if (ev.Target == null)
            {
                Playing = false;
            }
            else if (ev.Target.Role.Team != ev.Player.Role.Team)
            {
                ShotsHit++;
                ShowHint();
                yield return Timing.WaitForSeconds(0.5f);
                Weapon.Ammo = 1;
            }
            else
            {
                yield return Timing.WaitForSeconds(0.5f);
                Weapon.Ammo = 1;
            }
        }
        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != Player)
                return;
            ev.Amount = 150;
        }

        public void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (ev.Player != Player)
                return;
            ev.IsAllowed = false;
        }
        public void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (ev.Player != Player)
                return;
            ev.IsAllowed = false;
        }
        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Player != Player)
                return;
            ev.IsAllowed = false;
        }

        public void ShowHint()
        {
            var comboMeter = ShotsHit switch
            {
                0 => "",
                _ => $"<size={ShotsHit * 3 + 20}><color=yellow>x{ShotsHit}</color></size>",
            };

            var translation = SCPRandomCoin.Singleton?.Translation.OneInTheChamber ?? "One In The Chamber";
            Player.ShowHint($"{comboMeter}\n{translation}");
        }
    }

    public static Dictionary<Player, PlayerRecord> OitcPlayers = new();

    public IEnumerator<float> Coroutine(Player player)
    {
        if (OitcPlayers.ContainsKey(player))
            yield break;

        EffectHandler.HasOngoingEffect[player] = this;

        for (var i = 0; i < 3; i++)
        {
            player.ShowHint($"{translation.OneInTheChamber}\nGet Ready! In {3 - i}", 1);
            yield return Timing.WaitForSeconds(1);
        }
        player.ShowHint("<size=50>Start!</size>", 1);
        yield return Timing.WaitForSeconds(1);

        var weapon = (Firearm)player.AddItem(ItemType.GunRevolver);
        weapon.Ammo = 1;
        player.CurrentItem = weapon;

        var info = new PlayerRecord(player, weapon)
        {
            ShotsHit = 0,
            Playing = true,
        };
        OitcPlayers[player] = info;

        PlayerEvent.Shot += info.OnShot;
        PlayerEvent.Hurting += info.OnHurting;
        PlayerEvent.ChangingItem += info.OnChangingItem;
        PlayerEvent.SearchingPickup += info.OnSearchingPickup;
        PlayerEvent.DroppingItem += info.OnDroppingItem;

        while (player.IsAlive && info.Playing && !Round.IsEnded && player.CurrentItem == weapon)
        {
            info.ShowHint();
            yield return Timing.WaitForSeconds(1f);
        }

        if (!Round.IsEnded)
        {
            player.RemoveItem(weapon);
            var hint = translation.OneInTheChamberFinish.Format("count", info.ShotsHit);
            hint ??= $"{info.ShotsHit} hits";
            player.ShowHint(hint);

            EffectHandler.HasOngoingEffect.Remove(player);
        }

        OitcPlayers.Remove(player);
        PlayerEvent.Shot -= info.OnShot;
        PlayerEvent.Hurting -= info.OnHurting;
        PlayerEvent.ChangingItem -= info.OnChangingItem;
        PlayerEvent.SearchingPickup -= info.OnSearchingPickup;
        PlayerEvent.DroppingItem -= info.OnDroppingItem;
    }

    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) =>
        playerInfoCache.OngoingEffect == null && !playerInfoCache.IsScp;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        Timing.RunCoroutine(Coroutine(playerInfoCache.Player));
        hintLines.Add(translation.OneInTheChamber);
    }
}
