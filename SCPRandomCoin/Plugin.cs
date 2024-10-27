
namespace SCPRandomCoin;

using System;
using Exiled.API.Features;
using Configs;
using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using global::SCPRandomCoin.API;
using global::SCPRandomCoin.CoinEffects;

internal class SCPRandomCoin : Plugin<Config, Translation>
{
    public static SCPRandomCoin? Singleton;

    public override void OnEnabled()
    {
        Singleton = this;
        PlayerEvent.FlippingCoin += EventHandlers.OnCoinFlip;
        PlayerEvent.ChangedItem += EventHandlers.OnChangedItem;
        ServerEvent.RoundStarted += EventHandlers.OnRoundStarted;

        RegisterAll();

        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Singleton = null;
        PlayerEvent.FlippingCoin -= EventHandlers.OnCoinFlip;
        PlayerEvent.ChangedItem -= EventHandlers.OnChangedItem;
        ServerEvent.RoundStarted -= EventHandlers.OnRoundStarted;
        base.OnDisabled();
    }

    internal void RegisterAll()
    {
        CoinEffectRegistry.TryRegisterEffect<BecomeScp>();
        CoinEffectRegistry.TryRegisterEffect<BecomeSwappable>();
        CoinEffectRegistry.TryRegisterEffect<BecomeWide>();
        CoinEffectRegistry.TryRegisterEffect<CoinForAll>();
        CoinEffectRegistry.TryRegisterEffect<DoSwap>();
        CoinEffectRegistry.TryRegisterEffect<FakeScpDeath>();
        CoinEffectRegistry.TryRegisterEffect<GetALight>();
        CoinEffectRegistry.TryRegisterEffect<GetCandy>();
        CoinEffectRegistry.TryRegisterEffect<GetItem>();
        CoinEffectRegistry.TryRegisterEffect<Heal>();
        CoinEffectRegistry.TryRegisterEffect<LookLikeScp>();
        CoinEffectRegistry.TryRegisterEffect<LoseItem>();
        CoinEffectRegistry.TryRegisterEffect<Nothing>();
        CoinEffectRegistry.TryRegisterEffect<OneHp>();
        CoinEffectRegistry.TryRegisterEffect<OneInTheChamber>();
        CoinEffectRegistry.TryRegisterEffect<PocketDimension>();
        CoinEffectRegistry.TryRegisterEffect<PrizeRoom>();
        CoinEffectRegistry.TryRegisterEffect<RandomEffect>();
        CoinEffectRegistry.TryRegisterEffect<RemoveSwappable>();
        CoinEffectRegistry.TryRegisterEffect<ReSpawnSpectators>();
        CoinEffectRegistry.TryRegisterEffect<ReversedControls>();
        CoinEffectRegistry.TryRegisterEffect<Shrink>();
        CoinEffectRegistry.TryRegisterEffect<Snapback>();
        CoinEffectRegistry.TryRegisterEffect<SpawnGrenade>();
        CoinEffectRegistry.TryRegisterEffect<StartWarhead>();
        CoinEffectRegistry.TryRegisterEffect<StopWarhead>();
        CoinEffectRegistry.TryRegisterEffect<TpToRandom>();
        CoinEffectRegistry.TryRegisterEffect<TpToScp>();
    }


    public override string Name => "RandomCoin";
    public override string Author => "GCOTTRE";
    public override Version Version => new Version(1, 0, 1);
    public override Version RequiredExiledVersion => new Version(8, 13, 1);
}
