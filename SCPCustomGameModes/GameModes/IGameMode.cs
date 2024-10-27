using Exiled.Events.EventArgs.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.GameModes
{
    internal interface IGameMode
    {
        string Name { get; }

        string PreRoundInstructions { get; }

        void OnRoundStart();
        void OnRoundEnd();

        void OnWaitingForPlayers();
    }
}
