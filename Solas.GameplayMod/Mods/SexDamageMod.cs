using BaseMod.Core;
using BaseMod.Core.Enums;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class SexDamageMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> OnPlayerCum;
    internal static MelonPreferences_Entry<bool> ExtraDamage;
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

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(SexDamageMod), nameof(Enabled), false,
                "Activates the modification", new AcceptableValueList<bool>([true, false]));
            OnPlayerCum = config.Entry(nameof(SexDamageMod), nameof(OnPlayerCum), true,
                "Activate control for player's climax", new AcceptableValueList<bool>([true, false]));
            ExtraDamage = config.Entry(nameof(SexDamageMod), nameof(ExtraDamage), true,
                "The player will take additional damage from vibrator and \"fatigue\"", new AcceptableValueList<bool>([true, false]));

            if (!OnPlayerCum.Value && !ExtraDamage.Value)
            {
                Enabled.Value = false;
            }
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
        }
    }

    internal static void UpdateSexSystem(SexSystem sexSystem)
    {
        SexSystem = sexSystem;
        PlayerHealthSystem = SexSystem.playerHealthSystem;
    }

    internal static void PlayerCum()
    {
        try
        {
            if (!Enabled.Value || SexSystem is null || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            IsPlayerCum = false;
            IsSexDamage = false;

            if (!OnPlayerCum.Value || SexSystem.GameOver)
            {
                return;
            }

            if (PlayerHealthSystem.CurrentEc < PlayerHealthSystem.MaxEc * 0.85)
            {
                return;
            }

            GameplayMod.Log.Msg("Player Cum");
            switch (SexInteraction)
            {
                case SexInteraction.Masturbation:
                    int mastDamage = -20;
                    if (OrgasmControlMod.IsModActive && OrgasmControlMod.OrgasmControl is OrgasmControl.PunishmentOrgasm)
                    {
                        if (PlayerHealthSystem.CurrentEc >= PlayerHealthSystem.MaxEc && OrgasmControlMod.UpdateAndCheckPunishmentOrgasm(true))
                        {
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
                    if (OrgasmControlMod.IsModActive && OrgasmControlMod.OrgasmControl is OrgasmControl.PunishmentOrgasm)
                    {
                        if (PlayerHealthSystem.CurrentEc >= PlayerHealthSystem.MaxEc && OrgasmControlMod.UpdateAndCheckPunishmentOrgasm(true))
                        {
                            float punish = 0.1f;
                            punish += OrgasmControlMod.PunishmentOrgasmLimit.Value / 100f;
                            damage = Mathf.RoundToInt(SexSystem.playerHealthSystem.MaxEc * punish);
                        }
                    }
                    else
                    {
                        if (PlayerHealthSystem.CurrentEc > PlayerHealthSystem.MaxEc * 0.85)
                        {
                            damage -= SexSystem.EnemyPower * 2;
                        }

                        if (SexSystem.IsThreesome && SexSystem.Assist is not null)
                        {
                            int power = SexSystem.Assist.GetComponentWithCast<EnemySex>()?.Power ?? 0;
                            damage -= power;
                        }

                        if (SexSystem.Enemy.TryGetComponentWithCast(out EnemySex enemySex) && enemySex.IsBoss)
                        {
                            damage -= enemySex.Power;
                        }

                        if (ExtraDamage.Value)
                        {
                            if (Character.statusDATA.IsBoundVibrator > 0 && RandomUtils.Chance(20))
                            {
                                damage -= RandomUtils.Int32(1, 5);
                            }

                            if (SexSystem.RecentlyFucked > 0)
                            {
                                damage -= Math.Max(SexSystem.RecentlyFucked, 0);
                            }
                        }
                    }

                    GameplayMod.Log.Msg($"Player cum damage: {damage}");
                    IsPlayerCum = UpdatePlayerMaxHP(damage);

                    break;
            }
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex);
        }
    }

    internal static bool BlockReduceMaxHP() => SceneManager.GetActiveScene().buildIndex > 4 && !SexSystem.GameOver && IsPlayerCum;

    internal static bool BlockRestoreMaxHP() => SceneManager.GetActiveScene().buildIndex > 4 && SexSystem.Sexstatus is SEXSTATUS.Fucking;

    internal static void EnemyCum()
    {
        try
        {
            if (!Enabled.Value || SexSystem is null || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            GameplayMod.Log.Msg("Enemy Cum");
            if (SexSystem.enemyHealthSystem.CurrentEc >= SexSystem.enemyHealthSystem.MaxEc)
            {
                _ = UpdatePlayerMaxHP(Mathf.RoundToInt(SexSystem.EnemyPower * 0.5f));
            }

            if (!IsRestored && PlayerHealthSystem.CurrentEc < PlayerHealthSystem.MaxEc * 0.85)
            {
                PlayerHealthSystem.CurrentEc = Mathf.RoundToInt(PlayerHealthSystem.CurrentEc * 0.6f);
                PlayerHealthSystem.UpdateArousal(PlayerHealthSystem.CurrentAr, PlayerHealthSystem.CurrentEc);
                IsRestored = true;
            }

            if (SexSystem.IsThreesome && SexSystem.assistHealthSystem.CurrentHp > 10)
            {
                EnemySex assistSex = SexSystem.Assist.GetComponentWithCast<EnemySex>();
                assistSex?.CumFX();
                SexSystem.assistHealthSystem.ResetEc();
            }
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex);
        }
    }

    internal static void ResetStates()
    {
        IsPlayerCum = false;
        IsSexDamage = false;
        SexDamageSavePlayerEc = 0;
        SexInteraction = SexInteraction.Undefined;
    }

    internal static void ResetMod()
    {
        ResetStates();
        IsRestored = false;
        SexSystem = null;
        PlayerHealthSystem = null;
    }

    private static bool UpdatePlayerMaxHP(int amount)
    {
        if (PlayerHealthSystem.MaxHp <= 5 && amount < 0)
        {
            return false;
        }

        int maxHP = PlayerHealthSystem.MaxHp + amount;

        if (maxHP < 5)
        {
            maxHP = 5;
        }

        PlayerHealthSystem.MaxHp = maxHP;

        if (PlayerHealthSystem.MaxHp > Character.statusDATA.TotalMaxHP)
        {
            PlayerHealthSystem.MaxHp = Character.statusDATA.TotalMaxHP;
        }

        if (PlayerHealthSystem.CurrentHp > PlayerHealthSystem.MaxHp)
        {
            int diff = PlayerHealthSystem.CurrentHp - PlayerHealthSystem.MaxHp;
            PlayerHealthSystem.SubstractHealth(diff);
        }
        else
        {
            int diff = PlayerHealthSystem.MaxHp - PlayerHealthSystem.CurrentHp;
            PlayerHealthSystem.AddHealth(diff);
        }

        return true;
    }
}
