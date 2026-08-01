using BaseMod.Core.Extensions;
using HarmonyLib;
using Il2Cpp;
using Solas.GameplayMod.Components;
using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;
[HarmonyPatch(typeof(EnemyAI))]
internal class EnemyAIPatch
{
    internal static bool Prepare()
    {
        try
        {
            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(EnemyAIPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(EnemyAI.Start))]
    static void EnemyAIStartPostfix(EnemyAI __instance)
    {
        __instance.AddModComponent<EnemyCharacterComponent>();
        __instance.AddModComponent<HealthComponent>();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(EnemyAI.EndSex))]
    static void EnemyAIEndSexPostfix()
    {
        EnemyHPResetMod.Reset();
        ObeyToEnemyMod.Reset();
        SexDamageMod.ResetMod();
        SexInitiatorStateMod.ResetMod();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(EnemyAI.EndSexDeath))]
    static void EnemyAIEndSexDeathPostfix()
    {
        EnemyHPResetMod.Reset();
        ObeyToEnemyMod.Reset();
        SexDamageMod.ResetMod();
        SexInitiatorStateMod.ResetMod();
    }
}
