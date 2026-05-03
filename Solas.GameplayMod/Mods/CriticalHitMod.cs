using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class CriticalHitMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(CriticalHitMod), nameof(Enabled), false,
                "Activates the modification", new ModConfig.AcceptableValueList<bool>([true, false]));

        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
        }
    }

    internal static void PlayerCriticalHit(PlayerCombat playerCombat)
    {
        try
        {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            int chance = 5;

            if (Character.statusDATA.IsBoundVibrator > 0)
            {
                chance += 5;
            }

            if (Character.statusDATA.IsBoundBlindfold > 0)
            {
                chance += 5;
            }

            if (RandomUtils.Chance(chance))
            {
                GameplayMod.Log.Msg($"Player critical chance: {chance}");
                playerCombat.Sexscript.console.ConsoleWrite("Critical hit!");
                playerCombat.healthSystem.SubstractHealth(playerCombat.healthSystem.CurrentHp - 2);
            }

            return;
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex);
            return;
        }
    }

    internal static void EnemyCriticalHit(EnemyActions enemyActions)
    {
        try
        {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            if (!enemyActions.gameObject.TryGetComponentWithCast(out EnemyAI enemyAI))
            {
                return;
            }

            double chance = 0.01;
            int levelDiff = Character.statusDATA.Level / 2 - enemyAI.EnSex.Level;
            chance += 0.001 * levelDiff;

            if (enemyAI.IsBoss)
            {
                chance *= 0.5;
            }

            if (chance < 0.0)
            {
                chance = 0.0;
            }

            if (RandomUtils.Chance(chance))
            {
                GameplayMod.Log.Msg($"Enemy critical chance: {chance}");
                enemyAI.Sexscript.console.ConsoleWrite($"{enemyAI.enemyName} get critical hit!");
                enemyActions.healthSystem.SubstractHealth(enemyActions.healthSystem.CurrentHp - 2);
            }

            return;
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex);
            return;
        }
    }
}
