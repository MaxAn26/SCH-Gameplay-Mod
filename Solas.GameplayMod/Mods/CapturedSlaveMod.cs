using System;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class CapturedSlaveMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<int> InteractionCount;
    internal static ConfigEntry<bool> RandomCount;
    internal static ConfigEntry<int> MaxInteractions;
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

    internal static void Load( ConfigFile config ) {
        try {
            Enabled = config.Bind( nameof( CapturedSlaveMod ), nameof( Enabled ), false,
                new ConfigDescription( "Activates the modification", new AcceptableValueList<bool>( [true, false] ) ) );
            InteractionCount = config.Bind( nameof( CapturedSlaveMod ), nameof( InteractionCount ), 4,
                new ConfigDescription( "Count of sex interactions required for escape", new AcceptableValueRange<int>( 1, 50 ) ) );
            RandomCount = config.Bind( nameof( CapturedSlaveMod ), nameof( RandomCount ), true,
                new ConfigDescription( "Use a random count of sex interactions for escape", new AcceptableValueList<bool>( [true, false] ) ) );
            MaxInteractions = config.Bind( nameof( CapturedSlaveMod ), nameof( MaxInteractions ), 5,
                new ConfigDescription( "Maximum count of sex interactions for escape when using RandomCount", new AcceptableValueRange<int>( 1, 50 ) ) );

        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
        }
    }

    internal static void Apply( CaptureSystem captureSystem ) {
        try {
            if(!Enabled.Value || !CharacterData.adultSettingsDATA.NSFWMode || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            if(!IsActivated) {
                CaptureSystem = captureSystem;
                PlayerHealthSystem = CaptureSystem.sexsystem.playerHealthSystem;

                var interactions = RandomCount.Value ? RandomUtils.Int32(1, MaxInteractions.Value) : InteractionCount.Value;

                Step = PlayerHealthSystem.MaxSP / interactions;
                Limit = Step;
                EnemyDone = 1;

                IsActivated = true;
            }

            return;
        } catch(Exception ex) {
            Plugin.Log.Error( ex );
            return;
        }
    }

    internal static void NextEnemy() {
        try {
            if(!IsActivated)
                return;

            Limit += Step;
            if(Limit > PlayerHealthSystem.MaxSP)
                Limit = PlayerHealthSystem.MaxSP;
        } catch(Exception ex) {
            Plugin.Log.Error( ex );
            return;
        }
    }

    internal static void UpdateSpecial( ref int special ) {
        try {
            if(!IsActivated)
                return;

            if(special < Limit)
                return;

            PlayerHealthSystem.CurrentSP = Limit;
            special = PlayerHealthSystem.CurrentSP;
        } catch(Exception ex) {
            Plugin.Log.Error( ex );
        }
    }

    internal static void Reset() {
        CaptureSystem = null;
        PlayerHealthSystem = null;
        IsActivated = false;
    }
}