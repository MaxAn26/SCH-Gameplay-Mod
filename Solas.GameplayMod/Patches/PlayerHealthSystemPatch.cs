using HarmonyLib;
using Il2Cpp;
using Solas.GameplayMod.Mods;

using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Patches;

[HarmonyPatch(typeof(PlayerHealthSystem))]
internal class PlayerHealthSystemPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!SexDamageMod.IsModActive && !CapturedSlaveMod.IsModActive && !GlossEffectMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(PlayerHealthSystemPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(PlayerHealthSystem.ReduceMaxHP))]
    static bool PlayerHealthSystemReduceMaxHPPrefix(bool __runOriginal)
    {
        if (!__runOriginal)
        {
            return false;
        }

        return !SexDamageMod.BlockReduceMaxHP();
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(PlayerHealthSystem.RestoreMaxHP))]
    static bool PlayerHealthSystemRestoreMaxHPPrefix(bool __runOriginal)
    {
        if (!__runOriginal)
        {
            return false;
        }

        return !SexDamageMod.BlockRestoreMaxHP();
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(PlayerHealthSystem.UpdateArousal))]
    static bool PlayerHealthSystemUpdateArousalPrefix(PlayerHealthSystem __instance, bool __runOriginal, ref int __1)
    {
        if (SceneManager.GetActiveScene().buildIndex > 4 && !SexSystem.PlayerAttacker && SexSystem.Sexstatus is SEXSTATUS.Fucking && !SexSystem.IsCumming)
        {
            OrgasmControlMod.CheckOrgasmControl(__instance.CurrentEc, __instance.MaxEc, ref __1);
        }

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(PlayerHealthSystem.UpdateSpecial))]
    static bool PlayerHealthSystemUpdateSpecialPrefix(bool __runOriginal, ref int __0)
    {
        CapturedSlaveMod.UpdateSpecial(ref __0);

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }
}
