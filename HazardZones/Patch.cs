using RealismModSync.HazardZones.Patches;
using RealismModSync.StanceReplication.Patches;

namespace RealismModSync.HazardZones;

public static class Patch
{
    public static void Awake()
    {
        new ShouldSpawnZonePatch().Enable();
        new HandleZoneLootPatch().Enable();
        new HandleZoneAssetsPatch().Enable();
    }
}