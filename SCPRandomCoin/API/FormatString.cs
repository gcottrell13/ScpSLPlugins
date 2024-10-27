using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.API;

internal static class FormatString
{
    public static string Format(this string format, Dictionary<string, object> replacements)
    {
        foreach (var pair in replacements)
        {
            format = format.Replace("{" + pair.Key + "}", pair.Value.ToString());
        }
        return format;
    }
}
