using BaseMod.Core.Extensions;
using HarmonyLib;
using Il2Cpp;
using Solas.DirtyTalkMod.Components;
using Solas.DirtyTalkMod.Mods;

namespace Solas.DirtyTalkMod.Patches;
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
            DirtyTalkMod.Log.Warning($"{nameof(PlayerCombatPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(PlayerCombat), nameof(PlayerCombat.Start))]
    [HarmonyPriority(Priority.High)]
    static void PlayerCombatStartPostfix(PlayerCombat __instance)
    {

        __instance.AddModComponent<MainDirtyTalkComponent>();
        PlayerDirtyTalkMod.ApplyPlayerDirtyTalk(__instance);
    }
}