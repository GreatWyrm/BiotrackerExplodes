using Gear;
using HarmonyLib;

namespace BiotrackerExplode;

[HarmonyPatch]
public class BiotrackerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyScanner), nameof(EnemyScanner.UpdateTagProgress))]
    public static void Postfix(EnemyScanner __instance, int maxTags)
    {
        if (__instance.m_lastTagging && !__instance.m_tagging && !__instance.m_lastRecharging &&
            __instance.m_recharging)
        {
            // Biotracker has tagged
            Plugin.EvaluatePings(__instance.m_taggableEnemies);
        }
    }
}