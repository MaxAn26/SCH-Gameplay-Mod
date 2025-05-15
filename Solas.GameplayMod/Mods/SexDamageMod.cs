using System;

using BaseMod.Core.Enums;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

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

            switch (SexInteraction) {
                case SexInteraction.Masturbation:
                    IsPlayerCum = UpdatePlayerMaxHP(-20);
                    break;
                case SexInteraction.Undefined:
                    break;
                default:
                    int damage = 0;
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

                    IsPlayerCum = UpdatePlayerMaxHP(damage);

                    break;
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
        }
    }

    internal static bool BlockReduceMaxHP() => SceneManager.GetActiveScene().buildIndex > 4 && !SexSystem.GameOver && IsPlayerCum;

    internal static bool BlockRestoreMaxHP() => SceneManager.GetActiveScene().buildIndex > 4 && SexSystem.Sexstatus is SEXSTATUS.Fucking;

    internal static void PlayerDamage(ref int arousal, ref int ecstasy) {
        try {
            if (!Enabled.Value || SexSystem is null || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            IsSexDamage = false;

            if (!SexFight.Value || SexSystem.IsCumming)
                return;

            if (PlayerHealthSystem.CurrentAr < PlayerHealthSystem.MaxAr)                 
                return;

            switch (SexInteraction) {
                case SexInteraction.Undefined:
                    break;
                case SexInteraction.Masturbation:
                    break;
                case SexInteraction.Bukkake:
                    if (Character.statusDATA.IsBoundVibrator == 0)
                        PlayerHealthSystem.CurrentEc = SexDamageSavePlayerEc;
                    break;
                default:
                    if (SexSystem.Domination > 15 && SexSystem.Domination < 985)
                        PlayerHealthSystem.CurrentEc = SexDamageSavePlayerEc;
                    break;
            }

            arousal = PlayerHealthSystem.CurrentAr;
            ecstasy = PlayerHealthSystem.CurrentEc;
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
        }
    }

    internal static void EnemyCum() {
        try {
            if (!Enabled.Value || SexSystem is null || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            if (SexSystem.enemyHealthSystem.CurrentEc >= SexSystem.enemyHealthSystem.MaxEc)
                _ = UpdatePlayerMaxHP(Convert.ToInt32(Math.Round(SexSystem.EnemyPower * 0.5)));

            if (!IsRestored && PlayerHealthSystem.CurrentEc < PlayerHealthSystem.MaxEc * 0.85) {
                PlayerHealthSystem.CurrentEc = Convert.ToInt32(Math.Round(PlayerHealthSystem.CurrentEc * 0.6));
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

    internal static void EnemyEcstasyDamage(HealthSystem healthSystem, ref int damage) {
        try {
            if (!Enabled.Value || SexSystem is null || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            if (!SexFight.Value || SexSystem.GameOver || damage == 0)
                return;

            if (!healthSystem.gameObject.TryGetComponentWithCast(out EnemySex enemySex)) {
                Plugin.Log.Info("Can't find EnemySex");
                return;
            }

            if (enemySex.IsAssist)
                return;

            if (SexSystem.IsThreesome)
                SexSystem.assistHealthSystem.IncreaseEc(damage);

            if (healthSystem.CurrentEc < healthSystem.MaxEc / 2)
                return;

            if (SexSystem.Domination > 15 && SexSystem.Domination < 985)
                damage = 0;
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
        }
    }

    internal static void EnemyHPDamage(HealthSystem healthSystem, ref int damage) {
        try {
            if (!Enabled.Value || SexSystem is null || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            if (!ExtraDamage.Value || SexSystem.GameOver || damage == 0)
                return;

            if (!healthSystem.gameObject.TryGetComponentWithCast(out EnemySex enemySex)) {
                Plugin.Log.Info("Can't find EnemySex");
                return;
            }

            if (enemySex.IsAssist)                
                return;

            if (Character.statusDATA.IsBoundHeavyRestraint > 0)
                return;

            if (healthSystem.CurrentHp < 30)
                return;

            if (enemySex.IsCumming || SexSystem.IsCumming || !SexSystem.SexEffectRunning)
                return;

            if (SexSystem.Domination > 15 && SexSystem.Domination < 985)
                damage = 0;
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

    private static bool UpdatePlayerMaxHP(int damage) {
        if (PlayerHealthSystem.MaxHp <= 5 && damage < 0)
            return false;

        int maxHP = PlayerHealthSystem.MaxHp + damage;

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