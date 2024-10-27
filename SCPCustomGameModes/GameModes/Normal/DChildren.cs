using Exiled.API.Features;
using PlayerRoles;

namespace CustomGameModes.GameModes.Normal;

internal class DChildren
{
    public DChildren()
    {
        
    }

    public void Setup()
    {
        if (UnityEngine.Random.Range(0, 101) > CustomGameModes.Singleton.Config.Normal.DChildrenChance)
            return;

        foreach (var dclass in Player.Get(RoleTypeId.ClassD))
        {
            dclass.Scale = new UnityEngine.Vector3(0.7f, 0.7f, 0.7f);
            dclass.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 25, 9999f, true);
        }
    }
}
