using System;
using System.Collections.Generic;
using System.Linq;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

using Solas.GameplayMod.Models;

namespace Solas.GameplayMod.Mods;
internal class EnemyTraitsMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<int> ChanceEnhanced;
    internal static ConfigEntry<int> MaxEnhanced;
    internal static ConfigEntry<int> ChanceMiniBoss;
    internal static ConfigEntry<int> MaxMiniBoss;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static int EnhancedCount;
    internal static int MiniBossCount;
    internal static List<EnemyTraitModel> EnemyTraits = [];

    internal static void Load(ConfigFile config) {
        try {
            Enabled = config.Bind(nameof(EnemyTraitsMod), nameof(Enabled), false,
                new ConfigDescription("Activates the modification", new AcceptableValueList<bool>([true, false])));
            ChanceEnhanced = config.Bind(nameof(EnemyTraitsMod), nameof(ChanceEnhanced), 20,
                new ConfigDescription("Chance for enemy get Enhanced traits", new AcceptableValueRange<int>(0, 100)));
            MaxEnhanced = config.Bind(nameof(EnemyTraitsMod), nameof(MaxEnhanced), 15,
                new ConfigDescription("Max count of Enhanced traits on Stage", new AcceptableValueRange<int>(0, 100)));
            ChanceMiniBoss = config.Bind(nameof(EnemyTraitsMod), nameof(ChanceMiniBoss), 10,
                new ConfigDescription("Chance for enemy get Mini boss traits", new AcceptableValueRange<int>(0, 100)));
            MaxMiniBoss = config.Bind(nameof(EnemyTraitsMod), nameof(MaxMiniBoss), 5,
                new ConfigDescription("Max count of Mini boss traits on Stage", new AcceptableValueRange<int>(0, 100)));

            if (Enabled.Value) {
                if (!JsonUtils.TryDeserialize(Plugin.PluginResources, "EnemyTraits.json", out List<EnemyTraitModel> enemyTraits)) {
                    enemyTraits = GetDefaultTraits();
                    JsonUtils.TrySerialize(Plugin.PluginResources, "EnemyTraits.json", enemyTraits);
                }
                EnemyTraits = enemyTraits;
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static EnemyTraitModel GetEnemyTrait(int enemyType) {
        try {
            if (!Enabled.Value)
                return null;

            var traits = EnemyTraits.Where( t => t.EnemyType is null || t.EnemyType == enemyType);
            if (!traits.Any())
                return null;

            TraitType type = TraitType.General;
            if (RandomUtils.Chance(ChanceMiniBoss.Value) && MiniBossCount < MaxMiniBoss.Value && traits.Any(t => t.TraitType is TraitType.MiniBoss)) {
                type = TraitType.MiniBoss;
                MiniBossCount++;
            } else if (RandomUtils.Chance(ChanceEnhanced.Value) && EnhancedCount < MaxEnhanced.Value && traits.Any(t => t.TraitType is TraitType.Enhanced)) {
                type = TraitType.Enhanced;
                EnhancedCount++;
            }

            return traits.Where(t => t.TraitType == type).RandomItem();
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return null;
        }
    }

    internal static void ApplyTrait(EnemyAI enemyAI, EnemyTraitModel enemyTrait) {
        try {
            if (!Enabled.Value || enemyTrait is null || enemyAI.IsBoss)
                return;

            if (enemyTrait.HealthOperation is not TraitOperation.None) {
                int origin = (enemyAI.healthSystem.MaxHp - 70) / 50;
                int newValue = enemyTrait.CalculateValue(enemyTrait.HealthOperation, origin, enemyTrait.HealthValue);
                enemyAI.healthSystem.MaxHp = 70 + newValue * 50;
                enemyAI.healthSystem.CurrentHp = enemyAI.healthSystem.MaxHp;
                enemyAI.healthSystem.SendHealthUpdateEvent();
            }

            if (enemyTrait.ArmorOperation is not TraitOperation.None) {
                int origin = enemyAI.healthSystem.Armor / 6;
                int newValue = enemyTrait.CalculateValue(enemyTrait.ArmorOperation, origin, enemyTrait.ArmorValue);
                enemyAI.healthSystem.Armor = newValue * 6;
            }

            if (enemyTrait.RegenerationOperation is not TraitOperation.None) {
                int newValue = enemyTrait.CalculateValue(enemyTrait.RegenerationOperation, enemyAI.healthSystem.Regeneration, enemyTrait.RegenerationValue);
                enemyAI.healthSystem.Regeneration = newValue;
            }

            if (enemyTrait.PowerOperation is not TraitOperation.None) {
                int newValue = enemyTrait.CalculateValue(enemyTrait.PowerOperation, enemyAI.EnSex.Power, enemyTrait.PowerValue);
                enemyAI.EnSex.Power = newValue;
            }

            if (enemyTrait.ResistanceOperation is not TraitOperation.None) {
                int newValue = enemyTrait.CalculateValue(enemyTrait.ResistanceOperation, enemyAI.EnSex.Resistance, enemyTrait.ResistanceValue);
                enemyAI.EnSex.Resistance = newValue;
            }

            if (enemyTrait.LustfulOperation is not TraitOperation.None) {
                int newValue = enemyTrait.CalculateValue(enemyTrait.LustfulOperation, enemyAI.EnSex.Prowess, enemyTrait.LustfulValue);
                enemyAI.EnSex.Prowess = newValue;
            }

            if (enemyTrait.RestraintChanceOperation is not TraitOperation.None) {
                int newValue = enemyTrait.CalculateValue(enemyTrait.RestraintChanceOperation, enemyAI.EnSex.RestraintChance, enemyTrait.RestraintChanceValue);
                enemyAI.EnSex.RestraintChance = newValue;
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
            return;
        }
    }

    internal static void Reset() {
        EnhancedCount = 0;
        MiniBossCount = 0;
    }

    private static List<EnemyTraitModel> GetDefaultTraits() {
        List<EnemyTraitModel> traits = [
            new() {
                Name = "Default",
                TraitType = TraitType.General,
                HealthOperation = TraitOperation.Random
            },
            new() {
                Name = "SexDoll",
                TraitType = TraitType.MiniBoss,
                Invulnerable = CharacterInvulnerable.Pleasure,
                HealthOperation = TraitOperation.Random,
                RestraintChanceOperation = TraitOperation.Replace,
                RestraintChanceValue = 0,
                RegenerationOperation = TraitOperation.Replace,
                RegenerationValue = 0,
            },
        ];

        return traits;
    }
}