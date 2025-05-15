using System;

using HarmonyLib;

using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;
internal class EnemyActionsPatch {
    internal static bool Prepare() {
        try {
            if(!CriticalHitMod.IsModActive)
                return false;

            return true;
        } catch(Exception) {
            Plugin.Log.LogWarning( $"{nameof( EnemyActionsPatch )} not applied due exeption" );
            return false;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( EnemyActions ), nameof( EnemyActions.Hit ) )]
    static bool EnemyActionsHitPrefix( EnemyActions __instance, bool __runOriginal ) {
        CriticalHitMod.EnemyCriticalHit( __instance );

        if(!__runOriginal)
            return false;

        return true;
    }
}