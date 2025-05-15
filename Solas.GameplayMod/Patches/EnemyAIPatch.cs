using System;

using HarmonyLib;

using Solas.GameplayMod.Components;
using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;
internal class EnemyAIPatch {
    internal static bool Prepare() {
        try {
            if(!EnemyHPResetMod.IsModActive && !ObeyToEnemyMod.IsModActive && !SexDamageMod.IsModActive && !SexInitiatorStateMod.IsModActive)
                return false;

            return true;
        } catch(Exception) {
            Plugin.Log.LogWarning( $"{nameof( EnemyAIPatch )} not applied due exeption" );
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    static void EnemyAIStartPostfix(EnemyAI __instance) {
        EnemyCharacterComponent.RegisterClass(__instance);
        HealthComponent.RegisterClass(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( EnemyAI ), nameof( EnemyAI.EndSex ) )]
    static void EnemyAIEndSexPostfix() {
        EnemyHPResetMod.Reset();
        ObeyToEnemyMod.Reset();
        SexDamageMod.ResetMod();
        SexInitiatorStateMod.ResetMod();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( EnemyAI ), nameof( EnemyAI.EndSexDeath ) )]
    static void EnemyAIEndSexDeathPostfix() {
        EnemyHPResetMod.Reset();
        ObeyToEnemyMod.Reset();
        SexDamageMod.ResetMod();
        SexInitiatorStateMod.ResetMod();
    }
}