
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomGameModes.Configs;

internal sealed class SCP5000TestConfigs
{
    [Description("which coin effects (not defined as part of this game mode) to enable")]
    public List<string> EnableCoinEffects { get; set; } = new() { };
}
