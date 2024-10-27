using Exiled.API.Features.Items;
using SCPStore;

namespace SCPTeleporter;

public class TeleporterStoreItem : StoreItem
{
    public static string Name => SCPTeleporter.Singleton?.Config.StoreItemName ?? $"{typeof(Teleporter).Namespace}.{nameof(Teleporter)}";

    public TeleporterStoreItem() : base(Name, "Teleporter", ItemType.Medkit)
    {
    }

    public override void OnBuy(Item item)
    {
        EventHandlers.TeleporterPlacers[item] = SCPTeleporter.Singleton?.Config.StoreItemCharges ?? 2;
    }
}
