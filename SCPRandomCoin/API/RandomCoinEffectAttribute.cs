using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.API;

internal class RandomCoinEffectAttribute : Attribute
{
    public RandomCoinEffectAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
