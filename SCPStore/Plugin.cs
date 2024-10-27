namespace SCPStore;

using System;
using Exiled.API.Features;
using Configs;


internal class SCPStore : Plugin<Config, Translation>
{
    public static SCPStore? Singleton;

    public override void OnEnabled()
    {
        Singleton = this;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Singleton = null;
        base.OnDisabled();
    }


    public override string Name => "Store";
    public override string Author => "GCOTTRE";
    public override Version Version => new Version(1, 0, 1);
    public override Version RequiredExiledVersion => new Version(8, 13, 1);
}
