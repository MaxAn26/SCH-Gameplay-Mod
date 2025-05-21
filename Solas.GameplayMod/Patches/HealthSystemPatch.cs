using System;

using BaseMod.Core.Extensions;

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
    static bool HealthSystemIncreaseEcPrefix( HealthSystem __instance, bool __runOriginal, ref int __0 ) {
        if(SceneManager.GetActiveScene().buildIndex > 4 && SexSystem.Sexstatus is SEXSTATUS.Fucking && SexDamageMod.IsModActive){
            return false;
        }

        if(!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( HealthSystem ), nameof( HealthSystem.SubstractHealth ) )]
    static bool HealthSystemSubstractHealthPrefix( HealthSystem __instance, bool __runOriginal, ref int __0 ) {
        if (SceneManager.GetActiveScene().buildIndex > 4 && SexSystem.Sexstatus is SEXSTATUS.Fucking && SexDamageMod.IsModActive) {
            return false;
        }

        if (!__runOriginal)
            return false;

        return __0 != 0;
    }
}