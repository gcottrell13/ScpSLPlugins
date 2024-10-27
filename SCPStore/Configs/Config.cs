using Exiled.API.Interfaces;
using System.ComponentModel;

namespace SCPStore.Configs;

internal class Config : IConfig
{
    public bool IsEnabled { get; set; }
    public bool Debug { get; set; }

    [Description("verrrry brief instructions on accessing the store")]
    public string StoreAccessMethod { get; set; } = "Press ALT";
}
