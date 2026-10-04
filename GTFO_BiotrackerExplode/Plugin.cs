using System.Reflection;
using BepInEx;
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
    private static System.Random _random = new();
    
    
    public override void Load()
    {
        L = Log;
        
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
        Networking.Init();
        PluginConfig.Init();
        
        L.LogInfo("Exploding biotracker loaded!");
    }

    public static void EvaluatePings(List<EnemyAgent> enemies)
    {
        L.LogDebug("Evaluating biotracker pings");

        PluginConfig.ConfigSettings settings = PluginConfig.GetConfig();

        int totalEnemies = enemies.Count;
        int toSubtract = 0;
        
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsScout && !settings.includeScouts)
            {
                toSubtract++;
            }

            if (enemies[i].EnemyDataID == 29 && !settings.includeTanks)
            {
                toSubtract++;
            }
            
            if (enemies[i].EnemyDataID == 47 && !settings.includePablo)
            {
                toSubtract++;
            }
        }
        
        totalEnemies -= toSubtract;

        if (totalEnemies <= 0)
        {
            return;
        }

        if (totalEnemies > settings.threshold)
        {
            return;
        }
        
        L.LogDebug("Below threshold!");
        // Uh oh!
        if (_random.NextDouble() < settings.chance)
        {
            L.LogDebug("Rolled chance, exploding!");
            // Kaboom!
            var agent = PlayerManager.GetLocalPlayerAgent();
            if (agent == null)
            {
                return;
            }
            Networking.BroadcastAndPlayToSelf(agent.m_position);
            if (settings.isLethal)
            {
                agent.Damage.OnIncomingDamage(agent.Damage.Health + 1, agent.Damage.Health + 1);
            }
        }
    }
}