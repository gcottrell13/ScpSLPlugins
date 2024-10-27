using Exiled.API.Features;

namespace SCPRandomCoin.API;

public struct PlayerInfoCache
{
    public Player Player { get; }
    public bool IsScp;
    public ICoinEffectDefinition? OngoingEffect;
    public float Health;
    public float MaxHealth;
    public int ItemCount;
    public Room CurrentRoom;
    public Lift? Lift;

    public PlayerInfoCache(Player player)
    {
        Player = player;
        Lift = player.Lift;
        CurrentRoom = player.CurrentRoom;
        ItemCount = player.Items.Count;
        Health = player.Health;
        MaxHealth = player.MaxHealth;
        EffectHandler.HasOngoingEffect.TryGetValue(player, out OngoingEffect);
        IsScp = player.IsScp;
    }
}
