using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.API
{
    internal static class CassieCountdownHelper
    {
        public static void SayTimeReminder(int secondsRemaining, string after)
        {
            var sOne = secondsRemaining % 10;
            var sTen = (secondsRemaining % 60) / 10;
            var m = secondsRemaining / 60;

            if (secondsRemaining < 100 && m > 0)
            {
                sTen += 6;
                m = 0;
            }

            var minutes = m > 0 ? $"{m} minutes" : "";
            var secondsTens = sTen > 0 ? $"{sTen}0" : "";
            var secondsOnes = sOne > 0 ? $"{sOne}" : "";
            var seconds = sTen > 0 || sOne > 0 ? "seconds" : "";

            var message = $"{minutes} {secondsTens} {secondsOnes} {seconds} {after}";

            Log.Info($"CASSIE COUNTDOWN - {message}");
            Cassie.Clear();
            Cassie.Message(message, isNoisy: false);
        }

        public static void SayCountdown(int secondsRemaining, int howMany)
        {
            List<string> seconds = new();
            for (var i = 0; i < howMany && secondsRemaining - i > 0; i++) seconds.Add((secondsRemaining - i).ToString());

            var message = string.Join(" yield_1 ", seconds);
            Cassie.Clear();
            Cassie.Message(message, isNoisy: false);
        }
    }
}
