using Exiled.API.Interfaces;

namespace SCPRandomCoin.Configs;

internal class Translation : ITranslation
{
    public string GetItem { get; private set; } = "You got {item}.";
    public string LoseItem { get; private set; } = "You lost {item}.";
    public string CoinBreak { get; private set; } = "The coin was used too much and broke.";
    public string Nothing { get; private set; } = "The coin decided to do nothing.";
    public string Heal { get; private set; } = "You were fully healed!";
    public string Tp { get; private set; } = "The coin teleported you.";
    public string Grenade { get; private set; } = "<size=30>GRENADE!</size>";
    public string OneHp { get; private set; } = "The coin says: Try not to die.";
    public string Respawn { get; private set; } = "You brought back {count} players from the dead!";
    public string Respawned { get; private set; } = "{name} brought you back using their coin.";
    public string FeelFunny { get; private set; } = "You feel kind of funny...";
    public string CoinForAll { get; private set; } = "A Coin for Everyone!";
    public string Warhead { get; private set; } = "Better get out now!";
    public string PrizeRoom { get; private set; } = "You have {time} seconds left here";
    public string GoingToSwap { get; private set; } = """
        You will be swapped with another player in {time} seconds.
        <color=blue>.{command}</color> to cancel.
        """;
    public string CancelSwap { get; private set; } = "Target Was Stabilized, Cancelling...";
    public string DestabilizedSwap { get; private set; } = "Your identity feels unstable...";
    public string StabilizedSwap { get; private set; } = "Your identity feels stable again";
    public string OneInTheChamber { get; private set; } = "Minigame: <b>One in the Chamber</b>\nDon't miss!";
    public string OneInTheChamberFinish { get; private set; } = "You landed {count} shots! Impressive!";
    public string FakeScpDeath { get; private set; } = "You killed <color=red>{scp}</color>?";
    public string ReversedControls { get; private set; } = "Reversed Controls!";
}
