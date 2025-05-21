using System;

using BaseMod.Core.Enums;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

using Solas.GameplayMod.Components;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class SexDamageMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<bool> SexFight;
    internal static ConfigEntry<bool> OnPlayerCum;
    internal static ConfigEntry<bool> ExtraDamage;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static SexInteraction SexInteraction = SexInteraction.Undefined;

    internal static bool IsPlayerCum { get; set; }
    internal static bool IsSexDamage { get; set; }
    internal static bool IsRestored { get; set; }
    internal static int SexDamageSavePlayerEc { get; set; }
    internal static int SexDamageStack { get; set; }
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    internal static SexSystem SexSystem;
    internal static PlayerHealthSystem PlayerHealthSystem;
    #endregion

    internal static void Load(ConfigFile config) {
        try {
            Enabled = config.Bind(nameof(SexDamageMod), nameof(Enabled), false,
                new ConfigDescription("Activates the modification", new AcceptableValueList<bool>([true, false])));
            SexFight = config.Bind(nameof(SexDamageMod), nameof(SexFight), true,
                new ConfigDescription("Block damage during the struggle for dominance", new AcceptableValueList<bool>([true, false])));
            OnPlayerCum = config.Bind(nameof(SexDamageMod), nameof(OnPlayerCum), true,
                new ConfigDescription("Activate control for player's climax", new AcceptableValueList<bool>([true, false])));
            ExtraDamage = config.Bind(nameof(SexDamageMod), nameof(ExtraDamage), true,
                new ConfigDescription("The player will take additional damage from vibrator and \"fatigue\"", new AcceptableValueList<bool>([true, false])));

            if (!SexFight.Value && !OnPlayerCum.Value && !ExtraDamage.Value)
                Enabled.Value = false;
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void UpdateSexSystem(SexSystem sexSystem) {
        SexSystem = sexSystem;
        PlayerHealthSystem = SexSystem.playerHealthSystem;
    }

    internal static bool ApplySexDamage() {
        try {
            if (!Enabled.Value || SexSystem is null || SceneManager.GetActiveScene().buildIndex <= 4)
                return false;

            #region player
            var playerComponent = SexSystem.Player.GetComponentWithCast<HealthComponent>();
            if (playerComponent is null)
                return false;

            int spBonus = 0;
            int arBonus = 0;

            if (SexSystem.playerweak) {
                spBonus += 35;
                arBonus += 1;
            }

            if (CharacterData.Instance.statusDATA.UnlockedTalents.Contains(54)) {
                spBonus += 15;
            }
            if (CharacterData.Instance.statusDATA.UnlockedTalents.Contains(55)) {
                spBonus -= 25;
            }
            if (CharacterData.Instance.statusDATA.UnlockedTalents.Contains(57)) {
                spBonus += 15;
            }
            if (CharacterData.Instance.statusDATA.UnlockedTalents.Contains(58)) {
                spBonus -= 15;
            }
            if (CharacterData.Instance.adultSettingsDATA.CheatSpecial == true) {
                spBonus += 100;
            }

            if (CharacterData.Instance.statusDATA.UnlockedTalents.Contains(66)) {
                spBonus -= 40;
            }

            if (CharacterData.Instance.statusDATA.FetishList.Contains(3)) {
                //Domination fetish
                if (SexSystem.Domination > 900) {
                    spBonus += 25;
                    arBonus += 2;
                } else if (SexSystem.Domination > 750) {
                    spBonus += 12;
                    arBonus += 1;
                }
            }

            if (CharacterData.Instance.statusDATA.FetishList.Contains(8) && SexSystem.SexTypeEffect == 2) {
                //Wrestling fetish
                spBonus += 35;
                arBonus += 1;
            }
            if (CharacterData.Instance.statusDATA.FetishList.Contains(9) && SexSystem.SexTypeEffect == 3) {
                //Breathplay fetish
                spBonus += 35;
                arBonus += 1;
            }
            if (CharacterData.Instance.statusDATA.FetishList.Contains(4)) {
                //switch fetish
                if (SexSystem.Domination > 850) {
                    spBonus += 20;
                    arBonus += 1;
                } else if (SexSystem.Domination < 150) {
                    spBonus -= 20;
                }
            }

            if (SexSystem.playersuccubus) { spBonus -= 40; }
            if (SexSystem.CounterBuff) {
                spBonus += 25;
            }

            if (CharacterData.Instance.adultSettingsDATA.SubmissionSystem == true && CharacterData.Instance.statusDATA.TrainedLevel > 4) {
                if (CharacterData.Instance.statusDATA.IsBoundCollar != 0 && SexSystem.Stage >= 4) {
                    //collared
                    arBonus -= 1;
                }
            }

            if (CharacterData.Instance.adultSettingsDATA.CorruptionSystem == true && CharacterData.Instance.statusDATA.CorruptionLevel > 4) {
                arBonus -= 1;
            }


            if (CharacterData.Instance.statusDATA.UnlockedTalents.Contains(97)) {
                arBonus -= 1;
            }

            spBonus = SexSystem.Stage * (100 + spBonus) / 100;
            arBonus += SexSystem.Stage;

            if (spBonus < 1) {
                spBonus = 1;
            }

            if (!SexSystem.PlayerIsAttacking) {
                if (SexSystem.playerHealthSystem.CurrentEc >= SexSystem.playerHealthSystem.MaxEc * 0.85) {
                    switch (OrgasmControlMod.OrgasmControl) {
                        case OrgasmControl.SelfControl:
                            arBonus = 0;
                            break;
                        case OrgasmControl.DeniedOrgasm:
                            SexSystem.Player.GetComponentWithCast<HealthComponent>().ResetPleasure();
                            OrgasmControlMod.ResetDeniedOrgasm();
                            break;
                        case OrgasmControl.PunishmentOrgasm:
                            if (!OrgasmControlMod.UpdateAndCheckPunishmentOrgasm())
                                SexSystem.Player.GetComponentWithCast<HealthComponent>().ResetPleasure();
                            break;
                        default:
                            break;
                    }
                }
            }

            SexMoveChoiceMod.CheckPlayerArousal(ref arBonus);
            arBonus = OrgasmControlMod.ArousalFatigueCheck(SexSystem.Player, arBonus);

            Plugin.Log.Info($"Player sex damage: {arBonus}");
            playerComponent.AddPleasure(arBonus);
            SexSystem.playerHealthSystem.UpdateSpecial(SexSystem.playerHealthSystem.CurrentSP + spBonus);

            if (!SexSystem.IsCumming && SexSystem.playerHealthSystem.CurrentEc >= SexSystem.playerHealthSystem.MaxEc) {
                Plugin.Log.Info("Run CumEngine");
                SexSystem.IsCumming = true;
                SexSystem.StartCoroutine(SexSystem.CumEngine());
            }
            #endregion player

            #region enemy
            SexDamageStack++;
            if (SexDamageStack >= 6){
                SexDamageStack = 0;
                int enemyHealthDamage = 0;
                switch (SexSystem.Stage) {
                    case 1:
                        enemyHealthDamage += 5;
                        break;
                    case 2:
                        enemyHealthDamage += 4;
                        break;
                    case 3:
                        enemyHealthDamage += 3;
                        break;
                    case 4:
                        enemyHealthDamage += 2;
                        break;
                    case 5:
                        enemyHealthDamage += 1;
                        break;
                }

                if (CharacterData.Instance.statusDATA.FetishList.Contains(2)) {
                    if (SexSystem.Domination < 100f) {
                        enemyHealthDamage += 2;
                    } else if (SexSystem.Domination < 350f) {
                        enemyHealthDamage += 1;
                    }
                }

                if (CharacterData.Instance.statusDATA.FetishList.Contains(4) && SexSystem.Domination < 150f) {
                    enemyHealthDamage += 1;
                }

                if (CharacterData.Instance.statusDATA.UnlockedTalents.Contains(66) && SexSystem.Domination <= 300f) {
                    enemyHealthDamage += 2;
                }
                
                if (SexSystem.enemyHealthSystem.CurrentHp > enemyHealthDamage)
                    SexSystem.Enemy.GetComponentWithCast<HealthComponent>()?.SubstractHealth(enemyHealthDamage);
                if (SexSystem.IsThreesome && SexSystem.assistHealthSystem.CurrentHp > enemyHealthDamage)
                    SexSystem.Assist.GetComponentWithCast<HealthComponent>()?.SubstractHealth(enemyHealthDamage);

                int damage = RandomUtils.Int32(0, 5);
                if (SexSystem.PlayerIsAttacking && OrgasmControlMod.IsModActive) {
                    if (SexSystem.enemyHealthSystem.CurrentEc >= SexSystem.enemyHealthSystem.MaxEc * 0.85) {
                        switch (OrgasmControlMod.OrgasmControl) {
                            case OrgasmControl.SelfControl:
                                damage = 0;
                                break;
                            case OrgasmControl.DeniedOrgasm:
                                SexSystem.Enemy.GetComponentWithCast<HealthComponent>().ResetPleasure();
                                OrgasmControlMod.OrgasmControl = OrgasmControl.None;
                                break;
                            default:
                                break;
                        }
                    }
                }

                SexMoveChoiceMod.CheckEnemyArousal(ref damage);
                damage = OrgasmControlMod.ArousalFatigueCheck(SexSystem.Enemy, damage);

                Plugin.Log.Info($"Enemy sex damage: {damage}");
                SexSystem.Enemy.GetComponentWithCast<HealthComponent>()?.AddPleasure(damage);
                    if (SexSystem.IsThreesome)
                        SexSystem.Assist.GetComponentWithCast<HealthComponent>()?.AddPleasure(damage);
            }
            #endregion enemy

            return true;
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
            return false;
        }
    }

    internal static void PlayerCum() {
        try {
            if (!Enabled.Value || SexSystem is null || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            IsPlayerCum = false;
            IsSexDamage = false;

            if (!OnPlayerCum.Value || SexSystem.GameOver)
                return;

            if (PlayerHealthSystem.CurrentEc < PlayerHealthSystem.MaxEc * 0.85)
                return;

            Plugin.Log.Info("Player Cum");
            switch (SexInteraction) {
                case SexInteraction.Masturbation:
                    int mastDamage = -20;
                    if (OrgasmControlMod.IsModActive && OrgasmControlMod.OrgasmControl is OrgasmControl.PunishmentOrgasm) {
                        if (PlayerHealthSystem.CurrentEc >= PlayerHealthSystem.MaxEc && OrgasmControlMod.UpdateAndCheckPunishmentOrgasm(true)) {
                            float punish = 0.1f;
                            punish += OrgasmControlMod.PunishmentOrgasmLimit.Value / 100f;
                            mastDamage = Mathf.RoundToInt(SexSystem.playerHealthSystem.MaxEc * punish);
                        }
                    }

                    IsPlayerCum = UpdatePlayerMaxHP(mastDamage);
                    break;
                case SexInteraction.Undefined:
                    break;
                default:
                    int damage = 0;
                    if (OrgasmControlMod.IsModActive && OrgasmControlMod.OrgasmControl is OrgasmControl.PunishmentOrgasm) {
                        if (PlayerHealthSystem.CurrentEc >= PlayerHealthSystem.MaxEc && OrgasmControlMod.UpdateAndCheckPunishmentOrgasm(true)) {
                            float punish = 0.1f;
                            punish += OrgasmControlMod.PunishmentOrgasmLimit.Value / 100f;
                            damage = Mathf.RoundToInt(SexSystem.playerHealthSystem.MaxEc * punish);
                        }
                    } else {
                        if (PlayerHealthSystem.CurrentEc > PlayerHealthSystem.MaxEc * 0.85)
                            damage -= SexSystem.EnemyPower * 2;

                        if (SexSystem.IsThreesome && SexSystem.Assist is not null) {
                            int power = SexSystem.Assist.GetComponentWithCast<EnemySex>()?.Power ?? 0;
                            damage -= power;
                        }

                        if (SexSystem.Enemy.TryGetComponentWithCast(out EnemySex enemySex) && enemySex.IsBoss)
                            damage -= enemySex.Power;

                        if (ExtraDamage.Value) {
                            if (Character.statusDATA.IsBoundVibrator > 0 && RandomUtils.Chance(20))
                                damage -= RandomUtils.Int32(1, 5);

                            if (SexSystem.RecentlyFucked > 0)
                                damage -= Math.Max(SexSystem.RecentlyFucked, 0);
                        }
                    }

                    Plugin.Log.Info($"Player cum damage: {damage}");
                    IsPlayerCum = UpdatePlayerMaxHP(damage);

                    break;
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
        }
    }

    internal static bool BlockReduceMaxHP() => SceneManager.GetActiveScene().buildIndex > 4 && !SexSystem.GameOver && IsPlayerCum;

    internal static bool BlockRestoreMaxHP() => SceneManager.GetActiveScene().buildIndex > 4 && SexSystem.Sexstatus is SEXSTATUS.Fucking;

    internal static void EnemyCum() {
        try {
            if (!Enabled.Value || SexSystem is null || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            Plugin.Log.Info("Enemy Cum");
            if (SexSystem.enemyHealthSystem.CurrentEc >= SexSystem.enemyHealthSystem.MaxEc)
                _ = UpdatePlayerMaxHP(Mathf.RoundToInt(SexSystem.EnemyPower * 0.5f));

            if (!IsRestored && PlayerHealthSystem.CurrentEc < PlayerHealthSystem.MaxEc * 0.85) {
                PlayerHealthSystem.CurrentEc = Mathf.RoundToInt(PlayerHealthSystem.CurrentEc * 0.6f);
                PlayerHealthSystem.UpdateArousal(PlayerHealthSystem.CurrentAr, PlayerHealthSystem.CurrentEc);
                IsRestored = true;
            }

            if (SexSystem.IsThreesome && SexSystem.assistHealthSystem.CurrentHp > 10) {
                var assistSex = SexSystem.Assist.GetComponentWithCast<EnemySex>();
                assistSex?.CumFX();
                SexSystem.assistHealthSystem.ResetEc();
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
        }
    }

    internal static void ResetStates() {
        IsPlayerCum = false;
        IsSexDamage = false;
        SexDamageSavePlayerEc = 0;
        SexInteraction = SexInteraction.Undefined;
    }

    internal static void ResetMod() {
        ResetStates();
        IsRestored = false;
        SexSystem = null;
        PlayerHealthSystem = null;
    }

    private static bool UpdatePlayerMaxHP(int amount) {
        if (PlayerHealthSystem.MaxHp <= 5 && amount < 0)
            return false;

        int maxHP = PlayerHealthSystem.MaxHp + amount;

        if (maxHP < 5)
            maxHP = 5;

        PlayerHealthSystem.MaxHp = maxHP;

        if (PlayerHealthSystem.MaxHp > Character.statusDATA.TotalMaxHP)
            PlayerHealthSystem.MaxHp = Character.statusDATA.TotalMaxHP;

        if (PlayerHealthSystem.CurrentHp > PlayerHealthSystem.MaxHp) {
            int diff = PlayerHealthSystem.CurrentHp - PlayerHealthSystem.MaxHp;
            PlayerHealthSystem.SubstractHealth(diff);
        } else {
            int diff = PlayerHealthSystem.MaxHp - PlayerHealthSystem.CurrentHp;
            PlayerHealthSystem.AddHealth(diff);
        }

        return true;
    }
}