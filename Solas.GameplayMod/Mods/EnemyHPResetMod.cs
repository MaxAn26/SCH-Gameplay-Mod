using BaseMod.Core;
using Il2Cpp;
using MelonLoader;
using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class EnemyHPResetMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<int> MinHPPercent;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static bool IsActivated { get; set; }
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(EnemyHPResetMod), nameof(Enabled), false,
                "Activates the modification", new ModConfig.AcceptableValueList<bool>([true, false]));
            MinHPPercent = config.Entry(nameof(EnemyHPResetMod), nameof(MinHPPercent), 10,
                "Minimum percentage of health from the maximum value that allows HP restoration to the maximum value", new ModConfig.AcceptableValueRange<int>(1, 100));

        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
        }
    }

    internal static void Apply(SexSystem sexSystem)
    {
        try
        {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            if (Character.statusDATA.IsBoundHeavyRestraint > 0)
            {
                return;
            }

            if (!IsActivated)
            {
                if (sexSystem.enemyHealthSystem.CurrentHp < 10 || sexSystem.enemyHealthSystem.CurrentHp <= sexSystem.enemyHealthSystem.MaxHp * (MinHPPercent.Value / 100))
                {
                    return;
                }

                int addHp = sexSystem.enemyHealthSystem.MaxHp - sexSystem.enemyHealthSystem.CurrentHp;
                sexSystem.enemyHealthSystem.AddHealth(addHp);

                IsActivated = true;
            }
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
            return;
        }
    }

    internal static void Reset() => IsActivated = false;
}
