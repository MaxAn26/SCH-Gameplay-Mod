using BaseMod.Core.Extensions;
using HarmonyLib;
using Il2Cpp;
using Solas.DirtyTalkMod.Components;
using Solas.DirtyTalkMod.Mods;

namespace Solas.DirtyTalkMod.Patches;

[HarmonyPatch(typeof(SexSystem))]
internal class SexSystemPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!EnemyDirtyTalkMod.IsModActive && !PlayerDirtyTalkMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(SexSystemPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BoundHandCuffs))]
    [HarmonyPatch(nameof(SexSystem.BoundLegCufffs))]
    [HarmonyPatch(nameof(SexSystem.BoundHarness))]
    [HarmonyPatch(nameof(SexSystem.BoundNippleClamp))]
    static void SexSystemBoundGeneralPostfix(SexSystem __instance)
    {
        if (__instance.Enemy is null)
        {
            return;
        }

        __instance.Enemy?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.BoundGeneral();
        __instance.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.BoundGeneral();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BoundCollar))]
    static void SexSystemBoundCollarPostfix(SexSystem __instance)
    {
        if (__instance.Enemy is null)
        {
            return;
        }

        __instance.Enemy?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.BoundCollar();
        __instance.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.BoundCollar();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BoundGag))]
    static void SexSystemBoundGagPostfix(SexSystem __instance)
    {
        if (__instance.Enemy is null)
        {
            return;
        }

        __instance.Enemy?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.BoundGag();
        __instance.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.BoundGag();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BoundVibrator))]
    static void SexSystemBoundVibratorPostfix(SexSystem __instance)
    {
        if (__instance.Enemy is null)
        {
            return;
        }

        __instance.Enemy?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.BoundVibrator();
        __instance.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.BoundVibrator();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BoundPlug))]
    static void SexSystemBoundPlugPostfix(SexSystem __instance)
    {
        if (__instance.Enemy is null)
        {
            return;
        }

        __instance.Enemy?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.BoundPlug();
        __instance.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.BoundPlug();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BoundBlindfold))]
    static void SexSystemBoundBlindfoldPostfix(SexSystem __instance)
    {
        if (__instance.Enemy is null)
        {
            return;
        }

        __instance.Enemy?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.BoundBlindfold();
        __instance.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.BoundBlindfold();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BoundHandRestraint))]
    static void SexSystemBoundHandRestraintPostfix(SexSystem __instance)
    {
        if (__instance.Enemy is null)
        {
            return;
        }

        __instance.Enemy?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.BoundHandRestraints();
        __instance.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.BoundHandRestraints();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BoundLegRestraint))]
    static void SexSystemBoundLegRestraintPostfix(SexSystem __instance)
    {
        if (__instance.Enemy is null)
        {
            return;
        }

        __instance.Enemy?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.BoundLegsRestraints();
        __instance.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.BoundLegsRestraints();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.GrappleBeforeSex))]
    static void SexSystemGrappleBeforeSexPostfix(SexSystem __instance) => __instance.Enemy?.GetComponentWithCast<EnemyDirtyTalkComponent>()?.EnemyGrapple();

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.PlayerGrappleBeforeSex))]
    static void SexSystemPlayerGrappleBeforeSexPostfix(SexSystem __instance) => __instance.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.PlayerGrapple();
}
