using System;

using BaseMod.Core.Extensions;

using BepInEx.Configuration;

using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class SexInitiatorStateMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<bool> DisableAsSameRole;
    internal static ConfigEntry<bool> DisableWhenBound;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static bool? IsAttacker { get; private set; } = null;
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    internal static SexSystem SexSystem = null;
    #endregion

    internal static void Load( ConfigFile config ) {
        try {
            Enabled = config.Bind( nameof( SexInitiatorStateMod ), nameof( Enabled ), false,
                new ConfigDescription( "Activates the modification", new AcceptableValueList<bool>( [true, false] ) ) );
            DisableAsSameRole = config.Bind( nameof( SexInitiatorStateMod ), nameof( DisableAsSameRole ), true,
                new ConfigDescription( "The player will not gain the Initiator role if the enemy’s role matches the player’s role", new AcceptableValueList<bool>( [true, false] ) ) );
            DisableWhenBound = config.Bind( nameof( SexInitiatorStateMod ), nameof( DisableWhenBound ), true,
                new ConfigDescription( "The player will not gain the Initiator role if their hands are bound", new AcceptableValueList<bool>( [true, false] ) ) );

        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
        }
    }

    internal static void Apply() {
        try {
            if(!Enabled.Value || SexSystem is null || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            IsAttacker = false;
            if(SexSystem.IsThreesome || Character.adultSettingsDATA.CounterVictim)
                return;

            if(Character.statusDATA.IsBoundHeavyRestraint > 0)
                return;

            if(DisableAsSameRole.Value && SexSystem.TargetActive == SexSystem.CasterActive)
                return;

            if(DisableWhenBound.Value && Character.statusDATA.IsBoundHandRestraint > 0)
                return;

            if(SexSystem.playerHealthSystem.CurrentEc >= SexSystem.playerHealthSystem.MaxEc)
                IsAttacker = false;
            else if(SexSystem.enemyHealthSystem.CurrentEc >= SexSystem.enemyHealthSystem.MaxEc)
                IsAttacker = true;
        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
        }
    }

    internal static void SetInitiator( SexSystem sexSystem ) {
        SexSystem ??= sexSystem;

        if(SexSystem.IsThreesome) {
            SexSystem.PlayerAttacker = false;
            return;
        }

        if(IsAttacker is not null)
            SexSystem.PlayerAttacker = IsAttacker.Value;
    }

    internal static void ResetMod() => IsAttacker = null;
}