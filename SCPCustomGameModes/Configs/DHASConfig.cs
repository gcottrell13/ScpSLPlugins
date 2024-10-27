
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomGameModes.Configs;

internal sealed class DHASConfig
{
    [Description("chances of the SCP getting a particular role. Does not have to add up to 100%")]
    public Dictionary<string, float> DhasScpChance { get; set; } = new()
    {
        { nameof(RoleTypeId.Scp939), 100f },
    };

    [Description("which coin effects (not defined as part of this game mode) to enable")]
    public List<string> EnableCoinEffects { get; set; } = new() { };
}
