﻿namespace WaitAndChillReborn.LobbyRooms;

using UnityEngine;

public class EscapeRoom : BaseLobbyRoom
{
    public const string Name = "ESCAPE";

    public override void SetupSpawnPoints()
    {
        SpawnPoints.Add(new Vector3(123f, 988f - 700f, 26f));
    }
}
