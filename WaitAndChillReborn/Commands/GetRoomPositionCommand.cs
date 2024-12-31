using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WaitAndChillReborn.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class GetRoomPositionCommand : ICommand
{
    public string Command => "room-pos";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "get your position inside the room";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player? player = Player.Get(sender);
        if (player == null)
        {
            response = "player not found";
            return false;
        }

        Room? room = player.CurrentRoom;
        if (room == null)
        {
            response = "room not found";
            return false;
        }

        Vector3 localVector = room.transform.InverseTransformPoint(player.Position);

        response = $"""
            Room Info ({room.Type}):
            FW: {room.Transform.forward}
            UP: {room.Transform.up}
            RIGHT: {room.Transform.right}
            LOCAL POS: {localVector.x}, {localVector.y}, {localVector.z}
            """;

        return true;
    }
}
