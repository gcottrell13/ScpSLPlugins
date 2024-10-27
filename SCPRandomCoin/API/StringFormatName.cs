using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.API;

internal static class StringFormatName
{
    public static string Format(this string s, string name, object value)
    {
        return s.Format(new Dictionary<string, object> { { name, value } });
    }
}
