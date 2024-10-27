using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using PlayerEvent = Exiled.Events.Handlers.Player;
using Scp173Event = Exiled.Events.Handlers.Scp173;
using Scp914Event = Exiled.Events.Handlers.Scp914;
using Scp106Event = Exiled.Events.Handlers.Scp106;
using Scp939Event = Exiled.Events.Handlers.Scp939;
using Scp049Event = Exiled.Events.Handlers.Scp049;
using Scp096Event = Exiled.Events.Handlers.Scp096;
using Scp3114Event = Exiled.Events.Handlers.Scp3114;
using Exiled.Events.EventArgs.Player;
using MEC;
using Exiled.Events.EventArgs.Scp914;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.EventArgs.Scp096;
using System.Linq;
using CustomGameModes.API;
using VoiceChat;
using VoiceChatModifyHook.Events;
using VoiceChatModifyHook;

namespace CustomGameModes.GameModes.Normal;

internal class SCP5000Handler
{
    public static List<SCP5000Handler> Instances { get; } = new();

    public static void UnsubscribeAll()
    {
        foreach (var instance in Instances)
        {
            instance.UnsubscribeEventHandlers();
        }
        Instances.Clear();
    }

    public readonly Player Scp5000Owner;
    RoleTypeId Scp5000OwnerRole;

    CoroutineHandle scp5000Coroutine;
    DateTime LastNoisyAction;

    // time between ticks for checking visibility
    public float TickRateSeconds = 1f;

    public Dictionary<Player, int> VisibleTo = new();

    public SCP5000Handler(Player owner)
    {
        Scp5000Owner = owner;
        Scp5000OwnerRole = owner.Role;
    }

    ~SCP5000Handler()
    {
        UnsubscribeEventHandlers();
    }

    // ----------------------------------------------------------------------------------------------------
    // ----------------------------------------------------------------------------------------------------

    public static void SubscribeStaticEventHandlers()
    {
        Scp914Event.UpgradingPlayer += UpgradingPlayer;
    }

    public static void UnsubscribeStaticEventHandlers()
    {
        Scp914Event.UpgradingPlayer -= UpgradingPlayer;
    }

    public void UnsubscribeEventHandlers()
    {
        PlayerEvent.UsingMicroHIDEnergy -= IsNoisy;
        PlayerEvent.ChangingMicroHIDState -= IsNoisy;
        PlayerEvent.InteractingDoor -= IsNoisy;
        PlayerEvent.InteractingElevator -= IsNoisy;
        PlayerEvent.VoiceChatting -= IsNoisy;
        PlayerEvent.Hurt -= onHurt;
        PlayerEvent.OpeningGenerator -= IsNoisy;
        PlayerEvent.UnlockingGenerator -= IsNoisy;
        PlayerEvent.ClosingGenerator -= IsNoisy;
        PlayerEvent.StoppingGenerator -= IsNoisy;
        PlayerEvent.InteractingLocker -= IsNoisy;

        ModifyVoiceChat.OnVoiceChatListen -= OnVoiceChatListen;

        Scp914Event.Activating -= IsNoisy;
        Scp914Event.ChangingKnobSetting -= IsNoisy;

        Scp939Event.Lunging -= IsNoisy;
        Scp939Event.Clawed -= IsNoisy;

        Scp173Event.Blinking -= IsNoisy;
        Scp173Event.PlacingTantrum -= IsNoisy;

        Scp106Event.Attacking -= IsNoisy;
        Scp106Event.ExitStalking -= IsNoisy;
        Scp106Event.Teleporting -= IsNoisy;

        Scp049Event.Attacking -= IsNoisy;

        Scp096Event.StartPryingGate -= IsNoisy;
        Scp096Event.Enraging -= Scp096SuperNoisy;
        Scp096Event.CalmingDown -= ResetNoisy; // will become invisible soon after calming down
        Scp096Event.AddingTarget -= ShouldAdd096Target;

        Scp3114Event.Revealed -= IsNoisy;
        Scp3114Event.VoiceLines -= IsNoisy;

        PlayerEvent.Shooting -= IsNoisy;

        if (scp5000Coroutine.IsRunning)
        {
            Log.Debug($"Killing SCP 5000 coroutine for player {Scp5000Owner}");
            Timing.KillCoroutines(scp5000Coroutine);
        }
    }

    public void SubscribeOnPlayerGive()
    {
        PlayerEvent.InteractingDoor += IsNoisy;
        PlayerEvent.InteractingElevator += IsNoisy;
        PlayerEvent.OpeningGenerator += IsNoisy;
        PlayerEvent.UnlockingGenerator += IsNoisy;
        PlayerEvent.ClosingGenerator += IsNoisy;
        PlayerEvent.StoppingGenerator += IsNoisy;
        PlayerEvent.InteractingLocker += IsNoisy;

        ModifyVoiceChat.OnVoiceChatListen += OnVoiceChatListen;

        Scp914Event.Activating += IsNoisy;
        Scp914Event.ChangingKnobSetting += IsNoisy;

        switch (Scp5000OwnerRole)
        {
            case RoleTypeId.Scp939:
                {
                    Scp939Event.Lunging += IsNoisy;
                    Scp939Event.Clawed += IsNoisy;
                    break;
                }
            case RoleTypeId.Scp173:
                {
                    Scp173Event.Blinking += IsNoisy;
                    Scp173Event.PlacingTantrum += IsNoisy;
                    break;
                }
            case RoleTypeId.Scp106:
                {
                    Scp106Event.Attacking += IsNoisy;
                    Scp106Event.ExitStalking += IsNoisy;
                    Scp106Event.Teleporting += IsNoisy;
                    break;
                }
            case RoleTypeId.Scp049:
                {
                    Scp049Event.Attacking += IsNoisy;
                    break;
                }
            case RoleTypeId.Scp0492:
                {
                    PlayerEvent.Hurt += onHurt;
                    break;
                }
            case RoleTypeId.Scp096:
                {
                    Scp096Event.StartPryingGate += IsNoisy;
                    Scp096Event.Enraging += Scp096SuperNoisy;
                    // Scp096Event.Charging is not "noisy" in that we don't want it to overwrite the super noisy time
                    Scp096Event.CalmingDown += ResetNoisy; // overwrite the super noisy time, will become invisible soon after calming down
                    Scp096Event.AddingTarget += ShouldAdd096Target;
                    break;
                }
            case RoleTypeId.Scp3114:
                {
                    Scp3114Event.Revealed += IsNoisy;
                    Scp3114Event.VoiceLines += IsNoisy; // lol
                    break;
                }
            default:
                {
                    PlayerEvent.UsingMicroHIDEnergy += IsNoisy;
                    PlayerEvent.ChangingMicroHIDState += IsNoisy;
                    PlayerEvent.VoiceChatting += IsNoisy;
                    PlayerEvent.Shooting += IsNoisy;
                    break;
                }
        }
    }

    static void UpgradingPlayer(UpgradingPlayerEventArgs ev)
    {
        var chance = CustomGameModes.Singleton?.Config.Normal.Scp5000Chance ?? 0; 
        if (Instances.Count == 0 && (
            ev.KnobSetting == Scp914.Scp914KnobSetting.VeryFine
            || ev.KnobSetting == Scp914.Scp914KnobSetting.Fine
            ) && UnityEngine.Random.Range(1, 100) <= chance)
        {
            var scp5000 = new SCP5000Handler(ev.Player);
            scp5000.SetupScp5000();
            ev.Player.Position = ev.OutputPosition;
            ev.IsAllowed = false;
        }
    }

    private void OnVoiceChatListen(VoiceChatListenEvent ev)
    {
        if (ev.Speaker != Scp5000Owner) return;

        var speaker = ev.Speaker;
        var listener = ev.Listener;
        if (speaker == listener)
            return;
        if (VisibleTo.ContainsKey(listener) == false)
            ev.VoiceChatChannel = VoiceChatChannel.None;
    }

    void IsNoisy(IPlayerEvent ev)
    {
        if (ev.Player != Scp5000Owner) return;
        noisy();
    }

    void onHurt(HurtEventArgs ev)
    {
        if (ev.Attacker != Scp5000Owner) return;
        if (ev.Attacker?.Role == RoleTypeId.Scp0492) noisy();
    }

    void Scp096SuperNoisy(EnragingEventArgs ev)
    {
        if (ev.Player != Scp5000Owner) return;
        LastNoisyAction = DateTime.Now + TimeSpan.FromMinutes(5);
        Scp5000Owner.ChangeAppearance(RoleTypeId.Scp096);
    }

    void ResetNoisy(IPlayerEvent ev)
    {
        if (ev.Player != Scp5000Owner) return;
        LastNoisyAction = DateTime.Now;
    }

    void noisy()
    {
        if (LastNoisyAction > DateTime.Now) return;

        foreach (var player in allPlayersThatCanBeAffected().Where(x => (x.Position - Scp5000Owner.Position).MagnitudeIgnoreY() < 20))
        {
            if (!VisibleTo.ContainsKey(player))
            {
                Scp5000Owner.ChangeAppearance(Scp5000OwnerRole, true);
            }
            VisibleTo[player] = 0;
        }

        LastNoisyAction = DateTime.Now;
    }

    IEnumerable<Player> allPlayersThatCanBeAffected() => Player.Get(x => x != Scp5000Owner && x.Role != RoleTypeId.Spectator && x.Role != RoleTypeId.Scp079 && x.Role != RoleTypeId.Overwatch);

    void ShouldAdd096Target(AddingTargetEventArgs ev)
    {
        if (ev.Player != Scp5000Owner) return;
        ev.IsAllowed = VisibleTo.ContainsKey(ev.Player);
    }

    // ----------------------------------------------------------------------------------------------------
    // ----------------------------------------------------------------------------------------------------

    public void SetupScp5000()
    {
        Instances.Add(this);
        foreach (var player in allPlayersThatCanBeAffected())
        {
            VisibleTo[player] = (int)(-5 / TickRateSeconds);
        }

        SubscribeOnPlayerGive();
        ensureScp5000Thread();
        Log.Info($"Gave SCP 5000 to player {Scp5000Owner.NetId} - {Scp5000Owner.Nickname}");
    }

    bool WasRecentlyNoisy => (DateTime.Now - LastNoisyAction).TotalSeconds < 4;

    void ensureScp5000Thread()
    {
        if (scp5000Coroutine.IsRunning == false)
        {
            scp5000Coroutine = Timing.RunCoroutine(scp5000Loop());
        }
    }

    private IEnumerator<float> scp5000Loop()
    {
        while (Scp5000Owner != null && Scp5000Owner.IsConnected && Scp5000Owner.Role.Type == Scp5000OwnerRole)
        {
            if (!WasRecentlyNoisy)
            {
                // Scp5000Owner.EnableEffect(Exiled.API.Enums.EffectType.Invisible, 2f);
                foreach (var kvp in VisibleTo.ToList())
                {
                    var player = kvp.Key;
                    var ticksNotSeen = kvp.Value;
                    if (player.GetVisionInformation(Scp5000Owner).IsLooking == false)
                    {
                        ticksNotSeen++;
                        VisibleTo[player] = ticksNotSeen;
                    }
                    else
                    {
                        VisibleTo[player] = 0;
                    }

                    if (ticksNotSeen >= Round.ElapsedTime.Minutes / 5 + 1)
                    {
                        Scp5000Owner.ChangeAppearance(RoleTypeId.Spectator, new[] { player }, true);
                        VisibleTo.Remove(player);
                        Log.Debug($"{Scp5000Owner.DisplayNickname} - now invisible to {player.DisplayNickname}");
                    }
                }
            }

            yield return Timing.WaitForSeconds(TickRateSeconds);
        }

        UnsubscribeEventHandlers();
        Instances.Remove(this);
    }
}
