using HarmonyLib;
using Il2Cpp;
using Solas.DirtyTalkMod.Mods;

namespace Solas.DirtyTalkMod.Patches;

[HarmonyPatch(typeof(EnemyAI))]
internal class EnemyAIPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!EnemyDirtyTalkMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(EnemyAIPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(EnemyAI.Start))]
    [HarmonyPriority(Priority.High)]
    static void EnemyAIStartPostfix(EnemyAI __instance) => EnemyDirtyTalkMod.ApplyEnemyDirtyTalk(__instance);
}
