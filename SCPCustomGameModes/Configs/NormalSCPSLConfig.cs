using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomGameModes.Configs;

public sealed class NormalSCPSLConfig
{

    [Description("% chance of resurrecting a SCP-049-2 to look like SCP-3114 instead of the normal zombie")]
    public int Scp3114ZombieChance { get; set; } = 25;

    [Description("% chance of upgrading player with SCP 5000")]
    public int Scp5000Chance { get; set; } = 5;

    [Description("If set, play this CASSIE message to the recipient of SCP 5000")]
    public string Scp5000CassieIntro { get; set; } = "pitch_1.50 welcome new user to s c p 5 thousand . others cannot see you until you .G7";

    [Description("% chance of D-Children")]
    public int DChildrenChance { get; set; } = 10;

    [Description("How many credits the store-roles should get on spawn")]
    public int CreditsOnSpawn { get; set; } = 10;

    [Description("Roles that can access the store")]
    public List<RoleTypeId> StoreAccessRoles { get; set; } = new()
    {
        RoleTypeId.ChaosConscript,
        RoleTypeId.ChaosMarauder,
        RoleTypeId.ChaosRepressor,
        RoleTypeId.ChaosRifleman,
        RoleTypeId.NtfCaptain,
        RoleTypeId.NtfPrivate,
        RoleTypeId.NtfSergeant,
        RoleTypeId.NtfSpecialist,
    };

    [Description("Store items")]
    public Dictionary<string, int> Store { get; set; } = new()
    {
            { nameof(ItemType.SCP207), 2 },
            { nameof(ItemType.GrenadeFlash), 1 },
            { nameof(ItemType.Medkit), 1 },
    };
}
