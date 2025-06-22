using System;

using HarmonyLib;

using Solas.GameplayMod.Mods;

using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Patches;
internal class HealthSystemPatch {
    internal static bool Prepare() {
        try {
            if(!SexMoveChoiceMod.IsModActive && !SexDamageMod.IsModActive && !GlossEffectMod.IsModActive)
                return false;

            return true;
        } catch(Exception) {
            Plugin.Log.LogWarning( $"{nameof( HealthSystemPatch )} not applied due exception" );
            return false;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( HealthSystem ), nameof( HealthSystem.IncreaseEc ) )]
    static bool HealthSystemIncreaseEcPrefix(HealthSystem __instance, bool __runOriginal, ref int __0) {
        EnemyTraitsMod.ArousalDamage(__instance, ref __0);
        EnemySexExtendMod.CheckFetishAndWeaknesses(__instance, ref __0);
        if (SceneManager.GetActiveScene().buildIndex > 4 && SexSystem.PlayerAttacker && SexSystem.Sexstatus is SEXSTATUS.Fucking && !SexSystem.IsCumming) {
            OrgasmControlMod.CheckOrgasmControl(__instance.CurrentEc, __instance.MaxEc, ref __0);
        }

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( HealthSystem ), nameof( HealthSystem.SubstractHealth ) )]
    static bool HealthSystemSubstractHealthPrefix(HealthSystem __instance, bool __runOriginal, ref int __0 ) {
        EnemyTraitsMod.HealthDamage(__instance, ref __0);

        if (!__runOriginal)
            return false;

        return true;
    }
}