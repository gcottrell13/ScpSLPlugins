using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using VoiceChat;

namespace VoiceChatModifyHook.Events;

public class VoiceChatListenEvent : IExiledEvent, IPlayerEvent, IDeniableEvent
{
    public Player Player { get; }
    public Player Speaker => Player;
    public Player Listener { get; }
    public VoiceChatChannel VoiceChatChannel { get; set; }
    public bool IsAllowed { get; set; } = true;

    public VoiceChatListenEvent(Player speaker, Player listener, VoiceChatChannel voiceChatChannel)
    {
        Player = speaker;
        Listener = listener;
        VoiceChatChannel = voiceChatChannel;
    }
}
