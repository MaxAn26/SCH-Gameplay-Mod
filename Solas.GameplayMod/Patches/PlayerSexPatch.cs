using HarmonyLib;
using Il2Cpp;
using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;

[HarmonyPatch(typeof(PlayerSex))]
internal class PlayerSexPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!SexDamageMod.IsModActive && !SexInitiatorStateMod.IsModActive && !SexMoveChoiceMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(PlayerSexPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(PlayerSex.CumFX))]
    static void PlayerSexCumFXPostfix()
    {
        SexDamageMod.PlayerCum();
        SexInitiatorStateMod.Apply();
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(PlayerSex.Escape))]
    static void PlayerSexEscapePostfix() => SexMoveChoiceMod.InteractionCounts = 0;
}
