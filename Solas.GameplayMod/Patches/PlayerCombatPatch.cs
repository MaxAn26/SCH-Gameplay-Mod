using System;

using HarmonyLib;

using Solas.GameplayMod.Components;
using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;
internal class PlayerCombatPatch {
    internal static bool Prepare() {
        try {
            if(!CriticalHitMod.IsModActive && !SexDamageMod.IsModActive && !SexInitiatorStateMod.IsModActive)
                return false;

            return true;
        } catch(Exception) {
            Plugin.Log.LogWarning( $"{nameof( PlayerCombatPatch )} not applied due exeption" );
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(PlayerCombat), nameof(PlayerCombat.Start))]
    static void PlayerCombatStartPostfix(PlayerCombat __instance) {
        PlayerCharacterComponent.RegisterClass(__instance);
        HealthComponent.RegisterClass(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( PlayerCombat ), nameof( PlayerCombat.Death ) )]
    static void PlayerCombatDeathPostfix() {
        SexDamageMod.ResetMod();
        SexInitiatorStateMod.ResetMod();
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(PlayerCombat), nameof(PlayerCombat.Hit))]
    static bool PlayerCombatHitPrefix(PlayerCombat __instance, bool __runOriginal) {
        CriticalHitMod.PlayerCriticalHit(__instance);

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(PlayerCombat), nameof(PlayerCombat.GUNHit))]
    static bool PlayerCombatGUNHitPrefix(PlayerCombat __instance, bool __runOriginal) {
        CriticalHitMod.PlayerCriticalHit(__instance);

        if (!__runOriginal)
            return false;

        return true;
    }
}