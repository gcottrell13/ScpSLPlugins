using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using MEC;
using PlayerRoles;
using SCPRandomCoin.API;
using System.Collections.Generic;
using UnityEngine;

namespace SCPRandomCoin.CoinEffects;

[RandomCoinEffect(nameof(PrizeRoom))]
public class PrizeRoom : BaseCoinEffect, ICoinEffectDefinition
{
    public static readonly List<List<ItemType>> PrizeItems = new()
    {
        new()
        {
            ItemType.Medkit,
            ItemType.Medkit,
            ItemType.Adrenaline,
            ItemType.Adrenaline,
            ItemType.Painkillers,
            ItemType.Painkillers,
            ItemType.Painkillers,
            ItemType.None,
        },
        new()
        {
            ItemType.KeycardJanitor,
            ItemType.KeycardJanitor,
            ItemType.KeycardJanitor,
            ItemType.Medkit,
            ItemType.KeycardJanitor,
            ItemType.KeycardJanitor,
            ItemType.None,
        },
    };

    public IEnumerator<float> Coroutine(Player player, int waitSeconds)
    {
        var oldPos = player.Position;
        var newPos = RoleTypeId.Tutorial.GetRandomSpawnLocation().Position;
        player.Position = newPos;
        EffectHandler.HasOngoingEffect[player] = this;
        var spawnedItems = new List<Pickup>();
        var itemTypes = PrizeItems.GetRandomValue();
        itemTypes.ShuffleList();

        for (int i = 0; i < itemTypes.Count; i++)
        {
            var type = itemTypes[i];
            if (type == ItemType.None && SCPRandomCoin.Singleton != null)
                type = SCPRandomCoin.Singleton.Config.ItemList.GetRandomKeyByWeight();

            spawnedItems.Add(Pickup.CreateAndSpawn(
                type,
                newPos + (Quaternion.Euler(0, i * 360f / itemTypes.Count, 0) * Vector3.forward) + Vector3.up * 0.5f,
                Quaternion.Euler(UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180))
             ));
        }
        for (int i = 0; i < waitSeconds; i++)
        {
            player.ShowHint(translation.PrizeRoom.Format("time", waitSeconds - i), 1.1f);
            yield return Timing.WaitForSeconds(1f);
        }
        EffectHandler.HasOngoingEffect.Remove(player);
        player.Position = oldPos;
        foreach (var item in spawnedItems)
            if (item.IsSpawned) item.Destroy();
    }

    public bool CanHaveEffect(PlayerInfoCache playerInfoCache) =>
        playerInfoCache.OngoingEffect == null && Round.IsStarted && playerInfoCache.Lift == null && !playerInfoCache.IsScp;

    public void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines)
    {
        Timing.RunCoroutine(Coroutine(playerInfoCache.Player, 15));
    }
}
