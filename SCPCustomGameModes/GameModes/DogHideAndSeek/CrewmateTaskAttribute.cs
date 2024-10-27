using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.GameModes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class CrewmateTaskAttribute : Attribute
    {
        public CrewmateTaskAttribute(TaskDifficulty difficulty)
        {
            Difficulty = difficulty;
        }

        public TaskDifficulty Difficulty { get; }
    }

    internal enum TaskDifficulty
    {
        None = 0,
        Easy,
        Medium,
        Hard,
    }

    internal static class TaskDifficultyExtensions
    {
        public static int DifficultyTime(this TaskDifficulty difficulty) => difficulty switch
        {
            TaskDifficulty.Easy => 10,
            TaskDifficulty.Medium => 20,
            TaskDifficulty.Hard => 60,
            _ => 0,
        };
    }
}
