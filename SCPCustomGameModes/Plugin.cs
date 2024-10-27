using Exiled.Events.Commands.Reload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes
{
    using System;
    using Exiled.API.Features;
    using HarmonyLib;
    using Configs;
    using Config = global::CustomGameModes.Configs.Config;

    internal class CustomGameModes : Plugin<Config, Translation>
    {
        public static CustomGameModes? Singleton;
        private Harmony? _harmony;

        EventHandlers? handlers;

        public override void OnEnabled()
        {
            Singleton = this;
            handlers = new EventHandlers();
            handlers.RegisterEvents();

            _harmony = new Harmony($"gcottre-cgm-{DateTime.Now.Ticks}");
            _harmony.PatchAll();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Singleton = null;
            handlers?.UnregisterEvents();
            _harmony?.UnpatchAll();
            base.OnDisabled();
        }


        public override string Name => "CustomGameModes";
        public override string Author => "GCOTTRE";
        public override Version Version => new Version(1, 0, 4);
        public override Version RequiredExiledVersion => new Version(8, 13, 1);
    }
}
