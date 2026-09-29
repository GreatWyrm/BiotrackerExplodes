using Enemies;
using Gear;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;

namespace BiotrackerExplode;

[HarmonyPatch]
public class BiotrackerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyScanner), nameof(EnemyScanner.TryGetTaggableEnemies))]
    public static void Postfix(int maxTagAttempts, bool isOnAim, List<EnemyAgent> enemies)
    {
        if (enemies.Count == 0)
        {
            return;
        }
        
        Plugin.EvaluatePings(enemies);
    }
}