using Exiled.API.Interfaces;
using System.ComponentModel;

namespace SCPTeleporter.Configs;

internal class Config : IConfig
{
    public bool IsEnabled { get; set; }
    public bool Debug { get; set; }

    [Description("How many charges the store TP gets")]
    public int StoreItemCharges { get; set; } = 2;

    [Description("What name the item should use in the store")]
    public string StoreItemName { get; set; } = "SCPTeleporter.Teleporter";
}
