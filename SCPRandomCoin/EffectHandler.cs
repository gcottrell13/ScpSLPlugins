using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using PlayerRoles;
using SCPRandomCoin.API;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCPRandomCoin;

public class EffectHandler
{
    public static Dictionary<Player, ICoinEffectDefinition> HasOngoingEffect = new();
    internal static IReadOnlyDictionary<string, ICoinEffectDefinition> EffectsThisRound = CoinEffectRegistry.GetEffectDefinitions();

    public static void Reset()
    {
        HasOngoingEffect.Clear();

        EffectsThisRound = CoinEffectRegistry.GetEffectDefinitions();
    }

    public static void SpawnExtraCoins()
    {
        if (SCPRandomCoin.Singleton == null)
            return;

        var config = SCPRandomCoin.Singleton.Config;
        if (config.SpawnExtraCoins <= 0)
            return;

        var scpItems = Pickup.List.Where(x => x.Type.IsScp()).Take(config.SpawnExtraCoins).ToList();
        foreach (var item in scpItems)
        {
            var delta = Random.Range(-1f, 1f) * Vector3.left * 0.1f
                + Random.Range(-1f, 1f) * Vector3.forward * 0.1f;
            Pickup.CreateAndSpawn(ItemType.Coin, item.Position + delta, default, null);
        }
    }

    public static void OnCoinFlip(Player player, bool isTails)
    {
        if (SCPRandomCoin.Singleton == null) return;

        var translation = SCPRandomCoin.Singleton.Translation;
        var config = SCPRandomCoin.Singleton.Config;

        var doesBreak = Random.Range(1, 101) < config.CoinBreakPercent || (config.ScpCoinBreaksImmediately && player.Role.Team == Team.SCPs);

        if (isTails)
        {
            if (doesBreak && config.CoinBreakOnTails)
            {
                coinBreak(player, player.CurrentItem);
                player.ShowHint(coinBreak(player, player.CurrentItem), 5);
            }
            return;
        }

        var safeEffects = EffectsThisRound.Keys.ToHashSet().Except(config.KeterEffects.Union(config.EuclidEffects)).ToList();

        var allowedEffectsBasedOnTime = IsKeterTime() ? safeEffects.Union(config.KeterEffects).Union(config.EuclidEffects).ToList()
            : IsEuclidTime() ? safeEffects.Union(config.EuclidEffects)
            : safeEffects;

        var effects = config.Effects
            .Where(kvp => EffectsThisRound.ContainsKey(kvp.Key) && allowedEffectsBasedOnTime.Contains(kvp.Key) && !CoinEffectRegistry.disabledEffects.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var infoCache = new PlayerInfoCache(player);
        var effectName = effects.GetRandomKeyByWeight(x => EffectsThisRound[x].CanHaveEffect(infoCache));

        if (effectName == null)
        {
            Log.Debug($"Player '{player.DisplayNickname}' got heads, but no coin effect was chosen");
            return;
        }

        Log.Debug($"Player '{player.DisplayNickname}' got {effectName}");
        var hint = _doEffect(EffectsThisRound[effectName], infoCache);

        if (doesBreak)
        {
            hint += "\n" + coinBreak(player, player.CurrentItem);
        }
        ShowCoinEffectHint(player, hint);
    }

    public static void ForceCoinEffect(Player player, string effect)
    {
        var infoCache = new PlayerInfoCache(player);
        var hint = _doEffect(EffectsThisRound[effect], infoCache);
        ShowCoinEffectHint(player, hint);
    }

    private static void ShowCoinEffectHint(Player player, string hint)
    {
        if (!string.IsNullOrWhiteSpace(hint))
        {
            player.ShowHint(hint, 5 * (hint.Where(x => x == '\n').Count() + 1));
        }
    }


    private static string coinBreak(Player player, Item coin)
    {
        if (player.HasItem(coin))
        {
            player.RemoveItem(coin);
            return SCPRandomCoin.Singleton?.Translation.CoinBreak ?? "";
        }
        return "";
    }

    private static string _doEffect(ICoinEffectDefinition effect, PlayerInfoCache player)
    {
        if (SCPRandomCoin.Singleton == null) return "";

        var hintLines = new List<string>();

        effect.DoEffect(player, hintLines);

        return string.Join("\n", hintLines).Format("name", player.Player.DisplayNickname);
    }

    public static bool IsEuclidTime()
    {
        if (SCPRandomCoin.Singleton == null) return false;
        var config = SCPRandomCoin.Singleton.Config;
        return Round.ElapsedTime.TotalMinutes >= config.EuclidMinuteThreshold;
    }

    public static bool IsKeterTime()
    {
        if (SCPRandomCoin.Singleton == null) return false;
        var config = SCPRandomCoin.Singleton.Config;
        return Round.ElapsedTime.TotalMinutes >= config.KeterMinuteThreshold;
    }
}
