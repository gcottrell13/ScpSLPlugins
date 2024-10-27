using Exiled.API.Features;
using MEC;
using RueI.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SCPStore.API;

public class PlayerHintMenu
{
    public static Dictionary<Player, PlayerHintMenu> ByPlayerDict = new();
    private static Regex stripTagsRegex = new(@"<.*?>");

    private const string selectorId = ">>> ";
    private static string selectorPadding = new string(' ', (int)(CharacterLengths.StringSize(selectorId) / CharacterLengths.Lengths[' ']));

    public PlayerHintMenu(Player player) : this(player, new List<HintMenuItem>())
    {
    }

    public PlayerHintMenu(Player player, IEnumerable<HintMenuItem> items)
    {
        Items = items.ToList();
        Player = player;
        ByPlayerDict[player] = this;
    }

    public Action? OnHide;

    private CoroutineHandle countdownHandle;
    public List<HintMenuItem> Items { get; }
    public Player Player { get; }

    public int currentIndex = -1;

    public void AddItem(HintMenuItem item)
    {
        Items.Add(item);
    }

    public void Next()
    {
        var startIndex = currentIndex;
        while (true)
        {
            currentIndex++;
            if (currentIndex >= Items.Count)
            {
                currentIndex = -1;
            }
            if (currentIndex == startIndex)
                break;
            if (currentIndex >= 0 && Items[currentIndex].OnSelect != null)
                break;
        };
    }

    public void CountdownToSelect(string headerText, float seconds)
    {
        if (countdownHandle.IsRunning)
            Timing.KillCoroutines(countdownHandle);

        var start = DateTime.Now;
        IEnumerator<float> co()
        {
            yield return Timing.WaitForOneFrame;
            while ((DateTime.Now - start).TotalSeconds < seconds)
            {
                var current = GetCurrent();
                var rows = new List<List<string>>();
                var rowIndex = 0;
                var longestStrInColumn = new List<float>() { 0 };
                var colIndex = 0;
                var itemNumber = 0;
                var cursor = ColorHelper.Color(Misc.PlayerInfoColorTypes.Magenta, selectorId);
                foreach (var item in Items)
                {
                    var itemText = item.Text();
                    var number = item.OnSelect == null ? "" : $"{++itemNumber}. ";
                    foreach (var line in itemText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (rows.Count <= rowIndex)
                        {
                            rows.Add(new(new string[colIndex]));
                        }
                        var str = $"{(item == current ? cursor : selectorPadding)}{number}{line}".Replace("\n", "");
                        var withoutTags = CharacterLengths.StringSize(str.Contains("</") ? stripTagsRegex.Replace(str, "") : str);
                        if (withoutTags > longestStrInColumn[colIndex])
                            longestStrInColumn[colIndex] = withoutTags;
                        rows[rowIndex].Add(str);
                        rowIndex++;
                    }
                    if (item.ColumnBreakAfter == true)
                    {
                        rowIndex = 0;
                        colIndex++;
                        longestStrInColumn.Add(0);
                    }
                }

                var list = string.Join("\n", rows.Select(row =>
                {
                    while (row.Count <= colIndex) row.Add("");
                    var ret = string.Join("  ", row.Select((item, index) =>
                    {
                        var str = CharacterLengths.StringSize(item == null ? "" : item.Contains("</") ? stripTagsRegex.Replace(item, "") : item);
                        var diff = longestStrInColumn[index] - str;
                        return (item ?? "") + new string(' ', (int)(diff / CharacterLengths.Lengths[' ']));
                    }));
                    return ret + "|";
                }));
                var timer = seconds - (int)(DateTime.Now - start).TotalSeconds;
                var totalText = $"""
                    {headerText}{timer}

                    {list}
                    """;
                Player.ShowHint(totalText, 3);
                yield return Timing.WaitForSeconds(1);
            }
            if (InvokeSelection(out var returnText))
            {
                Player.ShowHint(returnText, 3);
            }
            else
            {
                Player.ShowHint("");
                Dispose();
            }
        }
        countdownHandle = Timing.RunCoroutine(co());
    }

    public HintMenuItem? GetCurrent() => currentIndex >= 0 ? Items[currentIndex] : null;

    public bool InvokeSelection(out string? returnText)
    {
        if (GetCurrent()?.OnSelect == null)
        {
            returnText = null;
            return false;
        }
        returnText = GetCurrent()?.OnSelect?.Invoke();
        Dispose();
        return true;
    }

    public void Dispose()
    {
        ByPlayerDict.Remove(Player);
        if (countdownHandle.IsRunning)
            Timing.KillCoroutines(countdownHandle);
        OnHide?.Invoke();
    }
}

public class HintMenuItem
{
    public readonly Func<string> Text;
    public readonly Func<string>? OnSelect;
    public readonly string? ActionName;
    public bool? ColumnBreakAfter = null;

    public HintMenuItem(Func<string> text)
    {
        Text = text;
    }

    public HintMenuItem(Func<string> text, Func<string> onSelect, string actionName) : this(text)
    {
        OnSelect = onSelect;
        ActionName = actionName;
    }
}