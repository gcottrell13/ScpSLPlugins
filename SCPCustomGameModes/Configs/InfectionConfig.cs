
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomGameModes.Configs;

internal sealed class InfectionConfig
{
    [Description("chances of the Survivors getting a particular role. Does not have to add up to 100%")]
    public Dictionary<string, float> SurvivorScpChance { get; set; } = new()
    {
        { nameof(RoleTypeId.ClassD), 100f },
    };

    [Description("chances of the Infected getting a particular role. Does not have to add up to 100%")]
    public Dictionary<string, float> InfectedScpChance { get; set; } = new()
    {
        { nameof(RoleTypeId.Scp0492), 100f },
    };

    [Description("chances of the Escapees getting a particular role. Does not have to add up to 100%")]
    public Dictionary<string, float> EscapeeScpChance { get; set; } = new()
    {
        { nameof(RoleTypeId.ChaosConscript), 100f },
    };

    [Description("which coin effects (not defined as part of this game mode) to enable")]
    public List<string> EnableCoinEffects { get; set; } = new() { };
}
