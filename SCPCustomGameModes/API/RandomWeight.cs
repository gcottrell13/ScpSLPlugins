using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameModes.API
{
    internal static class RandomWeight
    {
        public static IEnumerable<(T key, float chance)> Pairs<T>(this Dictionary<T, float> dict)
        {
            foreach (var kvp in dict) yield return (kvp.Key, kvp.Value);
        }

        public static T GetRandom<T>(this Dictionary<T, float> dict)
        {
            var total = dict.Values.Sum();
            var chosenValue = UnityEngine.Random.Range(1, total);
            foreach (var (key, value) in dict.Pairs())
            {
                total -= value;
                if (chosenValue > total) // not GTE since we are subtracting from total before this check.
                {
                    return key;
                }
            }
            return default;
        }
    }
}
