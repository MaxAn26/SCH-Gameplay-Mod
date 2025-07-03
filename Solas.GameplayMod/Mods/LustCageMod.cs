using System;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx;
using BepInEx.Configuration;

using Solas.GameplayMod.Models;

using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class LustCageMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<bool> DenyEscape;

    internal static LustCageConfig LustCageMale;
    internal static LustCageConfig LustCageFemale;
    internal static LustCageConfig LustCageFuta;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    #endregion

    internal static void Load( ConfigFile config ) {
        try {
            Enabled = config.Bind( nameof( LustCageMod ), nameof( Enabled ), false,
                new ConfigDescription( "Activates the modification", new AcceptableValueList<bool>( [true, false] ) ) );
            DenyEscape = config.Bind( nameof( LustCageMod ), nameof( DenyEscape ), true,
                new ConfigDescription( "Player can't escape from interaction and should end it by cumming or using counter", new AcceptableValueList<bool>( [true, false] ) ) );

            if(Enabled.Value) {
                if(!JsonUtils.TryDeserialize( Plugin.PluginConfigs, $"{MyPluginInfo.PLUGIN_GUID}_LustCage_Male.json", out LustCageConfig maleConfig )) {
                    maleConfig = new LustCageConfig();
                    _ = JsonUtils.TrySerialize(Plugin.PluginConfigs, $"{MyPluginInfo.PLUGIN_GUID}_LustCage_Male.json", maleConfig );
                }
                LustCageMale = maleConfig;

                if(!JsonUtils.TryDeserialize( Plugin.PluginConfigs, $"{MyPluginInfo.PLUGIN_GUID}_LustCage_Female.json", out LustCageConfig femaleConfig )) {
                    femaleConfig = new LustCageConfig();
                    _ = JsonUtils.TrySerialize( Plugin.PluginConfigs, $"{MyPluginInfo.PLUGIN_GUID}_LustCage_Female.json", femaleConfig );
                }
                LustCageFemale = femaleConfig;

                if(!JsonUtils.TryDeserialize( Plugin.PluginConfigs, $"{MyPluginInfo.PLUGIN_GUID}_LustCage_Futa.json", out LustCageConfig futaConfig )) {
                    futaConfig = new LustCageConfig();
                    _ = JsonUtils.TrySerialize( Plugin.PluginConfigs, $"{MyPluginInfo.PLUGIN_GUID}_LustCage_Futa.json", futaConfig );
                }
                LustCageFuta = futaConfig;
            }
        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
        }
    }

    internal static void Apply( SexSystem sexSystem ) {
        try {
            if(!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            if(sexSystem.playerweak)
                return;

            LustCageConfig config = null;
            if(sexSystem.PlayerMale)
                config = LustCageMale;
            else if(Character.adultSettingsDATA.FutaPlayerMode)
                config = LustCageFuta;
            else
                config = LustCageFemale;

            if(config is null)
                return;

            switch(sexSystem.SexType) {
                case 1: // oral
                    if(config.OralReceiver) {
                        var active = SexSystem.PlayerAttacker ? SexSystem.SexIsLickingTarget || SexSystem.SexIsOralTarget : SexSystem.SexIsLickingCaster || SexSystem.SexIsOralCaster;
                        if(active) {
                            ActivateBuff( sexSystem );
                            return;
                        }
                    }

                    if(config.LickPussy) {
                        var active = SexSystem.PlayerAttacker ? SexSystem.SexIsLickingCaster : SexSystem.SexIsLickingTarget;
                        if(active) {
                            ActivateBuff( sexSystem );
                            return;
                        }
                    }

                    if(config.SuckDick) {
                        var active = SexSystem.PlayerAttacker ? SexSystem.SexIsOralCaster : SexSystem.SexIsOralTarget;
                        if(active) {
                            ActivateBuff( sexSystem );
                            return;
                        }
                    }
                    break;
                case 2: // hand job
                    if(config.HandDom && SexSystem.PlayerAttacker) {
                        ActivateBuff( sexSystem );
                        return;
                    }

                    if(config.HandSub && !SexSystem.PlayerAttacker) {
                        ActivateBuff( sexSystem );
                        return;
                    }
                    break;
                case 3: // boob job
                    if(config.BoobsReceiver && !sexSystem.PlayerMale) {
                        ActivateBuff( sexSystem );
                        return;
                    }

                    if(config.BoobsLover && !sexSystem.EnemyMale) {
                        ActivateBuff( sexSystem );
                        return;
                    }
                    break;
                case 4: // foot job
                    if(config.FootFetish && !SexSystem.PlayerAttacker) {
                        ActivateBuff( sexSystem );
                        return;
                    }
                    break;
                case 6 or 7 or 8: // missionary
                    if(config.PenetrateOther) {
                        if(SexSystem.PlayerAttacker) {
                            if(sexSystem.CasterActive && !sexSystem.TargetActive) {
                                ActivateBuff( sexSystem );
                                return;
                            }
                        } else {
                            if(!sexSystem.CasterActive && sexSystem.TargetActive) {
                                ActivateBuff( sexSystem );
                                return;
                            }
                        }
                    }

                    if(config.BeingPenetrated) {
                        if(SexSystem.PlayerAttacker) {
                            if(!sexSystem.CasterActive && sexSystem.TargetActive) {
                                ActivateBuff( sexSystem );
                                return;
                            }
                        } else {
                            if(sexSystem.CasterActive) {
                                ActivateBuff( sexSystem );
                                return;
                            }
                        }
                    }

                    break;
                case 9: // threesome
                    if(config.ThreesomePose) {
                        ActivateBuff( sexSystem );
                        return;
                    }

                    if(config.OralReceiver) {
                        var active = SexSystem.PlayerAttacker ? SexSystem.SexIsLickingTarget || SexSystem.SexIsOralTarget : SexSystem.SexIsLickingCaster || SexSystem.SexIsOralCaster;
                        if(active) {
                            ActivateBuff( sexSystem );
                            return;
                        }
                    }

                    if(config.LickPussy) {
                        var active = SexSystem.PlayerAttacker ? SexSystem.SexIsLickingCaster : SexSystem.SexIsLickingTarget;
                        if(active) {
                            ActivateBuff( sexSystem );
                            return;
                        }
                    }

                    if(config.SuckDick) {
                        var active = SexSystem.PlayerAttacker ? SexSystem.SexIsOralCaster : SexSystem.SexIsOralTarget;
                        if(active) {
                            ActivateBuff( sexSystem );
                            return;
                        }
                    }

                    if(config.ThreesomeSex) {
                        if(sexSystem.PlayerMale) {
                            if(!sexSystem.CasterActive) {
                                ActivateBuff( sexSystem );
                                return;
                            }
                        } else {
                            if(sexSystem.CasterActive) {
                                ActivateBuff( sexSystem );
                                return;
                            }
                        }
                    }

                    if(config.ThreesomeSameGenderToPlayer && sexSystem.PlayerMale == sexSystem.EnemyMale && sexSystem.PlayerMale == sexSystem.AssistMale) {
                        ActivateBuff( sexSystem );
                        return;
                    }

                    break;
                default:
                    break;
            }

            if(config.HeavyBondageSlave && Character.statusDATA.IsBoundHeavyRestraint > 0) {
                ActivateBuff( sexSystem );
                return;
            }
        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
            return;
        }
    }

    internal static bool BlockEscape( SexSystem sexSystem ) {
        return  SceneManager.GetActiveScene().buildIndex > 4 && DenyEscape.Value && sexSystem.playerweak;
    }

    private static void ActivateBuff( SexSystem sexSystem ) {
        if(sexSystem.playerweak)
            return;

        sexSystem.buffsystem.CastBuff( 0, false, 0 );
        sexSystem.playerweak = true;
    }
}