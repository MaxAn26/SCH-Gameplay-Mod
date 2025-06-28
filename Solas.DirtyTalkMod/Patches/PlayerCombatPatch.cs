using System;

using HarmonyLib;

using Solas.DirtyTalkMod.Components;
using Solas.DirtyTalkMod.Mods;

namespace Solas.DirtyTalkMod.Patches;
internal class PlayerCombatPatch {
    internal static bool Prepare() {
        try {
            if (!PlayerDirtyTalkMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.LogWarning($"{nameof(PlayerCombatPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(PlayerCombat), nameof(PlayerCombat.Start))]
    [HarmonyPriority(Priority.High)]
    static void PlayerCombatStartPostfix(PlayerCombat __instance) {
        MainDirtyTalkComponent.RegisterClass(__instance);
        PlayerDirtyTalkMod.ApplyPlayerDirtyTalk(__instance);
    }
}