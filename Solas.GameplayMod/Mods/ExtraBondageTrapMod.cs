using System;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class ExtraBondageTrapMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<int> Chance;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    #region Storage
    internal static CharacterData.StatusData CharacterStatus => CharacterData.Instance.statusDATA;
    #endregion

    internal static void Load(ConfigFile config) {
        try {
            Enabled = config.Bind(nameof(ExtraBondageTrapMod), nameof(Enabled), false,
                new ConfigDescription("Activates the modification", new AcceptableValueList<bool>([true, false])));
            Chance = config.Bind(nameof(ExtraBondageTrapMod), nameof(Chance), 20,
                new ConfigDescription("Chance to put extra bondage or take damage", new AcceptableValueRange<int>(0, 100)));

        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void Apply(SexSystem sexSystem) {
        try {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            if(!RandomUtils.Chance(Chance.Value))
                return;

            if(CharacterStatus.IsBoundBlindfold != 0 && CharacterStatus.IsBoundCollar != 0 
                && CharacterStatus.IsBoundGag != 0 && CharacterStatus.IsBoundHandRestraint != 0
                && CharacterStatus.IsBoundHarness != 0 && CharacterStatus.IsBoundLegRestraint != 0
                && CharacterStatus.IsBoundNippleClamps != 0 && CharacterStatus.IsBoundPlug != 0 && CharacterStatus.IsBoundVibrator != 0) {
                int damage = Convert.ToInt32( Math.Round(sexSystem.playerHealthSystem.CurrentHp * 0.8));
                sexSystem.playerHealthSystem.SubstractHealth(damage);
            } else {
                sexSystem.BoundPlayer();
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return;
        }
    }
}