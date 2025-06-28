using System;
using System.Collections;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Unity.IL2CPP.Utils.Collections;

using HarmonyLib;

using Solas.DirtyTalkMod.Components;
using Solas.DirtyTalkMod.Mods;

using UnityEngine;

namespace Solas.DirtyTalkMod.Patches;
internal class CaptureSystemPatch {
    internal static bool Prepare() {
        try {
            if (!EnemyDirtyTalkMod.IsModActive && !PlayerDirtyTalkMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.LogWarning($"{nameof(CaptureSystemPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CaptureSystem), nameof(CaptureSystem.Start))]
    static void CaptureSystemStartPostfix(CaptureSystem __instance) {
        #region fix init
        if (__instance.Player.TryGetComponentWithCast(out PlayerCombat playerCombat))
            PlayerDirtyTalkMod.ApplyPlayerDirtyTalk(playerCombat);

        if (__instance.Char1.TryGetComponentWithCast(out EnemyAI enemyAI1))
            EnemyDirtyTalkMod.ApplyEnemyDirtyTalk(enemyAI1);
        if (__instance.Char2.TryGetComponentWithCast(out EnemyAI enemyAI2))
            EnemyDirtyTalkMod.ApplyEnemyDirtyTalk(enemyAI2);
        if (__instance.Char3.TryGetComponentWithCast(out EnemyAI enemyAI3))
            EnemyDirtyTalkMod.ApplyEnemyDirtyTalk(enemyAI3);
        if (__instance.Char4.TryGetComponentWithCast(out EnemyAI enemyAI4))
            EnemyDirtyTalkMod.ApplyEnemyDirtyTalk(enemyAI4);
        #endregion

        __instance.StartCoroutine(CaptureDirtyTalkEnumerator(__instance).WrapToIl2Cpp());
    }

    static IEnumerator CaptureDirtyTalkEnumerator(CaptureSystem captureSystem) {
        yield return new WaitForSeconds(4f);
        if (SexSystem.Sexstatus is not SEXSTATUS.Fucking or SEXSTATUS.Grappled) {
            if (CharacterData.Instance.statusDATA.TimesCaptured == 0) {
                if (RandomUtils.Chance(50)) {
                    captureSystem.Char1?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.CaptureFirstTime();
                    captureSystem.Char3?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.CaptureFirstTime();
                } else {
                    captureSystem.Char2?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.CaptureFirstTime();
                    captureSystem.Char4?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.CaptureFirstTime();
                }

                captureSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.CaptureFirstTime();
            } else {
                if (RandomUtils.Chance(50)) {
                    captureSystem.Char1?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.CapturedAgainTime();
                    captureSystem.Char3?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.CapturedAgainTime();
                } else {
                    captureSystem.Char2?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.CapturedAgainTime();
                    captureSystem.Char4?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.CapturedAgainTime();
                }

                captureSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.CapturedAgainTime();
            }
        }
    }
}