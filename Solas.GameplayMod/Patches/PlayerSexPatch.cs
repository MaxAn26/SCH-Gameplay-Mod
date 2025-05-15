using System;

using HarmonyLib;

using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;
internal class PlayerSexPatch {
    internal static bool Prepare() {
        try {
            if(!SexDamageMod.IsModActive && !SexInitiatorStateMod.IsModActive)
                return false;

            return true;
        } catch(Exception) {
            Plugin.Log.LogWarning( $"{nameof( PlayerSexPatch )} not applied due exeption" );
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch( typeof( PlayerSex ), nameof( PlayerSex.CumFX ) )]
    static void PlayerSexCumFXPostfix() {
        SexDamageMod.PlayerCum();
        SexInitiatorStateMod.Apply();
    }
}