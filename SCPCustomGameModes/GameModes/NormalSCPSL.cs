using PlayerEvent = Exiled.Events.Handlers.Player;
using CustomGameModes.GameModes.Normal;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using Exiled.API.Extensions;
using MEC;
using CustomGameModes.Configs;
using SCPStore;
using Exiled.API.Features;
using System.Collections.Generic;
using SCPStore.API;

namespace CustomGameModes.GameModes
{
    internal class NormalSCPSL : IGameMode
    {
        public string Name => "Normal";

        public string PreRoundInstructions => "";

        public const string OpenStoreInstructions = "Use ALT to buy items.";
        public const string StoreCycleInstructions = "Press ALT to cycle menu\n";
        public const string StoreCurrency = "{0} credits";

        ClassDStarterInventory ClassDStarterInventory { get; set; }
        CellGuard CellGuard { get; set; }
        GnomeoSquad GnomeoSquad { get; set; }

        NormalSCPSLConfig config => (CustomGameModes.Singleton?.Config ?? new()).Normal;

        Bank bank;

        public NormalSCPSL()
        {
            bank = new Bank();
            ClassDStarterInventory = new ClassDStarterInventory();
            CellGuard = new CellGuard();
            GnomeoSquad = new GnomeoSquad();
        }

        public void OnRoundEnd()
        {
            UnsubscribeEventHandlers();
        }

        public void OnRoundStart()
        {
            SubscribeEventHandlers();
        }

        public void OnWaitingForPlayers()
        {
            UnsubscribeEventHandlers();
        }

        void SubscribeEventHandlers()
        {
            SCP5000Handler.SubscribeStaticEventHandlers();
            ClassDStarterInventory.SubscribeEventHandlers();
            CellGuard.SubscribeEventHandlers();
            GnomeoSquad.SubscribeEventHandlers();
            new SkeletonSpawner().SubscribeEventHandlers();
            new DChildren().Setup();

            PlayerEvent.Spawned += OnSpawn;
            PlayerEvent.TogglingNoClip += OnToggleNoclip;
        }
        void UnsubscribeEventHandlers()
        {
            SCP5000Handler.UnsubscribeStaticEventHandlers();
            ClassDStarterInventory.UnsubscribeEventHandlers();
            CellGuard.UnsubscribeEventHandlers();
            GnomeoSquad.UnsubscribeEventHandlers();

            PlayerEvent.Spawned -= OnSpawn;
            PlayerEvent.TogglingNoClip -= OnToggleNoclip;
        }

        void OnSpawn(SpawnedEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Scp0492)
            {
                if (UnityEngine.Random.Range(1, 101) < config.Scp3114ZombieChance)
                {
                    Timing.CallDelayed(0.1f, () => ev.Player.ChangeAppearance(RoleTypeId.Scp3114));
                }
            }

            if (config.StoreAccessRoles.Contains(ev.Player.Role.Type) && config.Store.Count > 0)
            {
                bank.AddCredits(ev.Player, StoreCurrency, config.CreditsOnSpawn);
                ev.Player.ShowHint($"As a {ev.Player.Role.Type}\n{OpenStoreInstructions}", 20);
            }
        }

        public IEnumerator<float> OnToggleNoclip(TogglingNoClipEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;
            if (!config.StoreAccessRoles.Contains(ev.Player.Role.Type))
                yield break;

            if (config.Store.Count == 0)
                yield break;

            if (Store.TryCycleStoreIfExist(ev.Player, StoreCycleInstructions, out Store store))
                yield break;

            store.AddCredits = bank.AddCredits;
            store.GetCredits = bank.GetCredits;
            store.AddShowBalance(StoreCurrency);
            foreach (var shopItem in config.Store)
            {
                var name = shopItem.Key;
                var cost = shopItem.Value;
                store.AddStoreItem(name, cost, StoreCurrency);
            }

            store.DisplayAndCountdown($"{StoreCycleInstructions}Closing Menu In: ", 10);
        }
    }
}
