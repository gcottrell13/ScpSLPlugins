namespace SCPRandomCoin.API;

internal interface IVoiceChatListen
{
    void OnVoiceChatListen(LabApi.Events.Arguments.PlayerEvents.PlayerReceivingVoiceMessageEventArgs ev);
}
