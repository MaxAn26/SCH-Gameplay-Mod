using System;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

using Solas.GameplayMod.Components;

namespace Solas.GameplayMod.Mods;
internal class RandomEnemyRoleMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<int> MaleChance;
    internal static ConfigEntry<int> FemaleChance;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static bool? MaleDefaultActive = null;
    internal static bool? FemaleDefaultActive = null;

    internal static bool? EnemyActive = null;
    internal static bool EnemyFuta = false;
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    #endregion

    internal static void Load(ConfigFile config) {
        try {
            Enabled = config.Bind(nameof(RandomEnemyRoleMod), nameof(Enabled), false,
                new ConfigDescription("Activates the modification", new AcceptableValueList<bool>([true, false])));
            FemaleChance = config.Bind(nameof(RandomEnemyRoleMod), nameof(FemaleChance), 15,
                            new ConfigDescription("Chance for a female character to change to the opposite role", new AcceptableValueRange<int>(0, 100)));
            MaleChance = config.Bind(nameof(RandomEnemyRoleMod), nameof(MaleChance), 15,
                new ConfigDescription("Chance for a male character to change to the opposite role", new AcceptableValueRange<int>(0, 100)));

        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void Apply( SexSystem sexSystem) {
        try {
            if (!Enabled.Value)
                return;

            if (sexSystem.Enemy.TryGetComponentWithCast(out EnemyCharacterComponent enemyComponent) && enemyComponent.IsActive is not null) {
                if (SexSystem.PlayerAttacker) {
                    sexSystem.TargetActive = enemyComponent.IsActive.Value;
                    sexSystem.TargetFuta = !enemyComponent.EnemyAI.EnSex.EnemyMale && enemyComponent.IsActive.Value;
                } else {
                    sexSystem.CasterActive = enemyComponent.IsActive.Value;
                    sexSystem.CasterFuta = !enemyComponent.EnemyAI.EnSex.EnemyMale && enemyComponent.IsActive.Value;
                }

                if (sexSystem.IsThreesome) {
                    sexSystem.AssistActive = sexSystem.CasterActive;
                    sexSystem.AssistFuta = !sexSystem.AssistMale && sexSystem.CasterFuta;
                }
            }

        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return;
        }
    }

    internal static bool? GetRandomRole(bool isMale) {
        bool? activeDefault = null;

        if (isMale && Character.adultSettingsDATA.EnemyRoleM != 2)             
            activeDefault = Character.adultSettingsDATA.EnemyRoleM == 1;
        else if (!isMale && Character.adultSettingsDATA.EnemyRole != 2) {
            activeDefault = Character.adultSettingsDATA.EnemyRole == 1;
        }

        if (activeDefault is null)            
            return null;

        int chance = isMale ? MaleChance.Value : FemaleChance.Value;

        return RandomUtils.Chance(chance, !activeDefault, activeDefault);
    }
}