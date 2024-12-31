namespace WaitAndChillReborn.LobbyRooms;

using Exiled.API.Enums;
using Exiled.API.Features;
using System.Linq;
using UnityEngine;

internal class PC15Room : BaseLobbyRoom
{
    public const string Name = "PC15";

    protected override RoomType RoomType => RoomType.LczCafe;

    public static readonly Vector3[] LocalPoints = [
        new Vector3(3, 1, 0),
        new Vector3(7.5f, 1, -4.5f),
    ];

    public override void SetupSpawnPoints()
    {
        SpawnPoints.AddRange(LocalPoints.Select(ThisRoom.Transform.TransformPoint));
    }
}
