using CustomGameModes.GameModes;
using Exiled.API.Interfaces;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomGameModes.Configs
{
    internal class Config : IConfig
    {
        [Description("plugin is enabled")]
        public bool IsEnabled { get; set; }

        [Description("debug messages from plugin")]
        public bool Debug { get; set; }

        [Description("pregame round instruction size")]
        public int PregameRoundInstructionSize { get; set; } = 15;

        [Description("game modes")]
        public List<string> GameModes { get; set; } = new()
        {
            "dhas",
            "z",
            "n",
            "n",
        };


        public NormalSCPSLConfig Normal { get; set; } = new NormalSCPSLConfig();

        public InfectionConfig Infection { get; set; } = new InfectionConfig();

        public DHASConfig DogHideAndSeek { get; set; } = new DHASConfig();

        public TTTConfig TroubleInLightContainment { get; set; } = new TTTConfig();

        public SCP5000TestConfigs Scp5000 { get; set; } = new SCP5000TestConfigs();
    }
}
