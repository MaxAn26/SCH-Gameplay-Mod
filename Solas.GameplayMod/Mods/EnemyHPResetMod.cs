using System;

using BaseMod.Core.Extensions;

using BepInEx.Configuration;

using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class EnemyHPResetMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<int> MinHPPercent;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static bool IsActivated { get; set; }
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    #endregion

    internal static void Load( ConfigFile config ) {
        try {
            Enabled = config.Bind( nameof( EnemyHPResetMod ), nameof( Enabled ), false,
                new ConfigDescription( "Activates the modification", new AcceptableValueList<bool>( [true, false] ) ) );
            MinHPPercent = config.Bind( nameof( EnemyHPResetMod ), nameof( MinHPPercent ), 10,
                new ConfigDescription( "Minimum percentage of health from the maximum value that allows HP restoration to the maximum value", new AcceptableValueRange<int>( 1, 100 ) ) );

        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
        }
    }

    internal static void Apply( SexSystem sexSystem ) {
        try {
            if(!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            if(Character.statusDATA.IsBoundHeavyRestraint > 0)
                return;

            if(!IsActivated) {
                if(sexSystem.enemyHealthSystem.CurrentHp < 10 || sexSystem.enemyHealthSystem.CurrentHp <= sexSystem.enemyHealthSystem.MaxHp * (MinHPPercent.Value / 100))
                    return;

                var addHp = sexSystem.enemyHealthSystem.MaxHp - sexSystem.enemyHealthSystem.CurrentHp;
                sexSystem.enemyHealthSystem.AddHealth( addHp );

                IsActivated = true;
            }
        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
            return;
        }
    }

    internal static void Reset() {
        IsActivated = false;
    }
}