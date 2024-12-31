using Exiled.API.Enums;
using System.Linq;
using UnityEngine;

namespace WaitAndChillReborn.LobbyRooms;

internal class HczMicroHidRoom : BaseLobbyRoom
{
    public const string Name = "HczMicroHid";
    protected override RoomType RoomType => RoomType.HczHid;

    public static readonly Vector3[] LocalPoints = [
        new Vector3(-4, 5, 6),
    ];

    public override void SetupSpawnPoints()
    {
        SpawnPoints.AddRange(LocalPoints.Select(ThisRoom.Transform.TransformPoint));
    }
}
