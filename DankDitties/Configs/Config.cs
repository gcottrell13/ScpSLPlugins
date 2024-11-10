using Exiled.API.Interfaces;

namespace DankDitties.Configs;

internal class Config : IConfig
{
    public bool IsEnabled { get; set; }
    public bool Debug { get; set; }
    public string FilePath { get; set; } = "/dank-ditties";
}
