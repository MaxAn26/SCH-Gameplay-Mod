using HarmonyLib;
using Il2Cpp;
using Solas.GameplayMod.Mods;

namespace Solas.GameplayMod.Patches;
[HarmonyPatch(typeof(CaptureSystem))]
internal class CaptureSystemPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!CapturedSlaveMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(CaptureSystemPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(CaptureSystem.NextSex))]
    static void CaptureSystemNextSexPostfix() => CapturedSlaveMod.NextEnemy();

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(CaptureSystem.Start))]
    static void CaptureSystemStartPostfix(CaptureSystem __instance) => CapturedSlaveMod.Apply(__instance);

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(CaptureSystem.StopSex))]
    static void CaptureSystemStopSexPostfix() => CapturedSlaveMod.Reset();
}
