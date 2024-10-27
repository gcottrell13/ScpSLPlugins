using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace CustomGameModes.Commands;


[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class AddLightToPlayerCommand : ICommand
{
    public string Command => "player-light";
    public bool SanitizeResponse => false;

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Attempts to add a light to a player";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count != 1)
        {
            response = "must provide a player name";
            return false;
        }

        var targets = arguments.ElementAt(0) == "*" ? Player.List : RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var newargs).Select(Player.Get).ToList();

        if (!targets.Any())
        {
            response = "Invalid player selection";
            return false;
        }

        var lines = new List<string>();

        foreach (Player player in targets)
        {
            var light = Exiled.API.Features.Toys.Light.Create(Vector3.zero);
            light.MovementSmoothing = 60;
            light.Intensity = 10;
            light.Base.transform.SetParent(player.Transform);
            light.Position = player.Position;
            lines.Add($"Spawned in light {light.Base.netId} for player {player.DisplayNickname}");
        }

        response = string.Join("\n", lines);
        return true;
    }
}
