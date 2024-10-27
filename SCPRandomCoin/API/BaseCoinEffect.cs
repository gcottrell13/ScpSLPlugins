using SCPRandomCoin.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRandomCoin.API;

public abstract class BaseCoinEffect
{
    internal Translation translation => SCPRandomCoin.Singleton?.Translation ?? throw new ArgumentNullException();
    internal Config config => SCPRandomCoin.Singleton?.Config ?? throw new ArgumentNullException();
}
