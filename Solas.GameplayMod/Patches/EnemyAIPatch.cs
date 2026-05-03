using BaseMod.Core.Extensions;
using HarmonyLib;
using Il2Cpp;
using Solas.GameplayMod.Components;
using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;
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
            GameplayMod.Log.Warning($"{nameof(EnemyAIPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    static void EnemyAIStartPostfix(EnemyAI __instance)
    {
        __instance.AddModComponent<EnemyCharacterComponent>();
        __instance.AddModComponent<HealthComponent>();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.EndSex))]
    static void EnemyAIEndSexPostfix()
    {
        EnemyHPResetMod.Reset();
        ObeyToEnemyMod.Reset();
        SexDamageMod.ResetMod();
        SexInitiatorStateMod.ResetMod();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.EndSexDeath))]
    static void EnemyAIEndSexDeathPostfix()
    {
        EnemyHPResetMod.Reset();
        ObeyToEnemyMod.Reset();
        SexDamageMod.ResetMod();
        SexInitiatorStateMod.ResetMod();
    }
}
