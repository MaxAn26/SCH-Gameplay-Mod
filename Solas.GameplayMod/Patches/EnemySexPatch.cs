using System;

using HarmonyLib;

using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;
internal class EnemySexPatch {
    internal static bool Prepare() {
        try {
            if(!SexDamageMod.IsModActive && !SexInitiatorStateMod.IsModActive && !RandomFutaMod.IsModActive)
                return false;

            return true;
        } catch(Exception) {
            Plugin.Log.LogWarning( $"{nameof( EnemySexPatch )} not applied due exeption" );
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(EnemySex), nameof(EnemySex.Start))]
    static void EnemySexStartPostfix(EnemySex __instance) {
        RandomFutaMod.Apply(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( EnemySex ), nameof( EnemySex.CumFX ) )]
    static void EnemySexCumFXPostfix( EnemySex __instance ) {
        SexInitiatorStateMod.Apply();
        if(!__instance.IsAssist)             SexDamageMod.EnemyCum();
    }
}