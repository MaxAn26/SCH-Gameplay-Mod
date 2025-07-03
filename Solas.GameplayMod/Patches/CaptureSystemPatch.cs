using System;

using HarmonyLib;

using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;
internal class CaptureSystemPatch {
    internal static bool Prepare() {
        try {
            if(!CapturedSlaveMod.IsModActive)
                return false;

            return true;
        } catch(Exception) {
            Plugin.Log.LogWarning( $"{nameof( CaptureSystemPatch )} not applied due exeption" );
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( CaptureSystem ), nameof( CaptureSystem.NextSex ) )]
    static void CaptureSystemNextSexPostfix() {
        CapturedSlaveMod.NextEnemy();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( CaptureSystem ), nameof( CaptureSystem.Start ) )]
    static void CaptureSystemStartPostfix( CaptureSystem __instance ) {
        CapturedSlaveMod.Apply( __instance );
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( CaptureSystem ), nameof( CaptureSystem.StopSex ) )]
    static void CaptureSystemStopSexPostfix() {
        CapturedSlaveMod.Reset();
    }
}