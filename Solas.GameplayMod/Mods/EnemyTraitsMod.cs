using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using Solas.GameplayMod.Components;
using Solas.GameplayMod.Enums;
using Solas.GameplayMod.Models;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class EnemyTraitsMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<int> ChanceEnhanced;
    internal static MelonPreferences_Entry<int> MaxEnhanced;
    internal static MelonPreferences_Entry<int> ChanceMiniBoss;
    internal static MelonPreferences_Entry<int> MaxMiniBoss;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static int EnhancedCount;
    internal static int MiniBossCount;
    internal static List<EnemyTraitModel> EnemyTraits = [];

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(EnemyTraitsMod), nameof(Enabled), false,
                "Activates the modification", new AcceptableValueList<bool>([true, false]));
            ChanceEnhanced = config.Entry(nameof(EnemyTraitsMod), nameof(ChanceEnhanced), 20,
                "Chance for enemy get Enhanced traits", new AcceptableValueRange<int>(0, 100));
            MaxEnhanced = config.Entry(nameof(EnemyTraitsMod), nameof(MaxEnhanced), 15,
                "Max count of Enhanced traits on Stage", new AcceptableValueRange<int>(0, 100));
            ChanceMiniBoss = config.Entry(nameof(EnemyTraitsMod), nameof(ChanceMiniBoss), 10,
                "Chance for enemy get Mini boss traits", new AcceptableValueRange<int>(0, 100));
            MaxMiniBoss = config.Entry(nameof(EnemyTraitsMod), nameof(MaxMiniBoss), 5,
                "Max count of Mini boss traits on Stage", new AcceptableValueRange<int>(0, 100));

            if (Enabled.Value)
            {
                if (!JsonUtils.TryDeserialize(Core.PluginResources, "EnemyTraits.json", out List<EnemyTraitModel> enemyTraits))
                {
                    enemyTraits = GetDefaultTraits();
                    JsonUtils.TrySerialize(Core.PluginResources, "EnemyTraits.json", enemyTraits);
                }
                EnemyTraits = enemyTraits;
            }
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static EnemyTraitModel GetEnemyTrait(int enemyType)
    {
        try
        {
            if (!Enabled.Value)
            {
                return null;
            }

            IEnumerable<EnemyTraitModel> traits = EnemyTraits.Where(t => t.EnemyType is null || t.EnemyType == enemyType);
            if (!traits.Any())
            {
                return null;
            }

            TraitType type = TraitType.General;
            if (RandomUtils.Chance(ChanceMiniBoss.Value) && MiniBossCount < MaxMiniBoss.Value && traits.Any(t => t.TraitType is TraitType.MiniBoss))
            {
                type = TraitType.MiniBoss;
                MiniBossCount++;
            }
            else if (RandomUtils.Chance(ChanceEnhanced.Value) && EnhancedCount < MaxEnhanced.Value && traits.Any(t => t.TraitType is TraitType.Enhanced))
            {
                type = TraitType.Enhanced;
                EnhancedCount++;
            }

            return traits.Where(t => t.TraitType == type).RandomItem();
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
            return null;
        }
    }

    internal static void ApplyTrait(EnemyAI enemyAI, EnemyTraitModel enemyTrait)
    {
        try
        {
            if (!Enabled.Value || enemyTrait is null || enemyAI.IsBoss)
            {
                return;
            }

            if (enemyTrait.Health.Operation is not TraitOperation.None)
            {
                int origin = (enemyAI.healthSystem.MaxHp - 70) / 50;
                int newValue = enemyTrait.CalculateTrait(enemyTrait.Health, origin);
                enemyAI.healthSystem.MaxHp = 70 + newValue * 50;
                enemyAI.healthSystem.CurrentHp = enemyAI.healthSystem.MaxHp;
                enemyAI.healthSystem.SendHealthUpdateEvent();
            }

            if (enemyTrait.Armor.Operation is not TraitOperation.None)
            {
                int origin = enemyAI.healthSystem.Armor / 6;
                int newValue = enemyTrait.CalculateTrait(enemyTrait.Armor, origin);
                enemyAI.healthSystem.Armor = newValue * 6;
            }

            if (enemyTrait.Regeneration.Operation is not TraitOperation.None)
            {
                int newValue = enemyTrait.CalculateTrait(enemyTrait.Regeneration, enemyAI.healthSystem.Regeneration);

                int oldValue = enemyAI.healthSystem.Regeneration;
                enemyAI.healthSystem.Regeneration = newValue;

                if (oldValue == 0)
                {
                    enemyAI.healthSystem.StartCoroutine(enemyAI.healthSystem.RegenerationMotor());
                }
            }

            if (enemyTrait.Power.Operation is not TraitOperation.None)
            {
                int newValue = enemyTrait.CalculateTrait(enemyTrait.Power, enemyAI.EnSex.Power);
                enemyAI.EnSex.Power = newValue;
            }

            if (enemyTrait.Resistance.Operation is not TraitOperation.None)
            {
                int newValue = enemyTrait.CalculateTrait(enemyTrait.Resistance, enemyAI.EnSex.Resistance);
                enemyAI.EnSex.Resistance = newValue;
            }

            if (enemyTrait.Lustful.Operation is not TraitOperation.None)
            {
                int newValue = enemyTrait.CalculateTrait(enemyTrait.Lustful, enemyAI.EnSex.Prowess);
                enemyAI.EnSex.Prowess = newValue;
            }

            if (enemyTrait.RestraintChance.Operation is not TraitOperation.None)
            {
                int newValue = enemyTrait.CalculateTrait(enemyTrait.RestraintChance, enemyAI.EnSex.RestraintChance);
                enemyAI.EnSex.RestraintChance = newValue;
            }
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
            return;
        }
    }

    internal static void HealthDamage(HealthSystem healthSystem, ref int damage)
    {
        try
        {
            if (!Enabled.Value || SexSystem.Sexstatus is not SEXSTATUS.Fucking)
            {
                return;
            }

            if (!healthSystem.gameObject.TryGetComponentWithCast(out EnemyCharacterComponent enemyCharacter))
            {
                return;
            }

            if (enemyCharacter.EnemyTrait is not null && enemyCharacter.EnemyTrait.Invulnerable is TraitInvulnerable.SexHealth)
            {
                damage = 0;
            }

        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static void ArousalDamage(HealthSystem healthSystem, ref int damage)
    {
        try
        {
            if (!Enabled.Value || SexSystem.Sexstatus is not SEXSTATUS.Fucking)
            {
                return;
            }

            if (!healthSystem.gameObject.TryGetComponentWithCast(out EnemyCharacterComponent enemyCharacter))
            {
                return;
            }

            if (enemyCharacter.EnemyTrait is not null && enemyCharacter.EnemyTrait.Invulnerable is TraitInvulnerable.Pleasure)
            {
                damage = 0;
            }

        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static void Reset()
    {
        EnhancedCount = 0;
        MiniBossCount = 0;
    }

    private static List<EnemyTraitModel> GetDefaultTraits()
    {
        List<EnemyTraitModel> traits = [
            new() {
                Name = "Default",
                TraitType = TraitType.General,
                Armor = new(),
                Health = new EnemyTraitModel.Trait{ Operation = TraitOperation.Random },
                Lustful = new(),
                Power = new(),
                Regeneration = new(),
                Resistance = new(),
                RestraintChance = new(),
            },
            new() {
                Name = "SexDoll",
                TraitType = TraitType.Enhanced,
                Invulnerable = TraitInvulnerable.Pleasure,
                Armor = new(),
                Health = new() { Operation = TraitOperation.Random },
                Lustful = new(),
                Power = new(),
                Regeneration = new() { Operation = TraitOperation.Replace, Value = 2 },
                Resistance = new(),
                RestraintChance = new() { Operation = TraitOperation.Replace, Value = 0 },
            },
            new() {
                Name = "SexDrone",
                TraitType = TraitType.MiniBoss,
                Invulnerable = TraitInvulnerable.Pleasure,
                Armor = new(),
                Health = new() { Operation = TraitOperation.Random },
                Lustful = new() { Operation = TraitOperation.Random },
                Power = new() { Operation = TraitOperation.Random },
                Regeneration = new() { Operation = TraitOperation.Replace, Value = 2 },
                Resistance = new(),
                RestraintChance = new() { Operation = TraitOperation.Replace, Value = 0 },
            },
            new() {
                Name = "Vampire",
                TraitType = TraitType.MiniBoss,
                Invulnerable = TraitInvulnerable.SexHealth,
                Armor = new() { Operation = TraitOperation.Random },
                Health = new() { Operation = TraitOperation.Random },
                Lustful = new() { Operation = TraitOperation.Random },
                Power = new() { Operation = TraitOperation.Random },
                Regeneration = new() { Operation = TraitOperation.Replace, Value = 5 },
                Resistance = new() { Operation = TraitOperation.Random },
                RestraintChance = new() { Operation = TraitOperation.Replace, Value = 5 },
            },
        ];

        return traits;
    }
}
