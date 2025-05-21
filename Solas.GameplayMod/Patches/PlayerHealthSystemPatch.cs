using System;

using BaseMod.Core.Extensions;

using HarmonyLib;

using Solas.GameplayMod.Mods;

using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Patches;
internal class PlayerHealthSystemPatch {
    internal static bool Prepare() {
        try {
            if(!SexDamageMod.IsModActive && !CapturedSlaveMod.IsModActive && !GlossEffectMod.IsModActive)
                return false;

            return true;
        } catch(Exception) {
            Plugin.Log.LogWarning( $"{nameof( PlayerHealthSystemPatch )} not applied due exeption" );
            return false;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( PlayerHealthSystem ), nameof( PlayerHealthSystem.ReduceMaxHP ) )]
    static bool PlayerHealthSystemReduceMaxHPPrefix( bool __runOriginal ) {
        if(!__runOriginal)
            return false;

        return !SexDamageMod.BlockReduceMaxHP();
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( PlayerHealthSystem ), nameof( PlayerHealthSystem.RestoreMaxHP ) )]
    static bool PlayerHealthSystemRestoreMaxHPPrefix( bool __runOriginal ) {
        if(!__runOriginal)
            return false;

        return !SexDamageMod.BlockRestoreMaxHP();
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( PlayerHealthSystem ), nameof( PlayerHealthSystem.UpdateArousal ) )]
    static bool PlayerHealthSystemUpdateArousalPrefix( bool __runOriginal, ref int __0, ref int __1 ) {
        if (SceneManager.GetActiveScene().buildIndex > 4 && SexSystem.Sexstatus is SEXSTATUS.Fucking && !SexSystem.IsCumming && SexDamageMod.IsModActive) {
            Plugin.Log.Info($"UpdateArousal: {__0}/{__1}");
            return false;
        }

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( PlayerHealthSystem ), nameof( PlayerHealthSystem.UpdateSpecial ) )]
    static bool PlayerHealthSystemUpdateSpecialPrefix( bool __runOriginal, ref int __0 ) {
        CapturedSlaveMod.UpdateSpecial( ref __0 );

        if(!__runOriginal)
            return false;

        return true;
    }
}