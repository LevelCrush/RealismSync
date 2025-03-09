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
            new DoMapGasEventPatch().Enable();
            new DoMapRadPatch().Enable();
            new RunReInitPlayerPatch().Enable();
            new InteractableStateChangePatch().Enable();
            new InteractableGroupComponentNamePatch().Enable();
           // new CanTurnValvePatch().Enable();
        }
    }
}