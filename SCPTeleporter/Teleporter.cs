using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using System;
using UnityEngine;
using LightToy = Exiled.API.Features.Toys.Light;
using PrimitiveToy = Exiled.API.Features.Toys.Primitive;

namespace SCPTeleporter;

public enum TeleporterType
{
    /// <summary>
    /// once created, will not change team affiliation with the owner.
    /// </summary>
    Static = 0,

    /// <summary>
    /// will change team affiliation with the owner. Any non-coroporeal roles become "Any" team affiliated
    /// </summary>
    Loyal = 1,
}

public class Teleporter
{
    const float spinPerSecond = 360f;
    public int Id { get; private set; }
    public float Cooldown { get; }
    public DateTime TimeLastUsed { get; private set; } = DateTime.MaxValue;

    public bool Active = false;

    Vector3 logicalPosition;

    PrimitiveToy Base;
    LightToy Light;
    PrimitiveToy SphereEffect;

    Player? Owner;

    TeleporterType _type;


    public TeleporterType Type { get => _type; set
        {
            _type = value;
        }
    }

    public Teleporter(int id, Player owner, float cooldown = 5f, TeleporterType type = TeleporterType.Static) : this(id, cooldown, type) 
    {
        Owner = owner;
    }

    public Teleporter(int id, float cooldown = 5f, TeleporterType type = TeleporterType.Static)
    {
        Id = id;
        Cooldown = cooldown;
        Base = PrimitiveToy.Create(PrimitiveType.Cube, scale: new Vector3(0.1f, 0.1f, 1));
        Base.MovementSmoothing = 20;
        Base.Collidable = false;

        SphereEffect = PrimitiveToy.Create(PrimitiveType.Sphere, scale: new Vector3(1, 0.2f, 1));
        SphereEffect.Color = new Color(0f, 0f, 1f, 0.2f);
        SphereEffect.Collidable = false;

        //SphereEffect.AdminToyBase.transform.parent = Base.AdminToyBase.transform;
        SphereEffect.MovementSmoothing = 20;

        Light = LightToy.Create(position: new Vector3(0, 0.5f, 0));
        Light.AdminToyBase.transform.parent = Base.AdminToyBase.transform;
        Light.MovementSmoothing = 20;

        Light.Color = Color.blue;
        _type = type;
    }

    public Vector3 Position
    {
        get { return logicalPosition; }
        set { Base.Position = value; logicalPosition = value; }
    }

    public void SetUsed()
    {
        TimeLastUsed = DateTime.Now;
        Active = false;
    }

    public void Update(float dt)
    {
        double seconds = (double)DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000f;
        if ((DateTime.Now - TimeLastUsed).TotalSeconds > Cooldown)
        {
            Active = true;
            SphereEffect.Position = new Vector3(0, (float)Math.Sin(seconds), 0) * 0.5f + Base.Position + Vector3.up * 0.6f;
            SphereEffect.Visible = true;

            Base.Rotation *= Quaternion.AngleAxis(dt * spinPerSecond, Vector3.up);
        }
        else
        {
            SphereEffect.Visible = false;
            Active = false;
            Base.Rotation *= Quaternion.AngleAxis(spinPerSecond * dt * ((float)(DateTime.Now - TimeLastUsed).TotalSeconds / Cooldown), Vector3.up);
        }

        if (Owner is not null && Type == TeleporterType.Loyal)
        {
            Light.Color = Owner.Role.Color;
            SphereEffect.Color = Owner.Role.Color;
        }
    }

    public bool CanBeUsedBy(Player player)
    {
        if (Owner is not null && Owner.IsAlive && Type == TeleporterType.Loyal) return player.LeadingTeam == Owner.LeadingTeam;
        return true;
    }


    public void Destroy()
    {
        Light.Destroy();
        SphereEffect.Destroy();
        Base.Destroy();
    }

}
