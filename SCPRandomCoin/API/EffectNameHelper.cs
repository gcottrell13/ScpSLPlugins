using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SCPRandomCoin.API;

public static class EffectNameHelper
{
    internal static Dictionary<TypeInfo, string> pluginNameCache = new();

    public static string GetEffectName<T>()
        where T : ICoinEffectDefinition
    {
        var name = typeof(T).GetCustomAttribute<RandomCoinEffectAttribute>()?.Name 
            ?? throw new ArgumentException($"Must define a {nameof(RandomCoinEffectAttribute)} on the class {typeof(T).Name}");
        return name;
    }
}
