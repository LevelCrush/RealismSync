using BepInEx.Configuration;


namespace RealismModSync.StanceReplication;

    
public static class Config
{
    public static ConfigEntry<bool> Enabled { get; set; }
    public static ConfigEntry<bool> EnableForBots { get; set; }
    public static ConfigEntry<bool> EnableShoulderSwap { get; set; }
        
    public static ConfigEntry<float> CancelTimer { get; set; }
    public static ConfigEntry<float> ResetTimer { get; set; }

    private static string SECTION = "Stance Replication";

    public static void Bind(ConfigFile config)
    {
        Enabled = config.Bind<bool>(SECTION, "Enable Stance Replication", true, new ConfigDescription("Requires Restart. Toggles replication module. If any client has this off, ALL clients must also have this off. Otherwise issues will occur.", null, new ConfigurationManagerAttributes { Order = 0 }));

        EnableForBots = config.Bind<bool>(SECTION, "Enable Stance Replication For Bots", true, new ConfigDescription("Requires Restart. Toggles replication for bots. Disabling can help improve performance if there are any issues.", null, new ConfigurationManagerAttributes { Order = 1 }));
        EnableShoulderSwap = config.Bind<bool>(SECTION, "Enable Player Shoulder Swap Replication", true,
            new ConfigDescription(
                "Allow your shoulder swapping to be replicated. If your teammates are saying you look disjointed, disabling shoulder swap replication may help."));
        ResetTimer = config.Bind<float>(SECTION, "Reset Timer", 0.2f, new ConfigDescription("Time before stance resets after sprinting or collision.", new AcceptableValueRange<float>(0.0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 3, IsAdvanced = true }));
        CancelTimer = config.Bind<float>(SECTION, "Cancel Timer", 0.2f, new ConfigDescription("Time before stance is cancelled due to sprinting or collision.", new AcceptableValueRange<float>(0.0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 4, IsAdvanced = true }));

    }
    
}