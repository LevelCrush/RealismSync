using RealismModSync.Audio.Patches;

namespace RealismModSync.Audio;

public static class Patch
{
    public static void Awake()
    {
        if (Config.Enabled.Value)
        {
            new ObservedCoopPlayerCreatePatch().Enable();
            new PlayGeigerClipPatch().Enable();
            new PlayGasAnalyserClipPatch().Enable();
        }
    }
}