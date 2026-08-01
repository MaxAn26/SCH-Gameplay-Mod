using BaseMod.Core.Extensions;
using HarmonyLib;
using Il2Cpp;
using Solas.DirtyTalkMod.Components;
using Solas.DirtyTalkMod.Mods;

namespace Solas.DirtyTalkMod.Patches;

[HarmonyPatch(typeof(PlayerCombat))]
internal class PlayerCombatPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!PlayerDirtyTalkMod.IsModActive)
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
    [HarmonyPriority(Priority.High)]
    static void PlayerCombatStartPostfix(PlayerCombat __instance)
    {

        __instance.AddModComponent<MainDirtyTalkComponent>();
        PlayerDirtyTalkMod.ApplyPlayerDirtyTalk(__instance);
    }
}
