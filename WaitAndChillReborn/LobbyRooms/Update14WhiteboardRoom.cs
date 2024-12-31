
using Exiled.API.Enums;
using System.Linq;
using UnityEngine;

namespace WaitAndChillReborn.LobbyRooms;


internal class Update14WhiteboardRoom : BaseLobbyRoom
{
    public const string Name = "Update14Whiteboard";
    protected override RoomType RoomType => RoomType.Hcz106;

    public static readonly Vector3[] LocalPoints = [
        new Vector3(21, 14, -12),
    ];

    public override void SetupSpawnPoints()
    {
        SpawnPoints.AddRange(LocalPoints.Select(ThisRoom.Transform.TransformPoint));
    }
}
