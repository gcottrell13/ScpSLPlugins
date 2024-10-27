using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using SCPRandomCoin.Commands;
using System.Collections.Generic;
using System.Linq;

namespace SCPRandomCoin.API;

public static class GoingToSwapCoroutine
{
    public static HashSet<Player> ReadyToSwap = new();
    public static HashSet<Player> GoingToSwap = new();

    public static void Reset()
    {
        ReadyToSwap.Clear();
        GoingToSwap.Clear();
    }

    public static IEnumerator<float> Coroutine(Player player, int waitSeconds)
    {
        GoingToSwap.Add(player);
        for (int i = 0; i < waitSeconds; i++)
        {
            if (GoingToSwap.Contains(player) == false)
            {
                // the StableCommand could remove the player from this list.
                player.ShowHint("");
                yield break;
            }
            if (!ReadyToSwap.Any(x => x != player))
            {
                break;
            }

            player.ShowHint(SCPRandomCoin.Singleton?.Translation.GoingToSwap.Format(new()
            {
                { "time", waitSeconds - i },
                { "command", StableCommand.ShortAlias },
            }));

            yield return Timing.WaitForSeconds(1);
        }

        GoingToSwap.Remove(player);
        var target = ReadyToSwap.Where(x => x != player).GetRandomValue();
        if (target == null)
        {
            player.ShowHint(SCPRandomCoin.Singleton?.Translation.CancelSwap);
            yield break;
        }

        player.ShowHint("");
        ReadyToSwap.Remove(target);
        var p = new PlayerState(player);
        var t = new PlayerState(target);
        p.Apply(target);
        t.Apply(player);
    }
}
