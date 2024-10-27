using Exiled.API.Features;
using MEC;
using PlayerRoles;
using PlayerRoles.Spectating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.GameModes
{
    internal class SpectatorRole : DhasRole
    {
        public const string name = "spectator";

        public SpectatorRole(Player player, DhasRoleManager manager) : base(player, manager)
        {
            player.Role.Set(RoleType, RoleSpawnFlags.UseSpawnpoint);
        }

        public override List<dhasTask> Tasks => new()
        {
            Spectate,
        };

        public override void OnStop()
        {
        }

        public override string CountdownBroadcast => "The Class-D are hiding!";
        public override string SickoModeBroadcast => MainGameBroadcast;
        public override string MainGameBroadcast => "You Died. Enjoy the show!";
        public override string RoundEndBroadcast => "Round End";

        public override void GetPlayerReadyAndEquipped()
        {
        }
        public override void OnCompleteAllTasks() { }

        public override void ShowTaskCompleteMessage(float duration) { }

        public override RoleTypeId RoleType => RoleTypeId.Spectator;

        [CrewmateTask(TaskDifficulty.None)]
        public IEnumerator<float> Spectate()
        {
            while (player.IsDead)
            {
                yield return Timing.WaitForSeconds(1);
                var spectating = Player.List.FirstOrDefault(p => p.ReferenceHub.IsSpectatedBy(player.ReferenceHub));

                if (spectating == null)
                    continue;

                var theirHint = Manager.PlayerRoles[spectating].CurrentTaskHint;

                var myHint = $"""
                    Spectating: {PlayerNameFmt(spectating)}

                    {theirHint}
                    """;

                player.ShowHint(myHint, 2);
            }
        }
    }
}
