namespace SCPStore.API;

public class ColorHelper
{
    public static string Color(Misc.PlayerInfoColorTypes color, string innerText) => $"<color={Misc.AllowedColors[color]}>{innerText}</color>";
    public static string Pink(string message) => Color(Misc.PlayerInfoColorTypes.Pink, message);
    public static string Red(string message) => Color(Misc.PlayerInfoColorTypes.Red, message);
    public static string Brown(string message) => Color(Misc.PlayerInfoColorTypes.Brown, message);
    public static string Silver(string message) => Color(Misc.PlayerInfoColorTypes.Silver, message);
    public static string LightGreen(string message) => Color(Misc.PlayerInfoColorTypes.LightGreen, message);
    public static string Crimson(string message) => Color(Misc.PlayerInfoColorTypes.Crimson, message);
    public static string Cyan(string message) => Color(Misc.PlayerInfoColorTypes.Cyan, message);
    public static string Aqua(string message) => Color(Misc.PlayerInfoColorTypes.Aqua, message);
    public static string DeepPink(string message) => Color(Misc.PlayerInfoColorTypes.DeepPink, message);
    public static string Tomato(string message) => Color(Misc.PlayerInfoColorTypes.Tomato, message);
    public static string Yellow(string message) => Color(Misc.PlayerInfoColorTypes.Yellow, message);
    public static string Magenta(string message) => Color(Misc.PlayerInfoColorTypes.Magenta, message);
    public static string BlueGreen(string message) => Color(Misc.PlayerInfoColorTypes.BlueGreen, message);
    public static string Orange(string message) => Color(Misc.PlayerInfoColorTypes.Orange, message);
    public static string Lime(string message) => Color(Misc.PlayerInfoColorTypes.Lime, message);
    public static string Green(string message) => Color(Misc.PlayerInfoColorTypes.Green, message);
    public static string Emerald(string message) => Color(Misc.PlayerInfoColorTypes.Emerald, message);
    public static string Carmine(string message) => Color(Misc.PlayerInfoColorTypes.Carmine, message);
    public static string Nickel(string message) => Color(Misc.PlayerInfoColorTypes.Nickel, message);
    public static string Mint(string message) => Color(Misc.PlayerInfoColorTypes.Mint, message);
    public static string ArmyGreen(string message) => Color(Misc.PlayerInfoColorTypes.ArmyGreen, message);
    public static string Pumpkin(string message) => Color(Misc.PlayerInfoColorTypes.Pumpkin, message);
    public static string Black(string message) => Color(Misc.PlayerInfoColorTypes.Black, message);
    public static string White(string message) => Color(Misc.PlayerInfoColorTypes.White, message);
}
