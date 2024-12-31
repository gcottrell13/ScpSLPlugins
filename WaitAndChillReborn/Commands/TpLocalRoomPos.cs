using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WaitAndChillReborn.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class TpLocalRoomPos : ICommand
{
    public string Command => "tp-local-room-pos";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "tp to room, affected by room pos and rotation: [RoomType] [x] [y] [z]";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player? player = Player.Get(sender);
        if (player == null)
        {
            response = "player not found";
            return false;
        }

        if (arguments.Count != 4)
        {
            response = Description;
            return false;
        }

        if (!Enum.TryParse<RoomType>(arguments.ElementAt(0), out var roomName))
        {
            response = $"invalid room name.\n{Description}";
            return false;
        }

        if (!int.TryParse(arguments.ElementAt(1), out int x) 
            || !int.TryParse(arguments.ElementAt(2), out int y) 
            || !int.TryParse(arguments.ElementAt(3), out int z))
        {
            response = $"invalid position vector.\n{Description}";
            return false;
        }

        Room room = Room.Get(roomName);
        Vector3 pos = room.Transform.TransformPoint(new Vector3(x, y, z));
        player.Position = pos;
        response = $"""
            Teleported to:
            Room: {roomName}
            Position: {pos.x} {pos.y} {pos.z}
            """;
        return true;
    }
}
