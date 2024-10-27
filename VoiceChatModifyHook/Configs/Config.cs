using Exiled.API.Interfaces;

namespace VoiceChatModifyHook.Configs;

internal class Config : IConfig
{
    public bool IsEnabled { get; set; }
    public bool Debug { get; set; }
}
