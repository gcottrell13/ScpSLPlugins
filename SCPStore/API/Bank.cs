using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPStore.API;

public class Bank
{
    private Dictionary<(Player player, string currency), int> Accounts = new();

    public void AddCredits(Player player, string currency, int amount)
    {
        var key = (player, currency);
        if (!Accounts.ContainsKey(key))
            Accounts[key] = 0;
        Accounts[key] += amount;
    }

    public int GetCredits(Player player, string currency)
    {
        var key = (player, currency);
        return Accounts.TryGetValue(key, out var credits) ? credits : 0;
    }
}
