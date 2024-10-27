using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using SCPRandomCoin.CoinEffects;
using SCPRandomCoin.API;

namespace SCPRandomCoin.Configs;

internal class Config : IConfig
{
    public bool IsEnabled { get; set; }
    public bool Debug { get; set; }

    [Description("Spawn extra coins in the SCP item boxes")]
    public int SpawnExtraCoins { get; private set; } = 10;

    [Description("Effects to subject the coin flipper to, should they get heads. key is effect name, value is the relative weight.")]
    public Dictionary<string, float> Effects { get; private set; } = new()
    {
        { EffectNameHelper.GetEffectName<Nothing>(), 1 },
        { EffectNameHelper.GetEffectName<OneHp>(), 10 },
        { EffectNameHelper.GetEffectName<TpToScp>(), 10 },
        { EffectNameHelper.GetEffectName<BecomeScp>(), 10 },
        { EffectNameHelper.GetEffectName<GetItem>(), 10 },
        { EffectNameHelper.GetEffectName<LoseItem>(), 10 },
        { EffectNameHelper.GetEffectName<Heal>(), 10 },
        { EffectNameHelper.GetEffectName<TpToRandom>(), 10 },
        { EffectNameHelper.GetEffectName<StartWarhead>(), 1 },
    };

    [Description("Items to get")]
    public Dictionary<ItemType, float> ItemList { get; private set; } = new()
    {
        { ItemType.KeycardJanitor, 100f },
        { ItemType.KeycardScientist, 10f },
        { ItemType.KeycardResearchCoordinator, 10f },
        { ItemType.KeycardZoneManager, 10f },
        { ItemType.KeycardGuard, 10f },
        { ItemType.KeycardMTFPrivate, 10f },
        { ItemType.KeycardContainmentEngineer, 10f },
        { ItemType.KeycardMTFOperative, 10f },
        { ItemType.KeycardMTFCaptain, 10f },
        { ItemType.KeycardFacilityManager, 1f },
        { ItemType.KeycardChaosInsurgency, 10f },
        { ItemType.KeycardO5, 1f },
        { ItemType.Radio, 100f },
        { ItemType.GunCOM15, 10f },
        { ItemType.Medkit, 1f },
        { ItemType.Flashlight, 100f },
        { ItemType.MicroHID, 1f },
        { ItemType.SCP500, 1f },
        { ItemType.SCP207, 1f },
        { ItemType.Ammo12gauge, 10f },
        { ItemType.GunE11SR, 1f },
        { ItemType.GunCrossvec, 1f },
        { ItemType.Ammo556x45, 1f },
        { ItemType.GunFSP9, 1f },
        { ItemType.GunLogicer, 1f },
        { ItemType.GrenadeHE, 1f },
        { ItemType.GrenadeFlash, 100f },
        { ItemType.Ammo44cal, 1f },
        { ItemType.Ammo762x39, 1f },
        { ItemType.Ammo9x19, 1f },
        { ItemType.GunCOM18, 1f },
        { ItemType.SCP018, 1f },
        { ItemType.SCP268, 1f },
        { ItemType.Adrenaline, 1f },
        { ItemType.Painkillers, 100f },
        { ItemType.Coin, 1f },
        { ItemType.ArmorLight, 10f },
        { ItemType.ArmorCombat, 1f },
        { ItemType.ArmorHeavy, 1f },
        { ItemType.GunRevolver, 10f },
        { ItemType.GunAK, 1f },
        { ItemType.GunShotgun, 1f },
        { ItemType.SCP2176, 1f },
        { ItemType.SCP244a, 1f },
        { ItemType.SCP244b, 1f },
        { ItemType.SCP1853, 1f },
        { ItemType.ParticleDisruptor, 1f },
        { ItemType.GunCom45, 1f },
        { ItemType.SCP1576, 1f },
        { ItemType.Jailbird, 1f },
        { ItemType.AntiSCP207, 20f },
        { ItemType.GunFRMG0, 1f },
        { ItemType.GunA7, 1f },
        { ItemType.Lantern, 100f },
    };

    [Description("Should the coin have a chance to break even if it didn't do anything?")]
    public bool CoinBreakOnTails { get; private set; } = false;

    [Description("The % chance for the coin to break")]
    public int CoinBreakPercent { get; private set; } = 50;

    [Description("SCP users of a coin will break it on the first heads")]
    public bool ScpCoinBreaksImmediately { get; private set; } = true;

    [Description("How many minutes into the round 'euclid' effects should start appearing")]
    public float EuclidMinuteThreshold { get; private set; } = 4;
    [Description("Which effects should be considered 'euclid' (somewhat dangerous).\nAll effects that are not euclid or keter are 'safe'")]
    public List<string> EuclidEffects { get; private set; } = new();

    [Description("How many minutes into the round 'keter' effects should start appearing")]
    public float KeterMinuteThreshold { get; private set; } = 9;
    [Description("Which effects should be considered 'keter' (super dangerous)")]
    public List<string> KeterEffects { get; private set; } = new();

}
