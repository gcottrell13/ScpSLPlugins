using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SCPRandomCoin.API;

/// <summary>
/// Register custom coin effects
/// </summary>
public static class CoinEffectRegistry
{
    internal static Dictionary<string, Func<ICoinEffectDefinition>> registry = new();
    internal static Dictionary<Type, string> typeToName = new();


    /// <summary>
    /// Add a coin effect. Returns true if the registration was successful.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryRegisterEffect<T>() 
        where T : ICoinEffectDefinition, new()
    {
        var name = EffectNameHelper.GetEffectName<T>();
        if (registry.ContainsKey(name))
            return false;
        // this is the fastest option compared to Activator.CreateInstance and ConstructorInfo.Invoke
        Func<ICoinEffectDefinition> func = Expression.Lambda<Func<ICoinEffectDefinition>>(
            Expression.New(typeof(T).GetConstructor(Type.EmptyTypes))
        ).Compile();
        registry.Add(name, func);
        typeToName.Add(typeof(T), name);
        return true;
    }

    /// <summary>
    /// Remove an effect
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryRemoveEffect<T>()
    {
        if (!typeToName.TryGetValue(typeof(T), out var name))
        {
            return false;
        }
        registry.Remove(name);
        typeToName.Remove(typeof(T));
        return true;
    }

    internal static IReadOnlyDictionary<string, ICoinEffectDefinition> GetEffectDefinitions() => registry.ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value()
    );

    internal static HashSet<string> disabledEffects = new();
    public static void DisableEffects(params string[] names) => disabledEffects.UnionWith(names);
    public static void EnableEffects(params string[] names) => disabledEffects.ExceptWith(names);

    public static void DisableEffect<T>() where T : ICoinEffectDefinition => DisableEffects(EffectNameHelper.GetEffectName<T>());
    public static void EnableEffect<T>() where T : ICoinEffectDefinition => EnableEffects(EffectNameHelper.GetEffectName<T>());

    public static void DisableAll()
    {
        foreach (var name in registry.Keys)
            disabledEffects.Add(name);
    }
    public static void EnableAll()
    {
        disabledEffects.Clear();
    }
}
