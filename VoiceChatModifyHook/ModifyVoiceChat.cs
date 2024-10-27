using Exiled.API.Features;
using VoiceChat;
using VoiceChatModifyHook.Events;

namespace VoiceChatModifyHook;

public static class ModifyVoiceChat
{
    public static Exiled.Events.Features.Event<VoiceChatListenEvent> OnVoiceChatListen = new();

    internal static VoiceChatChannel SCPChat(VoiceChatChannel channel, ReferenceHub speaker, ReferenceHub listener)
    {
        var ev = new VoiceChatListenEvent(Player.Get(speaker), Player.Get(listener), channel);
        OnVoiceChatListen.InvokeSafely(ev);
        Log.Debug($"{ev.Speaker.Nickname} - {ev.Listener.Nickname} - {ev.VoiceChatChannel}");
        if (ev.IsAllowed == false)
            return VoiceChatChannel.None;
        return ev.VoiceChatChannel;
    }
}
