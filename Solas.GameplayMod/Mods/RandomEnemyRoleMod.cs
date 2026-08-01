using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using Solas.GameplayMod.Components;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class RandomEnemyRoleMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<int> MaleChance;
    internal static MelonPreferences_Entry<int> FemaleChance;
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

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(RandomEnemyRoleMod), nameof(Enabled), false,
                "Activates the modification", new AcceptableValueList<bool>([true, false]));
            FemaleChance = config.Entry(nameof(RandomEnemyRoleMod), nameof(FemaleChance), 15,
                "Chance for a female character to change to the opposite role", new AcceptableValueRange<int>(0, 100));
            MaleChance = config.Entry(nameof(RandomEnemyRoleMod), nameof(MaleChance), 15,
                "Chance for a male character to change to the opposite role", new AcceptableValueRange<int>(0, 100));

        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static void Apply(SexSystem sexSystem)
    {
        try
        {
            if (!Enabled.Value)
            {
                return;
            }

            if (sexSystem.Enemy.TryGetComponentWithCast(out EnemyCharacterComponent enemyComponent) && enemyComponent.IsActive is not null)
            {
                if (SexSystem.PlayerAttacker)
                {
                    sexSystem.TargetActive = enemyComponent.IsActive.Value;
                    sexSystem.TargetFuta = !enemyComponent.EnemyAI.EnSex.EnemyMale && enemyComponent.IsActive.Value;
                }
                else
                {
                    sexSystem.CasterActive = enemyComponent.IsActive.Value;
                    sexSystem.CasterFuta = !enemyComponent.EnemyAI.EnSex.EnemyMale && enemyComponent.IsActive.Value;
                }

                if (sexSystem.IsThreesome)
                {
                    sexSystem.AssistActive = sexSystem.CasterActive;
                    sexSystem.AssistFuta = !sexSystem.AssistMale && sexSystem.CasterFuta;
                }
            }

        }
        catch (Exception ex)
        {
            Core.LogError(ex);
            return;
        }
    }

    internal static bool? GetRandomRole(bool isMale)
    {
        bool? activeDefault = null;

        if (isMale && Character.adultSettingsDATA.EnemyRoleM != 2)
        {
            activeDefault = Character.adultSettingsDATA.EnemyRoleM == 1;
        }
        else if (!isMale && Character.adultSettingsDATA.EnemyRole != 2)
        {
            activeDefault = Character.adultSettingsDATA.EnemyRole == 1;
        }

        if (activeDefault is null)
        {
            return null;
        }

        int chance = isMale ? MaleChance.Value : FemaleChance.Value;

        return RandomUtils.Chance(chance, !activeDefault, activeDefault);
    }
}
