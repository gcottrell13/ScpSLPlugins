using System.Collections.Generic;

namespace SCPRandomCoin.API;

/// <summary>
/// Each coin effect definition is instantiated at the beginning of each round,
/// and this instance should keep track of the effect's effects during the whole round.
/// </summary>
public interface ICoinEffectDefinition
{
    bool CanHaveEffect(PlayerInfoCache playerInfoCache);

    void DoEffect(PlayerInfoCache playerInfoCache, List<string> hintLines);
}
