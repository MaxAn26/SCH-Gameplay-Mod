using BaseMod.Core;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using UnityEngine.SceneManagement;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class ExtraBondageTrapMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<int> Chance;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    #region Storage
    internal static CharacterData.StatusData CharacterStatus => CharacterData.Instance.statusDATA;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(ExtraBondageTrapMod), nameof(Enabled), false,
                "Activates the modification", new AcceptableValueList<bool>([true, false]));
            Chance = config.Entry(nameof(ExtraBondageTrapMod), nameof(Chance), 20,
                "Chance to put extra bondage or take damage", new AcceptableValueRange<int>(0, 100));

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

            if (!RandomUtils.Chance(Chance.Value))
            {
                return;
            }

            if (CharacterStatus.IsBoundBlindfold != 0 && CharacterStatus.IsBoundCollar != 0
                && CharacterStatus.IsBoundGag != 0 && CharacterStatus.IsBoundHandRestraint != 0
                && CharacterStatus.IsBoundHarness != 0 && CharacterStatus.IsBoundLegRestraint != 0
                && CharacterStatus.IsBoundNippleClamps != 0 && CharacterStatus.IsBoundPlug != 0 && CharacterStatus.IsBoundVibrator != 0)
            {
                int damage = Convert.ToInt32(Math.Round(sexSystem.playerHealthSystem.CurrentHp * 0.8));
                sexSystem.playerHealthSystem.SubstractHealth(damage);
            }
            else
            {
                sexSystem.BoundPlayer();
            }
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
            return;
        }
    }
}
