using System.Collections.Generic;
using System.ComponentModel;

namespace CustomGameModes.Configs;

internal sealed class TTTConfig
{
    [Description("TTT Max Karma")]
    public int TttMaxKarma { get; set; } = 1500;

    [Description("TTT Credit Store")]
    public Dictionary<string, int> TttStore { get; set; } = new()
        {
            { nameof(ItemType.SCP207), 2 },
            { nameof(ItemType.GrenadeFlash), 1 },
            { nameof(ItemType.Medkit), 1 },
        };

    [Description("TTT Credit for killing Innocent")]
    public int TttKillInnocentReward { get; set; } = 1;

    [Description("TTT Credit to detective for CI dying")]
    public int TttCiDyingReward { get; set; } = 1;

    [Description("TTT Traitor starting credits")]
    public int TttTraitorStartCredits { get; set; } = 1;

    [Description("TTT Detective starting credits")]
    public int TttDetectiveStartCredits { get; set; } = 1;

    [Description("which coin effects (not defined as part of this game mode) to enable")]
    public List<string> EnableCoinEffects { get; set; } = new() { };
}
