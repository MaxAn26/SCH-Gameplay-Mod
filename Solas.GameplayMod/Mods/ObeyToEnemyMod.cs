using System;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx;
using BepInEx.Configuration;

using Solas.GameplayMod.Models;

using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class ObeyToEnemyMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;

    internal static ObeyToEnemyConfig ObeyToEnemyMale;
    internal static ObeyToEnemyConfig ObeyToEnemyFemale;
    internal static ObeyToEnemyConfig ObeyToEnemyFuta;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static bool IsActivated { get; set; }
    internal static bool IsInitialized { get; set; }
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    #endregion

    internal static void Load( ConfigFile config ) {
        try {
            Enabled = config.Bind( nameof( ObeyToEnemyMod ), nameof( Enabled ), false,
                new ConfigDescription( "Activates the modification", new AcceptableValueList<bool>( [true, false] ) ) );

            if(Enabled.Value) {
                if(!JsonUtils.TryDeserialize( Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}_ObeyToEnemy_Male.json", out ObeyToEnemyConfig maleConfig )) {
                    maleConfig = new ObeyToEnemyConfig();
                    _ = JsonUtils.TrySerialize( Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}_ObeyToEnemy_Male.json", maleConfig );
                }
                ObeyToEnemyMale = maleConfig;

                if(!JsonUtils.TryDeserialize( Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}_ObeyToEnemy_Female.json", out ObeyToEnemyConfig femaleConfig )) {
                    femaleConfig = new ObeyToEnemyConfig();
                    _ = JsonUtils.TrySerialize( Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}_ObeyToEnemy_Female.json", femaleConfig );
                }
                ObeyToEnemyFemale = femaleConfig;

                if(!JsonUtils.TryDeserialize( Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}_ObeyToEnemy_Futa.json", out ObeyToEnemyConfig futaConfig )) {
                    futaConfig = new ObeyToEnemyConfig();
                    _ = JsonUtils.TrySerialize( Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}_ObeyToEnemy_Futa.json", futaConfig );
                }
                ObeyToEnemyFuta = futaConfig;
            }
        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
        }
    }

    internal static void Apply( SexSystem sexSystem ) {
        try {
            if(!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            if(!IsInitialized && !IsActivated) {
                ObeyToEnemyConfig config = null;
                if(sexSystem.PlayerMale)
                    config = ObeyToEnemyMale;
                else if(Character.adultSettingsDATA.FutaPlayerMode)
                    config = ObeyToEnemyFuta;
                else
                    config = ObeyToEnemyFemale;

                if(config is null)
                    return;

                if(!IsActivated && config.OnHandRestraints && Character.statusDATA.IsBoundHandRestraint > 0) {
                    Plugin.Log.LogInfo( "Set ObeyToEnemy due hand bound" );
                    if(config.PlayerVictimOnHandRestraints)
                        PlayerVictim( sexSystem );
                    IsActivated = true;
                }

                if(!IsActivated && config.AtSameRole && sexSystem.TargetActive == sexSystem.CasterActive) {
                    if(config.PlayerVictimAtSameRole)
                        PlayerVictim( sexSystem );

                    Plugin.Log.LogInfo( "Set ObeyToEnemy due equal roles" );
                    IsActivated = true;
                }

                if(!IsActivated && config.AtSameGender && sexSystem.TargetMale == sexSystem.CasterMale) {
                    if(config.PlayerVictimAtSameGender)
                        PlayerVictim( sexSystem );

                    Plugin.Log.LogInfo( "Set ObeyToEnemy due equal gender" );
                    IsActivated = true;
                }

                if(!IsActivated && config.RandomByEnemyPower && RandomUtils.Chance( sexSystem.EnemyPower * 2 )) {
                    if(config.PlayerVictimRandomByEnemyPower)
                        PlayerVictim( sexSystem );

                    Plugin.Log.LogInfo( "Set ObeyToEnemy due enemy power random" );
                    IsActivated = true;
                }

                if(IsActivated)
                    sexSystem.console.ConsoleWrite( "Obey to enemy" );

                IsInitialized = true;
            }
        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
            return;
        }
    }

    internal static void Reset() {
        IsActivated = false;
        IsInitialized = false;
    }

    private static void PlayerVictim( SexSystem sexSystem ) {
        if(!SexSystem.PlayerAttacker)
            return;

        SexSystem.PlayerAttacker = false;
        (sexSystem.Caster, sexSystem.Target) = (sexSystem.Target, sexSystem.Caster);
        (sexSystem.CasterAnim, sexSystem.TargetAnim) = (sexSystem.TargetAnim, sexSystem.CasterAnim);
        (sexSystem.CasterActive, sexSystem.TargetActive) = (sexSystem.TargetActive, sexSystem.CasterActive);
        (sexSystem.CasterFuta, sexSystem.TargetFuta) = (sexSystem.TargetFuta, sexSystem.CasterFuta);
        (sexSystem.CasterMale, sexSystem.TargetMale) = (sexSystem.TargetMale, sexSystem.CasterMale);
    }
}