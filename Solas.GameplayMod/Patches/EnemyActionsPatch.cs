using HarmonyLib;
using Il2Cpp;
using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;

[HarmonyPatch(typeof(EnemyActions))]
internal class EnemyActionsPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!CriticalHitMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(EnemyActionsPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(EnemyActions.Hit))]
    static bool EnemyActionsHitPrefix(EnemyActions __instance, bool __runOriginal)
    {
        CriticalHitMod.EnemyCriticalHit(__instance);

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }
}
