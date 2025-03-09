using RealismModSync.StanceReplication.Patches;

namespace RealismModSync.StanceReplication;

public static class Patch
{
    public static void Awake()
    {
        if (Config.Enabled.Value)
        {
            new CoopPlayer_Create_Patch().Enable();
            new ObservedCoopPlayer_Create_Patch().Enable();
            new RealismLeftShoulderSwapPatch().Enable();

            if (Config.EnableForBots.Value)
            {
                new CoopBot_Create_Patch().Enable();
            }
        }

    }
}