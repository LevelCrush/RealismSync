using RealismModSync.HazardZones.Patches;
using RealismModSync.StanceReplication.Patches;

namespace RealismModSync.HazardZones;

public static class Patch
{
    public static void Awake()
    {
        if (Config.Enabled.Value)
        {
            new ShouldSpawnZonePatch().Enable();
            new HandleZoneLootPatch().Enable();
            new HandleZoneAssetsPatch().Enable();
        }
    }
}