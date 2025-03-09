using BepInEx.Configuration;
using RealismMod;


namespace RealismModSync.Audio;
    
public static class Config
{
    
    private static string SECTION = "Audio";
    
    public static ConfigEntry<bool> Enabled { get; set; }
    

    public static void Bind(ConfigFile config)
    {

        Enabled = config.Bind<bool>(SECTION, "Enable Audio Sync. Requires Game restart.", true, new ConfigDescription("Enable/Disable Audio Sync Entirely", null, new ConfigurationManagerAttributes { Order = 1 }));


    }
    
}