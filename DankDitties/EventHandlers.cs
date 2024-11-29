using AudioPlayer.Other;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using SCPSLAudioApi.AudioCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DankDitties;

internal static class EventHandlers
{
    public static int DankDittiesAudioApiId = 10000;
    private static int trackCount = 0;
    private static int currentTrack = 0;
    private static Player? BotPlayer;

    public static IEnumerator<float> OnRoundStart()
    {
        var filePath = DankDittiesPlugin.Singleton.Config.FilePath;
        var dirInfo = new DirectoryInfo(filePath);
        var oggFiles = dirInfo.GetFiles("*.ogg").ToList();

        trackCount = oggFiles.Count;
        oggFiles.ShuffleList();

        var fakeConnectionList = Extensions.SpawnDummy("Dank Ditties", "Dank Ditties", "orange", DankDittiesAudioApiId);
        DankDittiesAudioApiId = fakeConnectionList.BotID;

        yield return Timing.WaitForOneFrame;

        foreach ( var oggFile in oggFiles )
        {
            Log.Debug($"Found file: {oggFile.FullName}");
            fakeConnectionList.audioplayer.Enqueue(oggFile.FullName, 0);
        }
        fakeConnectionList.audioplayer.Loop = true;
        fakeConnectionList.audioplayer.Volume = 8;
        fakeConnectionList.audioplayer.Continue = false;
        fakeConnectionList.audioplayer.LogInfo = true;

        yield return Timing.WaitForSeconds(1);

        BotPlayer = Player.Get(fakeConnectionList.hubPlayer);
        currentTrack = 0;
        fakeConnectionList.audioplayer.Play(currentTrack);
        SpawnAndStartPlaying(fakeConnectionList);

        // Timing.RunCoroutine(DankCoroutine());
    }

    public static IEnumerator<float> OnDied(DiedEventArgs ev)
    {
        Extensions.TryGetAudioBot(DankDittiesAudioApiId, out var fakeConnectionList);
        if (BotPlayer != ev.Player)
        {
            // hide the bot in the player list so spectators can't spectate
            BotPlayer?.ChangeAppearance(RoleTypeId.Spectator, new[] { ev.Player });
            if (BotPlayer?.IsDead != true)
                fakeConnectionList.audioplayer.BroadcastTo.Remove(ev.Player.Id);
        }
        else
        {
            if (ev.Attacker != null)
            {
                NextTrack();
            }

            fakeConnectionList.audioplayer.BroadcastTo.Clear();
            fakeConnectionList.audioplayer.BroadcastTo.AddRange(Player.Get(RoleTypeId.Spectator).Select(x => x.Id));
            yield return Timing.WaitForSeconds(20);

            if (Round.InProgress && ev.Player.IsDead)
                SpawnAndStartPlaying(fakeConnectionList);
        }
    }

    public static IEnumerator<float> OnSpawn(SpawnedEventArgs ev)
    {
        Extensions.TryGetAudioBot(DankDittiesAudioApiId, out var fakeConnectionList);
        if (fakeConnectionList == null)
            yield break;

        if (BotPlayer == null)
            yield break;

        if (ev.Player != BotPlayer && !ev.Player.IsDead)
        {
            fakeConnectionList.audioplayer.BroadcastTo.Add(ev.Player.Id);
            BotPlayer.ChangeAppearance(BotPlayer.Role, new[] { ev.Player });
        }
        else if (ev.Player.IsScp)
        {
            fakeConnectionList.audioplayer.BroadcastChannel = VoiceChat.VoiceChatChannel.ScpChat;
            fakeConnectionList.audioplayer.ShouldPlay = true;
            fakeConnectionList.audioplayer.BroadcastTo.Clear();
            fakeConnectionList.audioplayer.BroadcastTo.AddRange(Player.Get(x => x.IsScp).Select(x => x.Id));
            yield return Timing.WaitForSeconds(20);
            SpawnAndStartPlaying(fakeConnectionList);
        }
        else if (!BotPlayer.IsDead)
        {
            yield return Timing.WaitForOneFrame;
            BotPlayer.ChangeAppearance(RoleTypeId.Spectator, Player.Get(RoleTypeId.Spectator));
            foreach (var spec in Player.Get(RoleTypeId.Spectator))
                fakeConnectionList.audioplayer.BroadcastTo.Remove(spec.Id);
        }
    }

    public static void SpawnAndStartPlaying(FakeConnectionList fakeConnectionList)
    {
        if (!Round.InProgress)
            return;
        if (BotPlayer == null)
            return;
        fakeConnectionList.audioplayer.BroadcastChannel = VoiceChat.VoiceChatChannel.Proximity;
        fakeConnectionList.audioplayer.ShouldPlay = true;
        fakeConnectionList.audioplayer.BroadcastTo.AddRange(Player.List.Select(x => x.Id));
        BotPlayer.Role.Set(RoleTypeId.Tutorial);
        BotPlayer.CurrentItem = BotPlayer.AddItem(ItemType.KeycardJanitor);

        foreach (var ragdoll in Ragdoll.Get(BotPlayer)) ragdoll.Destroy();

        var elevatorTarget = new string[] { "GateB", "GateA", "Nuke" }.GetRandomValue();
        Server.ExecuteCommand($"/el t {elevatorTarget} {BotPlayer.Id}.");
    }

    public static void OnFinishedTrack(AudioPlayerBase playerBase, string track, bool directPlay, ref int nextQueuePos) => NextTrack();

    public static void NextTrack()
    {
        if (!Extensions.TryGetAudioBot(DankDittiesAudioApiId, out var fakeConnectionList))
            return;
        Log.Debug("Next Track");
        currentTrack = (currentTrack + 1) % trackCount;
        fakeConnectionList.audioplayer.Play(currentTrack);
    }

    public static IEnumerator<float> DankCoroutine()
    {
        Extensions.TryGetAudioBot(DankDittiesAudioApiId, out var fakeConnectionList);

        while (Round.InProgress)
        {
            yield return Timing.WaitForSeconds(5);
            if (BotPlayer?.IsAlive == true)
            {
                var closestPlayer = Player.List.OrderBy(x => (BotPlayer.Position - x.Position).magnitude).First(x => x != BotPlayer && x.Lift == BotPlayer.Lift);
                // Server.ExecuteCommand($"/audio lookat {DankDittiesAudioApiId}", closestPlayer.Sender);
            }
        }
    }
}
