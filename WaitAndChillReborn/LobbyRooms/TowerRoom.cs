﻿namespace WaitAndChillReborn.LobbyRooms;

using UnityEngine;

public class TowerRoom : BaseLobbyRoom
{
    public const string Name = "TOWER1";

    public override void SetupSpawnPoints()
    {
        SpawnPoints.Add(new Vector3(39.150f, 1014.112f - 700f, -31.818f));
    }
}
