using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.API
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// Static class that controls showing countdowns to players.
    /// </summary>
    public static class CountdownHelper
    {
        /// <summary>
        /// Gets or sets the main coroutine handle.
        /// </summary>
        public static CoroutineHandle MainHandle { get; set; }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of active countdowns.
        /// </summary>
        public static Dictionary<Player, Countdown> Countdowns { get; } = new();

        public static string CountdownString => "<size=26><color=#5EB3FF><b>{TEXT}</b></color></size>\\n{TIME}";

        /// <summary>
        /// Begins the countdown coroutine.
        /// </summary>
        public static void Start()
        {
            MainHandle = Timing.RunCoroutine(InternalCoroutine());
        }

        public static void Stop()
        {
            Timing.KillCoroutines(MainHandle);
            Countdowns.Clear();
        }

        /// <summary>
        /// Given a <see cref="TimeSpan"/>, returns a pretty string format.
        /// </summary>
        /// <param name="time">The time span.</param>
        /// <returns>String format.</returns>
        public static string Display(TimeSpan time)
        {
            int seconds = Mathf.RoundToInt((float)time.TotalSeconds);

            if (seconds <= 60)
                return Mathf.RoundToInt((float)time.TotalSeconds).ToString();

            string secondsStr = time.Seconds.ToString();
            if (secondsStr.Length == 1)
            {
                secondsStr = $"0{secondsStr}";
            }

            return $"{time.Minutes}:{secondsStr}";
        }

        public static void AddCountdownForAll(string text, TimeSpan ts)
        {
            foreach (var player in Player.List)
            {
                AddCountdown(player, text, ts);
            }
        }

        /// <summary>
        /// Adds a countdown to a player.
        /// </summary>
        /// <param name="player">The player to count down for.</param>
        /// <param name="text">The text to show.</param>
        /// <param name="ts">The time on the countdown.</param>
        public static void AddCountdown(Player player, string text, TimeSpan ts)
        {
            if (Countdowns.ContainsKey(player))
                Countdowns.Remove(player);

            Countdown ct = new(player, text, (int)ts.TotalSeconds);

            Countdowns.Add(player, ct);
            player.ClearBroadcasts();
            player.Broadcast(2, CountdownString.Replace("{TEXT}", ct.Text).Replace("{TIME}", Display(ts)));

            if (MainHandle.IsRunning != true)
            {
                Start();
            }
        }

        private static IEnumerator<float> InternalCoroutine()
        {
            while (true)
            {
                if (Round.IsEnded)
                    break;

                foreach (Player ply in Player.List)
                {
                    if (!Countdowns.TryGetValue(ply, out Countdown ct))
                        continue;

                    if (!ct.Expired)
                    {
                        ply.ClearBroadcasts();
                        ply.Broadcast(2, CountdownString.Replace("{TEXT}", ct.Text).Replace("{TIME}", Display(ct.TimeLeft)));
                    }
                    else
                    {
                        Countdowns.Remove(ply);
                    }
                }

                if (Countdowns.Count == 0)
                {
                    Stop();
                    break;
                }

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
