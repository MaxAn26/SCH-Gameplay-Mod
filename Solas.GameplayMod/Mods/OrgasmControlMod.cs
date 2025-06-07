using System;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

using Solas.GameplayMod.Components;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Solas.GameplayMod.Mods;
internal class OrgasmControlMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<bool> ArousalFatigue;
    internal static ConfigEntry<bool> SelfControl;
    internal static ConfigEntry<int> SelfControlChance;
    internal static ConfigEntry<bool> DeniedOrgasm;
    internal static ConfigEntry<int> DeniedOrgasmChance;
    internal static ConfigEntry<bool> PunishmentOrgasm;
    internal static ConfigEntry<int> PunishmentOrgasmChance;
    internal static ConfigEntry<int> PunishmentOrgasmLimit;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static OrgasmControl OrgasmControl = OrgasmControl.None;
    internal static int PunishmentOrgasmCount;
    internal static Sprite PunishmentOrgasmSprite;
    #endregion



    internal static void Load(ConfigFile config) {
        try {
            Enabled = config.Bind(nameof(OrgasmControlMod), nameof(Enabled), false,
                new ConfigDescription("Activates the modification", new AcceptableValueList<bool>([true, false])));
            ArousalFatigue = config.Bind(nameof(OrgasmControlMod), nameof(ArousalFatigue), true,
                new ConfigDescription("After cum character gain arouse slowly for a while", new AcceptableValueList<bool>([true, false])));
            SelfControl = config.Bind(nameof(OrgasmControlMod), nameof(SelfControl), true,
                new ConfigDescription("Target character may control his own orgasm and will cum with caster", new AcceptableValueList<bool>([true, false])));
            SelfControlChance = config.Bind(nameof(OrgasmControlMod), nameof(SelfControlChance), 10,
                new ConfigDescription("Chance for target character to control his own orgasm", new AcceptableValueRange<int>(0, 100)));
            DeniedOrgasm = config.Bind(nameof(OrgasmControlMod), nameof(DeniedOrgasm), true,
                new ConfigDescription("Caster may deny target to cum when target reached maximum ecstasy if first time during current sex position", new AcceptableValueList<bool>([true, false])));
            DeniedOrgasmChance = config.Bind(nameof(OrgasmControlMod), nameof(DeniedOrgasmChance), 10,
                new ConfigDescription("Chance for caster deny target to cum", new AcceptableValueRange<int>(0, 100)));
            PunishmentOrgasm = config.Bind(nameof(OrgasmControlMod), nameof(PunishmentOrgasm), true,
                new ConfigDescription("Enemy may deny player to cum in next several times", new AcceptableValueList<bool>([true, false])));
            PunishmentOrgasmChance = config.Bind(nameof(OrgasmControlMod), nameof(PunishmentOrgasmChance), 10,
                new ConfigDescription("Chance for enemy deny player to cum", new AcceptableValueRange<int>(0, 100)));
            PunishmentOrgasmLimit = config.Bind(nameof(OrgasmControlMod), nameof(PunishmentOrgasmLimit), 10,
                new ConfigDescription("Count of player reach max ecstasy before he will be punished", new AcceptableValueRange<int>(0, 100)));

            if (!Enabled.Value) {
                ArousalFatigue.Value = false;
                SelfControl.Value = false;
                PunishmentOrgasm.Value = false;
            } else {
                PunishmentOrgasmSprite = ResourcesUtils.LoadSprite(Plugin.PluginAssets, "belt.png");
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static int ArousalFatigueCheck(GameObject gameObject, int arousalDamage) {
        try {
            if (!Enabled.Value || !ArousalFatigue.Value)
                return arousalDamage;

            if (arousalDamage <= 0)
                return 0;

            if (!gameObject.TryGetComponentWithCast(out HealthComponent healthComponent))
                return arousalDamage;

            float deltaTime = Time.time - healthComponent.LastCumTime;
            float rate = 1f;
            if (healthComponent.IsCharacterSelfCum) {
                if (deltaTime < 15f) {
                    rate = 0.5f;
                } else if (deltaTime < 30f) {
                    rate = 0.75f;
                }
            } else {
                if (deltaTime < 15f) {
                    rate = 0.75f;
                }
            }

            float newArousal = arousalDamage * rate;
            return Mathf.Max(1, Mathf.RoundToInt(newArousal));
        } catch (Exception ex) { 
            Plugin.Log.Error(ex.Message);
            return arousalDamage;
        }
    } 

    internal static void ApplySexControl(SexSystem sexSystem) {
        try {
            if (!Enabled.Value || !sexSystem.IsSex || SceneManager.GetActiveScene().buildIndex <= 4)
                return;

            if (OrgasmControl is not OrgasmControl.None)
                return;

            if (SelfControl.Value && RandomUtils.Chance(SelfControlChance.Value)) {
                OrgasmControl = OrgasmControl.SelfControl;
            } else if (DeniedOrgasm.Value && RandomUtils.Chance(DeniedOrgasmChance.Value)) {
                OrgasmControl = OrgasmControl.DeniedOrgasm;
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return;
        }
    }

    internal static void ResetSelfControl() {
        if (OrgasmControl is OrgasmControl.SelfControl)
            OrgasmControl = OrgasmControl.None;
    }

    internal static void ResetDeniedOrgasm() {
        if (OrgasmControl is OrgasmControl.DeniedOrgasm)
            OrgasmControl = OrgasmControl.None;
    }

    internal static bool ApplyPunishmentOrgasm(SexSystem sexSystem) {
        try {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
                return false;

            if (OrgasmControl is not OrgasmControl.None)
                return false;

            if (PunishmentOrgasm.Value && RandomUtils.Chance(PunishmentOrgasmChance.Value)) {
                OrgasmControl = OrgasmControl.PunishmentOrgasm;
                string enemyName = sexSystem.Enemy.TryGetComponentWithCast(out EnemyAI enemyAI) ? enemyAI.enemyName : "Enemy";
                AddIcon(sexSystem, PunishmentOrgasmSprite);
                sexSystem.console.ConsoleWrite($"{enemyName} put on you Punishment Orgasm buff");
                return true;
            }

            return false;
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return false;
        }
    }

    internal static bool UpdateAndCheckPunishmentOrgasm(bool reset = false) {
        if (OrgasmControl is OrgasmControl.PunishmentOrgasm) {
            PunishmentOrgasmCount++;
            if (PunishmentOrgasmCount >= PunishmentOrgasmLimit.Value) {
                if (reset) 
                    ResetPunishmentOrgasm();

                return true;
            }
        } else {
            PunishmentOrgasmCount = 0;
        }

        return false;
    }

    internal static void BreakPunishmentOrgasm(SexSystem sexSystem) {
        if (OrgasmControl is OrgasmControl.PunishmentOrgasm) {
            ResetPunishmentOrgasm();
            RemoveIcon(sexSystem, PunishmentOrgasmSprite);
            sexSystem.console.ConsoleWrite("You managed to break Punishment Orgasm!");
        }
    }

    internal static void ResetPunishmentOrgasm() {
        if (OrgasmControl is OrgasmControl.PunishmentOrgasm)
            OrgasmControl = OrgasmControl.None;

        PunishmentOrgasmCount = 0;
    }

    internal static void AddIcon(SexSystem sexSystem, Sprite sprite) {
        if (sprite is null)
            return;

        Transform prefabTransform = sexSystem.buffsystem.transform;
        var gameObject = UnityEngine.Object.Instantiate(sexSystem.buffsystem.TextPrefab, prefabTransform);
        gameObject.GetComponentWithCast<Image>().sprite = sprite;
        sexSystem.buffsystem.Buffs.Add(gameObject);
    }

    internal static void RemoveIcon(SexSystem sexSystem, Sprite sprite) {
        if (sprite is null)
            return;

        for (int i = 0; i < sexSystem.buffsystem.Buffs.Count; i++) {
            if (sexSystem.buffsystem.Buffs[i].GetComponentWithCast<Image>().sprite == sprite) {
                UnityEngine.Object.Destroy(sexSystem.buffsystem.Buffs[i].gameObject);
                sexSystem.buffsystem.Buffs.RemoveAt(i);
            }
        }
    }
}

internal enum OrgasmControl {
    None,
    SelfControl,
    DeniedOrgasm,
    PunishmentOrgasm
}