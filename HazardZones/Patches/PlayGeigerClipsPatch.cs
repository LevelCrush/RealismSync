using System.Reflection;
using RealismMod;
using SPT.Reflection.Patching;

namespace RealismModSync.HazardZones.Patches;

public class PlayGeigerClipsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(AudioController).GetMethod("DoGeigerAudio", BindingFlags.Instance | BindingFlags.Public);
    }
    
    
}