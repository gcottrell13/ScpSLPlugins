using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using System.IO;

namespace DankDitties;

internal static class EventHandlers
{
    public static readonly int DankDittiesAudioApiId = 10000;
    public static void OnRoundStart()
    {
        var fakeConnection = AudioPlayer.Other.Extensions.SpawnDummy("Dank Ditties", "Dank Ditties", "orange", DankDittiesAudioApiId);

        var filePath = DankDittiesPlugin.Singleton.Config.FilePath;
        var dirInfo = new DirectoryInfo(filePath);
        var oggFiles = dirInfo.GetFiles("*.ogg");
        foreach ( var oggFile in oggFiles )
        {
            fakeConnection.audioplayer.Enqueue(oggFile.FullName, 0);
        }

        fakeConnection.audioplayer.Loop = true;
        fakeConnection.audioplayer.Continue = true;

        var player = Player.Get(fakeConnection.hubPlayer);

        player.Position = RoleTypeId.Scp939.GetRandomSpawnLocation().Position;
    }
}
