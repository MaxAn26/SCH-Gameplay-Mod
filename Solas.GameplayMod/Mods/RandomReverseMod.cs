using BaseMod.Core;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using UnityEngine.SceneManagement;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class RandomReverseMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> IgnoreAtSameRoles;
    internal static MelonPreferences_Entry<bool> WhenPlayerWeak;
    internal static MelonPreferences_Entry<bool> WhenPlayerCollared;
    internal static MelonPreferences_Entry<int> Chance;
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
            Enabled = config.Entry(nameof(RandomReverseMod), nameof(Enabled), false,
                "Activates the modification", new AcceptableValueList<bool>([true, false]));
            IgnoreAtSameRoles = config.Entry(nameof(RandomReverseMod), nameof(IgnoreAtSameRoles), true,
                "Do not activate when the enemy's role matches the player's role", new AcceptableValueList<bool>([true, false]));
            WhenPlayerWeak = config.Entry(nameof(RandomReverseMod), nameof(WhenPlayerWeak), true,
                "Activate only if the player is weak to the current sexual position", new AcceptableValueList<bool>([true, false]));
            WhenPlayerCollared = config.Entry(nameof(RandomReverseMod), nameof(WhenPlayerCollared), true,
                "Activate only if the player wears a collar", new AcceptableValueList<bool>([true, false]));
            Chance = config.Entry(nameof(RandomReverseMod), nameof(Chance), 20,
                "Chance for animation reversal", new AcceptableValueRange<int>(0, 100));

        }
        catch (Exception ex)
        {
            Core.LogError(ex);
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

            if (Character.statusDATA.IsBoundHeavyRestraint > 0 || SexSystem.GameOver)
            {
                return;
            }

            if (IgnoreAtSameRoles.Value && sexSystem.CasterActive == sexSystem.TargetActive)
            {
                return;
            }

            if (WhenPlayerWeak.Value && !sexSystem.playerweak)
            {
                return;
            }

            if (WhenPlayerCollared.Value && Character.statusDATA.IsBoundCollar == 0)
            {
                return;
            }

            if (RandomUtils.Chance(Chance.Value))
            {
                SexSystem.ReverseMode = !SexSystem.ReverseMode;
                IsActivated = true;
            }
            else
            {
                IsActivated = false;
            }

            return;
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
            return;
        }
    }
}
