using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using BiotrackerExplode;
using Enemies;
using Il2CppSystem.Collections.Generic;
using Player;

[assembly: AssemblyVersion(Plugin.VERSION)]
[assembly: AssemblyFileVersion(Plugin.VERSION)]
[assembly: AssemblyInformationalVersion(Plugin.VERSION)]

namespace BiotrackerExplode;

[BepInPlugin(GUID, MOD_NAME, VERSION)]
[BepInDependency("dev.gtfomodding.gtfo-api")]
public class Plugin : BasePlugin
{
    public const string GUID = "dev.Giginss.gtfo.BiotrackerExplode";
    public const string MOD_NAME = ManifestInfo.TSName;
    public const string VERSION = ManifestInfo.TSVersion;

    internal static ManualLogSource L;
    
    private static readonly Harmony _harmony = new(GUID);
    private static ConfigEntry<double> explodeChance;
    private static ConfigEntry<int> explodeThreshold;
    private static ConfigEntry<bool> includeScouts;
    private static ConfigEntry<bool> willExplodeSelf;
    private static System.Random _random = new();
    
    
    public override void Load()
    {
        L = Log;

        explodeChance = Config.Bind("Exploding Biotracker", "ExplosionChance", 0.05, "The chance for the biotracker to explode when you don't get above the threshold");
        explodeThreshold = Config.Bind("Exploding Biotracker", "ExplosionThreshold", 1, "The amount of enemies to be equal to or below for the chance to explode to be rolled");
        includeScouts = Config.Bind("Exploding Biotracker", "IncludeScouts", false, "Whether or not to include scouts (when they're scouting) when tallying the amount of enemies that were pinged.");
        willExplodeSelf = Config.Bind("Exploding Biotracker", "IsLethal", true,
            "Whether or not exploding will kill the player using the biotracker");
        
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
        Networking.Init();
        
        L.LogInfo("Exploding biotracker loaded!");
    }

    public static void EvaluatePings(List<EnemyAgent> enemies)
    {
        L.LogInfo("Evaluating biotracker pings");
        
        int totalEnemies = enemies.Count;
        if (!includeScouts.Value)
        {
            int scouts = 0;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsScout && enemies[i].Locomotion.CurrentStateEnum == ES_StateEnum.ScoutDetection)
                {
                    scouts++;
                }
            }
            totalEnemies -= scouts;
        }

        if (totalEnemies <= 0)
        {
            return;
        }

        if (totalEnemies <= explodeThreshold.Value)
        {
            L.LogInfo("Below threshold!");
            // Uh oh!
            if (_random.NextDouble() < explodeChance.Value)
            {
                L.LogInfo("Rolled chance, exploding!");
                // Kaboom!
                var agent = PlayerManager.GetLocalPlayerAgent();
                if (agent == null)
                {
                    return;
                }
                Networking.BroadcastAndPlayToSelf(agent.m_position);
                if (willExplodeSelf.Value)
                {
                    agent.Damage.OnIncomingDamage(agent.Damage.Health + 1, agent.Damage.Health + 1);
                }
            }
        }
    }
}