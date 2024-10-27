using CommandSystem;
using CustomGameModes.GameModes.Normal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.Commands;


[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class SpawnBallTag : ICommand
{
    public string Command => "balltag";
    public bool SanitizeResponse => false;

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Spawns a ball to chase and kill people";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        int? ballWaitAfterKillSeconds = null;
        int? ballMaxSpeed = null;
        float? ballAccelPerSecond = null;
        int? tagImmuneSeconds = null;
        foreach (string arg in arguments)
        {
            if (arg.StartsWith("-w=") && parseInt(arg, out ballWaitAfterKillSeconds)) ;
            if (arg.StartsWith("-m=") && parseInt(arg, out ballMaxSpeed)) ;
            if (arg.StartsWith("-a=") && parseFloat(arg, out ballAccelPerSecond)) ;
            if (arg.StartsWith("-t=") && parseInt(arg, out tagImmuneSeconds)) ;
        }

        BallTag tag = new BallTag(ballWaitAfterKillSeconds, ballMaxSpeed, ballAccelPerSecond, tagImmuneSeconds);
        tag.Start();

        response = "Started ball tag";
        return true;
    }

    private bool parseFloat(string flag, out float? value)
    {
        if (float.TryParse(flag.Split('=')[1], out var floatValue))
        {
            value = floatValue; return true;
        }
        throw new Exception($"invalid float in flag value: {flag}");
    }

    private bool parseInt(string flag, out int? value)
    {
        if (int.TryParse(flag.Split('=')[1], out var intValue))
        {
            value = intValue; return true;
        }
        throw new Exception($"invalid int in flag value: {flag}");
    }
}
