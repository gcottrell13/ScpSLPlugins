using System;
using Exiled.API.Features;
using HarmonyLib;

namespace VoiceChatModifyHook;

using Configs;
using Config = global::VoiceChatModifyHook.Configs.Config;

internal class CustomGameModes : Plugin<Config, Translation>
{
    public static CustomGameModes? Singleton;
    private Harmony? _harmony;

    public override void OnEnabled()
    {
        Singleton = this;

        _harmony = new Harmony($"gcottre-vch-{DateTime.Now.Ticks}");
        _harmony.PatchAll();

        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Singleton = null;
        _harmony?.UnpatchAll();
        base.OnDisabled();
    }


    public override string Name => "VoiceChatModifyHook";
    public override string Author => "GCOTTRE";
    public override Version Version => new Version(1, 0, 0);
    public override Version RequiredExiledVersion => new Version(8, 13, 1);
}
