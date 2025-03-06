using BepInEx.Configuration;
using RealismMod;


namespace RealismModSync.HazardZones;

    
public static class Config
{
    
    private static string SECTION = "Hazard Zones";
    
    public static ConfigEntry<bool> Enabled { get; set; }
    

    public static void Bind(ConfigFile config)
    {

        Enabled = config.Bind<bool>(SECTION, "Enable Hazard Zone Sync. Requires Game restart.", true, new ConfigDescription("Enable/Disable Hazard Sync Entirely", null, new ConfigurationManagerAttributes { Order = 1 }));


    }
    
}