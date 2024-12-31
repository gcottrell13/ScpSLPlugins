using Exiled.API.Enums;
using System.Linq;
using UnityEngine;

namespace WaitAndChillReborn.LobbyRooms;

internal class CapybaraRoom : BaseLobbyRoom
{
    public const string Name = "Capybara";
    protected override RoomType RoomType => RoomType.Hcz106;

    public static readonly Vector3[] LocalPoints = [
        new Vector3(5, 12, -10),
    ];

    public override void SetupSpawnPoints()
    {
        SpawnPoints.AddRange(LocalPoints.Select(ThisRoom.Transform.TransformPoint));
    }
}
