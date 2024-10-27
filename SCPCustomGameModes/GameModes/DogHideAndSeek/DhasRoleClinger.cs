using CustomGameModes.API;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;

namespace CustomGameModes.GameModes
{
    internal class DhasRoleClinger : DhasRole
    {
        public const string name = "clinger";

        public override RoleTypeId RoleType => RoleTypeId.Scientist;

        public override void OnStop()
        {
            if (CurrentTask == BeNearWhenTaskComplete)
            {
                Manager.PlayerCompleteOneTask -= OnSomeoneCompleteTask;
            }
        }

        public override List<dhasTask> Tasks => new()
        {
            BeNearWhenTaskComplete,
            FindAPlayer,
            GetAKeycard,
            FindAPlayer,
        };

        public DhasRoleClinger(Player player, DhasRoleManager manager) : base(player, manager)
        {
            player.Role.Set(RoleType, RoleSpawnFlags.UseSpawnpoint);
        }


        [CrewmateTask(TaskDifficulty.Easy)]
        private IEnumerator<float> GetAKeycard()
        {
            bool predicate(Pickup pickup) => pickup.Type.IsKeycard();
            void onFail() { player.CurrentItem = player.AddItem(ItemType.KeycardScientist); }

            while (GoGetPickup(predicate, onFail) && MyTargetPickup != null)
            {
                var compass = GetCompass(MyTargetPickup.Position);
                FormatTask("Pick up a Keycard", compass);
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}
