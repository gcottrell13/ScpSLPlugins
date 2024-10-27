using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCPRandomCoin.API;

internal class PlayerState
{
    public RoleTypeId Role;
    public float Health;
    public Item[] Inventory;
    public Vector3 Position;
    public Quaternion Rotation;
    public (EffectType effect, float duration)[] Effects;
    public Dictionary<ItemType, ushort> Ammo;

    public Lift? Elevator;
    public Vector3 ElevatorDelta;

    public PlayerState(Player player)
    {
        Ammo = player.Ammo;
        Role = player.Role;
        Health = player.Health;
        Inventory = player.Items.Select(x => x.Clone()).ToArray(); // this might not perfect
        Position = player.Position;
        Effects = player.ActiveEffects.Select(x => (x.GetEffectType(), x.TimeLeft)).ToArray();
        Rotation = player.Rotation;

        Elevator = Lift.Get(player.Position);
        if (Elevator != null)
        {
            ElevatorDelta = player.Position - Elevator.Position;
        }
    }

    public string? Apply(Player player, bool failIfWarhead = true)
    {
        if (failIfWarhead && Warhead.IsDetonated && AlphaWarheadController.CanBeDetonated(Position))
        {
            return "The Warhead was Detonated";
        }

        if (Elevator != null)
        {
            Position = Elevator.Position + ElevatorDelta;
        }

        if (player.Role != Role)
            player.Role.Set(Role, RoleSpawnFlags.None);
        player.Health = Health;
        player.ClearInventory();
        player.Rotation = Rotation;
        player.Position = Position;
        foreach (var ammo in Ammo)
        {
            player.SetAmmo(ammo.Key.GetAmmoType(), ammo.Value);
        }
        foreach (var item in Inventory)
        {
            player.AddItem(item);
        }
        foreach (var f in Effects)
        {
            player.EnableEffect(f.effect, f.duration);
        }

        return null;
    }
}
