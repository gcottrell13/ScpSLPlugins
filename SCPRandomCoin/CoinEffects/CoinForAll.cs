using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using SCPRandomCoin.API;
using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(CoinForAll))]
public class CoinForAll : ICoinEffectDefinition
{
    public bool didEffect { get; private set; } = false;

    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) => !didEffect;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        didEffect = true;
        var translation = SCPRandomCoin.Singleton!.Translation;

        foreach (Player alivePlayer in Player.Get(x => x.IsAlive))
        {
            if (alivePlayer.IsScp)
            {
                alivePlayer.CurrentItem = alivePlayer.AddItem(ItemType.Coin);
                alivePlayer.ShowHint(translation.GetItem.Format("item", "Coin"), 10);
            }
            else
            {
                var pickup = Pickup.CreateAndSpawn(ItemType.Coin, alivePlayer.Position, default, alivePlayer);
                if (alivePlayer != playerInfoCache.Player)
                {
                    alivePlayer.ShowHint(translation.GetItem.Format("item", "Coin"), 10);
                }
                else
                {
                    hintLines.Add(translation.CoinForAll);
                }
            }
        }
    }
}