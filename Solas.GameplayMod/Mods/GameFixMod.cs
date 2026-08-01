using BaseMod.Core;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace Solas.GameplayMod.Mods;
internal class GameFixMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(GameFixMod), nameof(Enabled), false,
                "Activates the modification", new ModConfig.AcceptableValueList<bool>([true, false]));
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static void DickSetup(SexSystem sexSystem)
    {
        if (!Enabled.Value)
        {
            return;
        }

        if (SexSystem.Sexstatus is not SEXSTATUS.Fucking)
        {
            return;
        }

        GameObject player = sexSystem.Player;
        bool playerDickRequired = sexSystem.PlayerIsAttacking
            ? sexSystem.CasterActive || sexSystem.CasterMale || sexSystem.CasterFuta || sexSystem.CasterDickRequired
            : sexSystem.TargetActive || sexSystem.TargetMale || sexSystem.TargetFuta || sexSystem.TargetDickRequired;

        if (playerDickRequired)
        {
            player.GetComponent<PlayerSex>()?.Dick?.gameObject.SetActive(true);
        }

        GameObject enemy = sexSystem.Enemy;
        bool enemyDickRequired = sexSystem.PlayerIsAttacking
            ? sexSystem.TargetActive || sexSystem.TargetMale || sexSystem.TargetFuta || sexSystem.TargetDickRequired
            : sexSystem.CasterActive || sexSystem.CasterMale || sexSystem.CasterFuta || sexSystem.CasterDickRequired;

        if (enemyDickRequired)
        {
            enemy.GetComponent<EnemySex>()?.Dick?.gameObject.SetActive(true);
        }

        if (sexSystem.IsThreesome && sexSystem.Assist is not null)
        {
            GameObject assist = sexSystem.Assist;
            bool oralPlayer = sexSystem.PlayerIsAttacking ? SexSystem.SexIsOralCaster : SexSystem.SexIsOralTarget;
            bool assistDickRequired = sexSystem.AssistMale || sexSystem.AssistFuta || sexSystem.AssistDickRequired || oralPlayer;

            if (assistDickRequired)
            {
                assist.GetComponent<EnemySex>()?.Dick?.gameObject.SetActive(true);
            }
        }
    }
}
