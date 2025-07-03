using System;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class RandomReverseMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<bool> IgnoreAtSameRoles;
    internal static ConfigEntry<bool> WhenPlayerWeak;
    internal static ConfigEntry<bool> WhenPlayerCollared;
    internal static ConfigEntry<int> Chance;
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
            Enabled = config.Bind( nameof( RandomReverseMod ), nameof( Enabled ), false,
                new ConfigDescription( "Activates the modification", new AcceptableValueList<bool>( [true, false] ) ) );
            IgnoreAtSameRoles = config.Bind( nameof( RandomReverseMod ), nameof( IgnoreAtSameRoles ), true,
                new ConfigDescription( "Do not activate when the enemy's role matches the player's role", new AcceptableValueList<bool>( [true, false] ) ) );
            WhenPlayerWeak = config.Bind( nameof( RandomReverseMod ), nameof( WhenPlayerWeak ), true,
                new ConfigDescription( "Activate only if the player is weak to the current sexual position", new AcceptableValueList<bool>( [true, false] ) ) );
            WhenPlayerCollared = config.Bind( nameof( RandomReverseMod ), nameof( WhenPlayerCollared ), true,
                new ConfigDescription( "Activate only if the player wears a collar", new AcceptableValueList<bool>( [true, false] ) ) );
            Chance = config.Bind( nameof( RandomReverseMod ), nameof( Chance ), 20,
                new ConfigDescription( "Chance for animation reversal", new AcceptableValueRange<int>( 0, 100 ) ) );

        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
        }
    }

    internal static void Apply( SexSystem sexSystem ) {
        try {
            if(!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            if(Character.statusDATA.IsBoundHeavyRestraint > 0 || SexSystem.GameOver)
                return;

            if(IgnoreAtSameRoles.Value && sexSystem.CasterActive == sexSystem.TargetActive)
                return;

            if(WhenPlayerWeak.Value && !sexSystem.playerweak)
                return;

            if(WhenPlayerCollared.Value && Character.statusDATA.IsBoundCollar == 0)
                return;

            if(RandomUtils.Chance( Chance.Value )) {
                SexSystem.ReverseMode = !SexSystem.ReverseMode;
                IsActivated = true;
            } else {
                IsActivated = false;
            }    

            return;
        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
            return;
        }
    }
}