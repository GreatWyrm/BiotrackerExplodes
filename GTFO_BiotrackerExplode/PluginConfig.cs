using System.IO;
using BepInEx;
using BepInEx.Configuration;
using GTFO.API.Utilities;

namespace BiotrackerExplode;

public class PluginConfig
{
    public struct ConfigSettings
    {
        public double chance;
        public double threshold;
        public bool includeScouts;
        public bool isLethal;
        public bool includeTanks;
        public bool includePablo;
    }
    
    private static ConfigFile ConfigFile;
    private static ConfigEntry<double> explodeChance;
    private static ConfigEntry<int> explodeThreshold;
    private static ConfigEntry<bool> includeScouts;
    private static ConfigEntry<bool> willExplodeSelf;
    private static ConfigEntry<bool> includeTanks;
    private static ConfigEntry<bool> includePablo;

    public static void Init()
    {
        ConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, Plugin.MOD_NAME + ".cfg"), saveOnInit: true);
        
        explodeChance = ConfigFile.Bind("Exploding Biotracker", "ExplosionChance", 0.05, "The chance for the biotracker to explode when you don't get above the threshold");
        explodeThreshold = ConfigFile.Bind("Exploding Biotracker", "ExplosionThreshold", 1, "The amount of enemies to be equal to or below for the chance to explode to be rolled");
        includeScouts = ConfigFile.Bind("Exploding Biotracker", "IncludeScouts", false, "Whether or not to include scouts when tallying the amount of enemies that were pinged.");
        willExplodeSelf = ConfigFile.Bind("Exploding Biotracker", "IsLethal", true,
            "Whether or not exploding will kill the player using the biotracker");
        includeTanks = ConfigFile.Bind("Exploding Biotracker", "IncludeTanks", true, "Whether or not to include tanks when tallying the amount of enemies");
        includePablo = ConfigFile.Bind("Exploding Biotracker", "IncludeImmortal", false, "Whether or not to include the Immortal/Pablo when tallying the amount of enemies");
        
        SafeFileSystemWatcher _ = SafeFileSystemWatcher.Create(ConfigFile);
    }

    public static ConfigSettings GetConfig()
    {
        return new ConfigSettings
        {
            chance = explodeChance.Value,
            threshold = explodeThreshold.Value,
            includeScouts = includeScouts.Value,
            includeTanks = includeTanks.Value,
            includePablo = includePablo.Value,
            isLethal = willExplodeSelf.Value,
        };
    }
}