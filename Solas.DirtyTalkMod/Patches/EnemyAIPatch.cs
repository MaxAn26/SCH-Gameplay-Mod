using System;

using HarmonyLib;

using Solas.DirtyTalkMod.Mods;

namespace Solas.DirtyTalkMod.Patches;
internal class EnemyAIPatch {
    internal static bool Prepare() {
        try {
            if (!EnemyDirtyTalkMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.LogWarning($"{nameof(EnemyAIPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    [HarmonyPriority(Priority.High)]
    static void EnemyAIStartPostfix(EnemyAI __instance) {
        EnemyDirtyTalkMod.ApplyEnemyDirtyTalk(__instance);
    }
}