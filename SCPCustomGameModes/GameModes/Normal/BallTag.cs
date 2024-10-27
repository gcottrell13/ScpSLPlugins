using Exiled.API.Features;
using Exiled.API.Features.Toys;
using Exiled.Events.EventArgs.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;
using MEC;
using System;
using System.Collections.Generic;
using Exiled.API.Extensions;
using LightToy = Exiled.API.Features.Toys.Light;
using UnityEngine;
using Exiled.API.Enums;
using System.Linq;
using Exiled.API.Features.Doors;
using CustomGameModes.API;
using PlayerRoles;

namespace CustomGameModes.GameModes.Normal;

internal class BallTag
{
    public static List<BallTag> GamesOfTag = new();

    CoroutineHandle handle;

    bool GameRunning;

    Vector3 BallPosition;
    Vector3 PositionOffset = Vector3.up;
    LightToy? TheLight;

    Player? Target;
    Player? LastTarget;

    TimeSpan TagImmunity;
    DateTime LastTagTime; // cannot tag the last target before the tag immunity wears off

    float TickRate = 0.5f;
    DateTime LastRePathTime;
    TimeSpan RePathRate = TimeSpan.FromSeconds(2); // time between each re-pathing to the target

    float BallSpeed = 0f;
    float BallAccelPerSecond;
    float BallMaxSpeed;
    TimeSpan BallWaitAfterKill;

    Queue<Vector3> NextPositions = new();

    public BallTag(int? ballWaitAfterKillSeconds = null, int? ballMaxSpeed = null, float? ballAccelPerSecond = null, int? tagImmuneSeconds = null)
    {
        BallWaitAfterKill = TimeSpan.FromSeconds(ballWaitAfterKillSeconds ?? 10);
        BallMaxSpeed = ballMaxSpeed ?? 2;
        BallAccelPerSecond = ballAccelPerSecond ?? 0.1f;
        TagImmunity = TimeSpan.FromSeconds(tagImmuneSeconds ?? 10);

        GamesOfTag.Add(this);
    }

    public void Start()
    {
        if (handle.IsRunning) throw new Exception($"Instance of {nameof(BallTag)} already started");

        handle = Timing.RunCoroutine(co());
    }

    public void Stop()
    {
        GameRunning = false;
        GamesOfTag.Remove(this);
    }

    public IEnumerator<float> co()
    {

        try
        {

            PlayerEvent.Died += OnDied;
            PlayerEvent.Hurt += OnHurt;

            BallPosition = RoleTypeId.Scp939.GetRandomSpawnLocation().Position;

            YouAreIt(Player.Get(x => x.IsAlive).GetRandomValue());

            TheLight = LightToy.Create(BallPosition, default, default, true, color: Color.blue);
            TheLight.Intensity = 30f;
            TheLight.MovementSmoothing = 3;

            GameRunning = true;

            while (GameRunning)
            {
                yield return Timing.WaitForSeconds(TickRate);

                foreach (Player player in Player.List)
                {
                    if (player.IsAlive) PlayerTick(player);
                }

                if (DateTime.Now >= LastRePathTime + RePathRate)
                {
                    LastRePathTime = DateTime.Now;
                    DoPathing();
                }
                else if (NextPositions.TryDequeue(out Vector3 next))
                {
                    BallSpeed = Math.Min(BallMaxSpeed, BallSpeed + BallAccelPerSecond * TickRate);
                    BallPosition = next;
                    TheLight.Position = next + PositionOffset;

                    TheLight.Color = Color.red;
                }
                else
                {
                    TheLight.Color = Color.yellow;
                }
            }
        }
        finally
        {
            TheLight?.Destroy();

            GameRunning = false;
            PlayerEvent.Died -= OnDied;
            PlayerEvent.Hurt -= OnHurt;

            Log.Info($"Ending {nameof(BallTag)}");
        }
    }

    private void YouAreIt(Player? player)
    {
        if (player == null)
        {
            GameRunning = false;
            return;
        }

        LastTarget = Target;
        Target = player;

        LastTagTime = DateTime.Now;
        LastRePathTime = DateTime.Now + BallWaitAfterKill;
        BallSpeed = 0;

        Log.Info($"Player {Target.DisplayNickname} is now IT");

        Target?.ShowHint("You are IT!", 6);
        LastTarget?.ShowHint("You are NOT IT", 4);
    }

    private void PlayerTick(Player player)
    {
        if (player == Target)
        {
            if (NextPositions.Count == 0) 
                return;

            if ((player.Position - BallPosition).sqrMagnitude < 10)
            {
                player.Hurt(50 * TickRate);
            }
        }
        else
        {
            if ((player.Position - BallPosition).sqrMagnitude < 4)
            {
                player.EnableEffect(EffectType.SinkHole, 2 * TickRate);
            }
        }
    }


    private void OnHurt(HurtEventArgs ev)
    {
        if (ev.Attacker != Target) 
            return;

        if (ev.Player == LastTarget && DateTime.Now < LastTagTime + TagImmunity) 
            return;
        YouAreIt(ev.Player);
    }

    private void OnDied(DiedEventArgs ev)
    {
        if (ev.Player != Target) 
            return;

        YouAreIt(Player.List.GetRandomValue(p => p.IsAlive && p.Role != RoleTypeId.Scp079));
    }


    private void DoPathing()
    {
        if (Target == null) return;

        if ((Target.Zone & (ZoneType.HeavyContainment & ZoneType.Entrance)) != 0)
        {
            NextPositions.Clear();
            return;
        }
        if (Target.CurrentRoom.Type == RoomType.Hcz049 || Target.CurrentRoom.Type == RoomType.HczNuke)
        {
            NextPositions.Clear();
            return;
        }

        Room currentBallRoom = Room.Get(BallPosition);
        if (currentBallRoom == null) return;

        void ballInSameRoom(Vector3 workingpos)
        {
            Vector3 delta = Target.Position - workingpos;
            Vector3 deltaNorm = delta.NormalizeIgnoreY();
            float magnitude = delta.magnitude;
            for (float i = BallSpeed; i < magnitude; i += BallSpeed)
            {
                NextPositions.Enqueue(workingpos + deltaNorm * i);
            }
            NextPositions.Enqueue(Target.Position);
        }

        if (Target.CurrentRoom == currentBallRoom)
        {
            NextPositions.Clear();
            ballInSameRoom(BallPosition);
        }
        else
        {
            RoomSearch fromTarget = new RoomSearch(Target.CurrentRoom);
            RoomSearch fromBall = new RoomSearch(currentBallRoom);

            while (!fromTarget.Rooms.Overlaps(fromBall.Rooms))
            {
                if (!fromTarget.TrySearchOneMore() || !fromBall.TrySearchOneMore())
                {
                    NextPositions.Clear();
                    return;
                }
            }

            int numberOfPositionsToQueue = (int)(RePathRate.TotalSeconds / TickRate) + 1;

            Room midpointRoom = fromTarget.Rooms.Intersect(fromBall.Rooms).First();
            Queue<Room> totalPath = new Queue<Room>(fromBall.PathByRoom[midpointRoom].Concat(fromTarget.PathByRoom[midpointRoom].Reversed().Skip(1))); // Skip(1) to deduplicate the midpoint room
            Room currentRoom = totalPath.Dequeue();
            Vector3 workingPosition = BallPosition;

            NextPositions.Clear();
            while (NextPositions.Count < numberOfPositionsToQueue)
            {
                if (totalPath.Count == 0 || currentRoom == Target.CurrentRoom)
                {
                    // if the working position has reached the same room as the target, then change the pathing strategy
                    ballInSameRoom(workingPosition);
                    break;
                }
                Room nextRoom = totalPath.Peek();

                Vector3 destination = currentRoom.Doors.FirstOrDefault(door => door.Rooms.Contains(nextRoom)).Position;

                Vector3 delta = destination - workingPosition;
                Vector3 deltaNorm = delta.NormalizeIgnoreY();
                float magnitude = (delta.magnitude / BallSpeed) + 1;
                for (float i = 0; i < magnitude; i ++)
                {
                    workingPosition += deltaNorm * BallSpeed;
                    NextPositions.Enqueue(workingPosition);
                }
                currentRoom = totalPath.Dequeue();
            }
        }

    }

}


internal class RoomSearch
{
    public HashSet<Room> Rooms = new();
    public Dictionary<Room, List<Room>> PathByRoom = new();

    private HashSet<Room> Frontier = new();

    public RoomSearch(Room startingRoom)
    {
        Frontier.Add(startingRoom);
        PathByRoom[startingRoom] = new() { startingRoom };
    }

    public bool TrySearchOneMore()
    {
        if (Frontier.Count == 0) 
            return false;

        var f = Frontier.ToList();
        Frontier.Clear();
        // we don't need to search through elevators. Light Containment, Surface, 049 and warhead can be safe zones
        foreach (Room room in f)
        {
            Rooms.Add(room);

            foreach (Door door in room.Doors)
            {
                if (door.Rooms.Count < 2) continue;

                foreach (Room neighbor in door.Rooms)
                {
                    if (!Rooms.Contains(neighbor))
                    {
                        PathByRoom[neighbor] = PathByRoom[room].Append(neighbor).ToList();
                        Frontier.Add(neighbor);
                    }
                }
            }
        }

        return true;
    }
}