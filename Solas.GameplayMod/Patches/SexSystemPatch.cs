using BaseMod.Core.Enums;

using HarmonyLib;
using Il2Cpp;
using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;

[HarmonyPatch(typeof(SexSystem))]
internal class SexSystemPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!FuckMeMod.IsModActive && !EnemySexExtendMod.IsModActive && !ExtraBondageTrapMod.IsModActive && !EnemyHPResetMod.IsModActive 
                && !LustCageMod.IsModActive && !ObeyToEnemyMod.IsModActive && !SexDamageMod.IsModActive && !RandomEnemyRoleMod.IsModActive 
                && !SexMoveChoiceMod.IsModActive && !SexInitiatorStateMod.IsModActive && !RandomReverseMod.IsModActive && !GameFixMod.IsModActive)
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

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BoundPlayer))]
    static bool SexSystemBoundPlayerPrefix(SexSystem __instance, bool __runOriginal)
    {
        if (OrgasmControlMod.ApplyPunishmentOrgasm(__instance))
        {
            return false;
        }

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BoundPlayerTrap))]
    static void SexSystemBoundPlayerTrapPostfix(SexSystem __instance) => ExtraBondageTrapMod.Apply(__instance);

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.BreakRestraint))]
    static void SexSystemBreakRestraintPostfix(SexSystem __instance) => OrgasmControlMod.BreakPunishmentOrgasm(__instance);

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.CallbonusDom))]
    static bool SexSystemCallbonusDomPrefix(bool __runOriginal)
    {
        if (__runOriginal && ObeyToEnemyMod.IsActivated)
        {
            __runOriginal = false;
        }

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.CallBonusEsc))]
    static bool SexSystemCallBonusEscPrefix(SexSystem __instance, bool __runOriginal)
    {
        if (__runOriginal && LustCageMod.BlockEscape(__instance))
        {
            __runOriginal = false;
        }

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.InitializeSexSystem))]
    static void SexSystemInitializeSexSystemPostfix(SexSystem __instance) => FuckMeMod.Apply(__instance);

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.SetSexAnimation))]
    static bool SexSystemSetSexAnimationPrefix(bool __runOriginal)
    {
        SexMoveChoiceMod.SetSexID();

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.SetSexAnimation))]
    static void SexSystemSetSexAnimationPostfix(SexSystem __instance)
    {
        EnemySexExtendMod.ReduceEnemyPower(__instance);
        OrgasmControlMod.ApplySexControl(__instance);
        RandomReverseMod.Apply(__instance);
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.SetThreesomeAnimation))]
    static bool SexSystemSetThreesomeAnimationPrefix(bool __runOriginal)
    {
        SexMoveChoiceMod.SetSexID();

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.SetThreesomeAnimation))]
    static void SexSystemSetThreesomeAnimationPostfix(SexSystem __instance)
    {
        OrgasmControlMod.ApplySexControl(__instance);
        RandomReverseMod.Apply(__instance);
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.SetHeavyBondageAnimation))]
    static bool SexSystemSetHeavyBondageAnimationPrefix(bool __runOriginal)
    {
        SexMoveChoiceMod.SetSexID();

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.SetHeavyBondageAnimation))]
    static void SexSystemSetHeavyBondageAnimationPostfix(SexSystem __instance) => OrgasmControlMod.ApplySexControl(__instance);

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.Setup))]
    static bool SexSystemSetupPrefix(SexSystem __instance, bool __runOriginal)
    {
        SexInitiatorStateMod.SetInitiator(__instance);
        OrgasmControlMod.ResetSelfControl();
        EnemySexExtendMod.UpdateThreesome(__instance);

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.Setup))]
    static void SexSystemSetupPostfix(SexSystem __instance)
    {
        SexMoveChoiceMod.SexSystem = __instance;

        EnemyHPResetMod.Apply(__instance);
        RandomEnemyRoleMod.Apply(__instance);
        SexDamageMod.UpdateSexSystem(__instance);
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.SexDamage))]
    static bool SexSystemSexDamagePrefix(SexSystem __instance, bool __runOriginal)
    {
        SexDamageMod.IsSexDamage = true;
        SexDamageMod.SexDamageSavePlayerEc = __instance.playerHealthSystem.CurrentEc;

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.StartBukkake))]
    static void SexSystemStartBukkakePostfix()
    {
        SexDamageMod.ResetStates();
        SexDamageMod.SexInteraction = SexInteraction.Bukkake;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.StartMasturbation))]
    static void SexSystemStartMasturbationPostfix()
    {
        SexDamageMod.ResetStates();
        SexDamageMod.SexInteraction = SexInteraction.Masturbation;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.StartSex))]
    static void SexSystemStartSexPostfix(SexSystem __instance)
    {
        SexDamageMod.ResetStates();
        SexDamageMod.SexInteraction = SexInteraction.Sex;
        ObeyToEnemyMod.Apply(__instance);
        LustCageMod.Apply(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.StartThreesome))]
    static void SexSystemStartThreesomePostfix(SexSystem __instance)
    {
        SexDamageMod.ResetStates();
        SexDamageMod.SexInteraction = SexInteraction.Threesome;
        ObeyToEnemyMod.Apply(__instance);
        LustCageMod.Apply(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexSystem.SetDicks))]
    static void SexSystemSetDicksPostfix(SexSystem __instance) => GameFixMod.DickSetup(__instance);
}
