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
    public static readonly int DankDittiesAudioApiId = 10000;
    private static int trackCount = 0;
    private static int currentTrack = 0;

    public static IEnumerator<float> OnRoundStart()
    {
        var filePath = DankDittiesPlugin.Singleton.Config.FilePath;
        var dirInfo = new DirectoryInfo(filePath);
        var oggFiles = dirInfo.GetFiles("*.ogg").ToList();

        trackCount = oggFiles.Count;
        oggFiles.ShuffleList();

        var fakeConnectionList = Extensions.SpawnDummy("Dank Ditties", "Dank Ditties", "orange", DankDittiesAudioApiId);

        yield return Timing.WaitForOneFrame;

        foreach ( var oggFile in oggFiles )
        {
            Log.Debug($"Found file: {oggFile.FullName}");
            fakeConnectionList.audioplayer.Enqueue(oggFile.FullName, 0);
        }

        fakeConnectionList.audioplayer.Loop = true;
        fakeConnectionList.audioplayer.Volume = 15;
        fakeConnectionList.audioplayer.Continue = false;
        fakeConnectionList.audioplayer.LogInfo = true;

        yield return Timing.WaitForSeconds(1);

        currentTrack = 0;
        fakeConnectionList.audioplayer.Play(currentTrack);
        SpawnAndStartPlaying(fakeConnectionList);

        // Timing.RunCoroutine(DankCoroutine());
    }

    public static IEnumerator<float> OnDied(DiedEventArgs ev)
    {
        Extensions.TryGetAudioBot(DankDittiesAudioApiId, out var fakeConnectionList);
        if (ev.Player.ReferenceHub != fakeConnectionList?.hubPlayer)
            yield break;

        if (ev.Attacker != null)
        {
            NextTrack();
        }

        fakeConnectionList.audioplayer.BroadcastTo.AddRange(Player.Get(RoleTypeId.Spectator).Select(x => x.Id));

        yield return Timing.WaitForSeconds(20);

        if (Round.InProgress && ev.Player.IsDead)
            SpawnAndStartPlaying(fakeConnectionList);
    }

    public static void OnSpawn(SpawnedEventArgs ev)
    {
        Extensions.TryGetAudioBot(DankDittiesAudioApiId, out var fakeConnectionList);
        if (fakeConnectionList == null)
            return;

        var bot = Player.Get(fakeConnectionList.hubPlayer);
        if (bot == null)
            return;

        if (ev.Player != bot)
        {
            if (ev.Player.IsScp == bot.IsScp)
                fakeConnectionList.audioplayer.BroadcastTo.Add(ev.Player.Id);
            else
                fakeConnectionList.audioplayer.BroadcastTo.Remove(ev.Player.Id);
        }
        else if (ev.Player.IsScp)
        {
            fakeConnectionList.audioplayer.BroadcastChannel = VoiceChat.VoiceChatChannel.ScpChat;
            fakeConnectionList.audioplayer.ShouldPlay = true;
            fakeConnectionList.audioplayer.BroadcastTo.Clear();
            fakeConnectionList.audioplayer.BroadcastTo.AddRange(Player.Get(x => x.IsScp).Select(x => x.Id));

            Timing.CallDelayed(20f, () =>
            {
                SpawnAndStartPlaying(fakeConnectionList);
            });
        }
        else
        {
            fakeConnectionList.audioplayer.BroadcastTo.Clear();
        }

    }

    public static void SpawnAndStartPlaying(FakeConnectionList fakeConnectionList)
    {
        if (!Round.InProgress)
            return;
        fakeConnectionList.audioplayer.BroadcastChannel = VoiceChat.VoiceChatChannel.Proximity;
        fakeConnectionList.audioplayer.ShouldPlay = true;
        fakeConnectionList.audioplayer.BroadcastTo.Clear();
        var player = Player.Get(fakeConnectionList.hubPlayer);
        player.Role.Set(RoleTypeId.Tutorial);

        foreach (var ragdoll in Ragdoll.Get(player)) ragdoll.Destroy();

        var elevatorTarget = new string[] { "GateB", "GateA", "Nuke" }.GetRandomValue();
        Server.ExecuteCommand($"/el t {elevatorTarget} {player.Id}.");
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
        var bot = Player.Get(fakeConnectionList.hubPlayer);

        while (Round.InProgress)
        {
            yield return Timing.WaitForSeconds(5);
            if (bot.IsAlive)
            {
                var closestPlayer = Player.List.OrderBy(x => (bot.Position - x.Position).magnitude).First(x => x != bot && x.Lift == bot.Lift);
                // Server.ExecuteCommand($"/audio lookat {DankDittiesAudioApiId}", closestPlayer.Sender);
            }
        }
    }
}
