using BaseMod.Core;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class CapturedSlaveMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<int> InteractionCount;
    internal static MelonPreferences_Entry<bool> RandomCount;
    internal static MelonPreferences_Entry<int> MaxInteractions;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static bool IsActivated { get; set; }

    internal static int EnemyDone { get; set; } = 0;
    internal static int Step { get; set; } = 0;

    internal static int Limit { get; set; } = 0;
    #endregion

    #region Storage
    internal static CharacterData CharacterData => CharacterData.Instance;
    internal static CaptureSystem CaptureSystem;
    internal static PlayerHealthSystem PlayerHealthSystem;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(CapturedSlaveMod), nameof(Enabled), false,
                "Activates the modification", new ModConfig.AcceptableValueList<bool>([true, false]));
            InteractionCount = config.Entry(nameof(CapturedSlaveMod), nameof(InteractionCount), 4,
                "Count of sex interactions required for escape", new ModConfig.AcceptableValueRange<int>(1, 50));
            RandomCount = config.Entry(nameof(CapturedSlaveMod), nameof(RandomCount), true,
                "Use a random count of sex interactions for escape", new ModConfig.AcceptableValueList<bool>([true, false]));
            MaxInteractions = config.Entry(nameof(CapturedSlaveMod), nameof(MaxInteractions), 5,
                "Maximum count of sex interactions for escape when using RandomCount", new ModConfig.AcceptableValueRange<int>(1, 50));

        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
        }
    }

    internal static void Apply(CaptureSystem captureSystem)
    {
        try
        {
            if (!Enabled.Value || !CharacterData.adultSettingsDATA.NSFWMode || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            if (!IsActivated)
            {
                CaptureSystem = captureSystem;
                PlayerHealthSystem = CaptureSystem.sexsystem.playerHealthSystem;

                int interactions = RandomCount.Value ? RandomUtils.Int32(1, MaxInteractions.Value) : InteractionCount.Value;

                Step = PlayerHealthSystem.MaxSP / interactions;
                Limit = Step;
                EnemyDone = 1;

                IsActivated = true;
            }

            return;
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex);
            return;
        }
    }

    internal static void NextEnemy()
    {
        try
        {
            if (!IsActivated)
            {
                return;
            }

            Limit += Step;
            if (Limit > PlayerHealthSystem.MaxSP)
            {
                Limit = PlayerHealthSystem.MaxSP;
            }
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex);
            return;
        }
    }

    internal static void UpdateSpecial(ref int special)
    {
        try
        {
            if (!IsActivated)
            {
                return;
            }

            if (special < Limit)
            {
                return;
            }

            PlayerHealthSystem.CurrentSP = Limit;
            special = PlayerHealthSystem.CurrentSP;
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex);
        }
    }

    internal static void Reset()
    {
        CaptureSystem = null;
        PlayerHealthSystem = null;
        IsActivated = false;
    }
}
