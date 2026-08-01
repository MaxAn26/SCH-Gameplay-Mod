using BaseMod.Core.Extensions;
using HarmonyLib;
using Il2Cpp;
using Solas.GameplayMod.Components;
using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;

[HarmonyPatch(typeof(PlayerCombat))]
internal class PlayerCombatPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!CriticalHitMod.IsModActive && !SexDamageMod.IsModActive && !SexInitiatorStateMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(PlayerCombatPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(PlayerCombat.Start))]
    static void PlayerCombatStartPostfix(PlayerCombat __instance)
    {
        __instance.AddModComponent<PlayerCharacterComponent>();
        __instance.AddModComponent<HealthComponent>();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(PlayerCombat.Death))]
    static void PlayerCombatDeathPostfix()
    {
        SexDamageMod.ResetMod();
        SexInitiatorStateMod.ResetMod();
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(PlayerCombat.Hit))]
    static bool PlayerCombatHitPrefix(PlayerCombat __instance, bool __runOriginal)
    {
        CriticalHitMod.PlayerCriticalHit(__instance);

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(PlayerCombat.GUNHit))]
    static bool PlayerCombatGUNHitPrefix(PlayerCombat __instance, bool __runOriginal)
    {
        CriticalHitMod.PlayerCriticalHit(__instance);

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }
}
