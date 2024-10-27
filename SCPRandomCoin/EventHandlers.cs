using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Warhead;
using MEC;
using System.Collections.Generic;

namespace SCPRandomCoin;

internal static class EventHandlers
{
    public static IEnumerator<float> OnChangedItem(ChangedItemEventArgs ev)
    {
        yield return Timing.WaitForSeconds(0.5f);
        while (ev.Player.CurrentItem.Type == ItemType.Coin)
        {
            if (string.IsNullOrWhiteSpace(ev.Player.CurrentHint?.Content) || ev.Player.CurrentHint?.Content.StartsWith("Round Time") == true)
            {
                var color = EffectHandler.IsKeterTime() ? "red" :
                    EffectHandler.IsEuclidTime() ? "yellow" :
                    "white";
                ev.Player.ShowHint($"Round Time: <color={color}>{Round.ElapsedTime:mm\\:ss}</color>", 2);
            }
            yield return Timing.WaitForSeconds(1);
        }
    }

    public static IEnumerator<float> OnCoinFlip(FlippingCoinEventArgs ev)
    {
        yield return Timing.WaitForSeconds(2);
        EffectHandler.OnCoinFlip(ev.Player, ev.IsTails);
    }


    public static void OnRoundStarted()
    {
        EffectHandler.Reset();
        EffectHandler.SpawnExtraCoins();
    }
}
