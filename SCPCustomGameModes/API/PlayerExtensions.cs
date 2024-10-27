using Exiled.API.Features;
using Exiled.API.Features.Roles;
using PlayerRoles.PlayableScps;

namespace CustomGameModes.API;

internal static class PlayerExtensions
{
    /// <summary>
    /// Gets vision information based on the specified target player and optional mask layer.
    /// </summary>
    /// <param name="target">The Player to target.</param>
    /// <param name="maskLayer">The mask layer to use (default is 0).</param>
    /// <returns>A <see cref="VisionInformation"/> object containing the provided information.</returns>
    public static VisionInformation GetVisionInformation(this Player me, Player target, int maskLayer = 0) =>
        VisionInformation.GetVisionInformation(me.ReferenceHub, me.CameraTransform, target.Position, 15, 0, true, true, (int)maskLayer, true);
}
