using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using Solas.GameplayMod.Components;
using Solas.GameplayMod.Models;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Solas.GameplayMod.Mods;
internal class EnemySexExtendMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> UseEnemyThreesomeChance;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static List<EnemySexModel> EnemySexTypes { get; set; } = [];
    #endregion

    internal static CharacterData Character => CharacterData.Instance;

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(EnemySexExtendMod), nameof(Enabled), false,
                "Activates the modification", new ModConfig.AcceptableValueList<bool>([true, false]));
            UseEnemyThreesomeChance = config.Entry(nameof(EnemySexExtendMod), nameof(UseEnemyThreesomeChance), false,
                "Use threesome chance according with enemy type", new ModConfig.AcceptableValueList<bool>([true, false]));

            if (Enabled.Value)
            {
                if (!JsonUtils.TryDeserialize(GameplayMod.PluginResources, "EnemySexTypes.json", out List<EnemySexModel> enemySexType))
                {
                    enemySexType = GetEnemySexTypes();
                    JsonUtils.TrySerialize(GameplayMod.PluginResources, "EnemySexTypes.json", enemySexType);
                }
                EnemySexTypes = enemySexType;
            }
            else
            {
                UseEnemyThreesomeChance.Value = false;
            }
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
        }
    }

    internal static void UpdateThreesome(SexSystem sexSystem)
    {
        try
        {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            if (!UseEnemyThreesomeChance.Value)
            {
                return;
            }

            int chance = 0;
            if (!SexSystem.PlayerAttacker)
            {
                EnemyCharacterComponent enemyComponent = sexSystem.Enemy.GetComponentWithCast<EnemyCharacterComponent>();
                if (enemyComponent is not null && enemyComponent.EnemySexTypes is not null)
                {
                    chance = enemyComponent.EnemySexTypes.ThreesomeChance;
                    if (enemyComponent.EnemyFetish.HasFlag(Fetish.Dominant) || enemyComponent.EnemyFetish.HasFlag(Fetish.Submissive)
                        || enemyComponent.EnemyFetish.HasFlag(Fetish.Romance))
                    {
                        chance -= 10;
                    }
                    if (enemyComponent.EnemyFetish.HasFlag(Fetish.Wrestling))
                    {
                        chance += 10;
                    }
                }
            }

            Character.adultSettingsDATA.ThreesomeChance = chance;
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex);
        }
    }

    internal static int CalculateCorruption(int baseChance, Fetish fetishes)
    {
        int chance = baseChance;

        if (fetishes.HasFlag(Fetish.Submissive))
        {
            chance += 10;
        }
        else if (fetishes.HasFlag(Fetish.Romance))
        {
            chance -= 15;
        }

        return Mathf.Clamp(chance, 0, 100);
    }

    internal static EnemySexModel GetEnemySexModel(int enemyTypeId)
    {
        try
        {
            if (!Enabled.Value)
            {
                return null;
            }

            return EnemySexTypes.Where(t => t.EnemyType == enemyTypeId).RandomItem();
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex);
            return null;
        }
    }

    internal static void ReduceEnemyPower(SexSystem sexSystem)
    {
        if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
        {
            return;
        }

        if (sexSystem.Enemy.TryGetComponentWithCast(out EnemyCharacterComponent enemyCharacter))
        {
            if (IsFetishTriggered(sexSystem, enemyCharacter.EnemyFetish) || IsWeaknessesTriggered(sexSystem, enemyCharacter.AsCasterWeaknesses, enemyCharacter.AsTargetWeaknesses))
            {
                sexSystem.EnemyPower = Mathf.RoundToInt(sexSystem.EnemyPower * 0.8f);
            }
        }
    }

    internal static bool IsFetishTriggered(SexSystem sexSystem, Fetish fetishes)
    {
        try
        {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return false;
            }

            if (SexMoveChoiceMod.IsModActive && SexMoveChoiceMod.LastSexMove is null || sexSystem.IsThreesome)
            {
                return false;
            }

            if (fetishes.HasFlag(Fetish.Romance) && SexMoveChoiceMod.LastSexMove.IsSensual)
            {
                return true;
            }
            else if (fetishes.HasFlag(Fetish.BreathPlay) && SexMoveChoiceMod.LastSexMove.IsSmothering)
            {
                return true;
            }

            if (SexSystem.PlayerAttacker)
            {
                if (fetishes.HasFlag(Fetish.Submissive) && SexMoveChoiceMod.LastSexMove.IsService)
                {
                    return true;
                }
                else if (fetishes.HasFlag(Fetish.Masochist) && SexMoveChoiceMod.LastSexMove.IsSpanking && !SexMoveChoiceMod.LastSexMove.IsCommand)
                {
                    return true;
                }
                else if (fetishes.HasFlag(Fetish.Sadist) && SexMoveChoiceMod.LastSexMove.IsSpanking && SexMoveChoiceMod.LastSexMove.IsCommand)
                {
                    return true;
                }
            }
            else
            {
                if (fetishes.HasFlag(Fetish.Dominant) && SexMoveChoiceMod.LastSexMove.IsDominant)
                {
                    return true;
                }
                else if (fetishes.HasFlag(Fetish.Masochist) && SexMoveChoiceMod.LastSexMove.IsSpanking && SexMoveChoiceMod.LastSexMove.IsCommand)
                {
                    return true;
                }
                else if (fetishes.HasFlag(Fetish.Sadist) && SexMoveChoiceMod.LastSexMove.IsSpanking && !SexMoveChoiceMod.LastSexMove.IsCommand)
                {
                    return true;
                }
                else if (fetishes.HasFlag(Fetish.Wrestling) && SexMoveChoiceMod.LastSexMove.IsWresting)
                {
                    return true;
                }
                else if (fetishes.HasFlag(Fetish.Bondage))
                {
                    if (Character.statusDATA.IsBoundCollar > 0 || Character.statusDATA.IsBoundGag > 0 || Character.statusDATA.IsBoundHandRestraint > 0
                        || Character.statusDATA.IsBoundLegRestraint > 0 || Character.statusDATA.IsBoundHeavyRestraint > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
            return false;
        }
    }

    internal static bool IsWeaknessesTriggered(SexSystem sexSystem, Weaknesses asCaster, Weaknesses asTarget)
    {
        try
        {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return false;
            }

            if (SexMoveChoiceMod.IsModActive && SexMoveChoiceMod.LastSexMove is null || sexSystem.IsThreesome)
            {
                return false;
            }

            Weaknesses weaknesses = Weaknesses.NoWeaknesses;
            switch (SexMoveChoiceMod.LastSexMove.Type)
            {
                case 1:
                    weaknesses = Weaknesses.Orals;
                    break;
                case 2:
                    weaknesses = Weaknesses.HandJobs;
                    break;
                case 3:
                    weaknesses = Weaknesses.BreastPlay;
                    break;
                case 4:
                    weaknesses = Weaknesses.FootPlay;
                    break;
                case 6:
                    weaknesses = Weaknesses.MissionaryStyle;
                    break;
                case 7:
                    weaknesses = Weaknesses.DoggyStyle;
                    break;
                case 8:
                    weaknesses = Weaknesses.MountingStyle;
                    break;
                default:
                    break;
            }

            if (SexSystem.PlayerAttacker)
            {
                return asTarget.HasFlag(weaknesses);
            }
            else
            {
                return asCaster.HasFlag(weaknesses);
            }
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
            return false;
        }
    }

    internal static void CheckFetishAndWeaknesses(HealthSystem healthSystem, ref int ecstasyDamage)
    {
        try
        {
            if (!Enabled.Value)
            {
                return;
            }

            if (healthSystem.gameObject.TryGetComponentWithCast(out EnemyCharacterComponent enemyCharacter))
            {
                if (IsFetishTriggered(enemyCharacter.SexSystem, enemyCharacter.EnemyFetish) || IsWeaknessesTriggered(enemyCharacter.SexSystem, enemyCharacter.AsCasterWeaknesses, enemyCharacter.AsTargetWeaknesses))
                {
                    ecstasyDamage = RandomUtils.Chance(20, ecstasyDamage * 2, ecstasyDamage);
                }
            }
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
        }
    }

    private static List<EnemySexModel> GetEnemySexTypes()
    {
        List<EnemySexModel> enemySexTypes = [
            new() {
                Name = "Master",
                EnemyType = 0,
                CorruptionChance = 5,
                ThreesomeChance = 20,
                AllowedFetishes = Fetish.NoFetish | Fetish.Dominant | Fetish.Sadist | Fetish.Romance | Fetish.Bondage,
                CasterWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.MissionaryStyle | Weaknesses.MountingStyle,
                TargetWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.Orals | Weaknesses.BreastPlay,
                UseTag = true,
                SexTags = SexTag.Dominant | SexTag.Smothering | SexTag.Punishment
            },
            new() {
                Name = "Fighter",
                EnemyType = 1,
                CorruptionChance = 10,
                ThreesomeChance = 10,
                AllowedFetishes = Fetish.NoFetish | Fetish.Dominant | Fetish.Sadist | Fetish.Wrestling,
                CasterWeaknesses = Weaknesses.DoggyStyle | Weaknesses.MountingStyle | Weaknesses.NoWeaknesses,
                TargetWeaknesses = Weaknesses.BreastPlay | Weaknesses.Orals | Weaknesses.FootPlay | Weaknesses.NoWeaknesses,
                UseTag = true,
                SexTags = SexTag.Wrestling | SexTag.Dominant | SexTag.Punishment
            },
            new() {
                Name = "Ninja",
                EnemyType = 2,
                CorruptionChance = 20,
                ThreesomeChance = 5,
                AllowedFetishes = Fetish.NoFetish | Fetish.Romance | Fetish.BreathPlay,
                CasterWeaknesses = Weaknesses.NoWeaknesses,
                TargetWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.Orals | Weaknesses.HandJobs,
                UseTag = true,
                SexTags = SexTag.Wrestling | SexTag.Smothering
            },
            new() {
                Name = "Police",
                EnemyType = 3,
                CorruptionChance = 15,
                ThreesomeChance = 15,
                AllowedFetishes = Fetish.NoFetish | Fetish.Dominant | Fetish.Sadist | Fetish.Wrestling,
                CasterWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.DoggyStyle,
                TargetWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.BreastPlay | Weaknesses.FootPlay,
                UseTag = true,
                SexTags = SexTag.Dominant | SexTag.Smothering
            },
            new() {
                Name = "Servant",
                EnemyType = 4,
                CorruptionChance = 30,
                ThreesomeChance = 5,
                AllowedFetishes = Fetish.NoFetish | Fetish.Romance | Fetish.Submissive | Fetish.Masochist,
                CasterWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.HandJobs,
                TargetWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.FootPlay | Weaknesses.BreastPlay | Weaknesses.MissionaryStyle,
                UseTag = true,
                SexTags = SexTag.Service | SexTag.Sensual
            },
            new() {
                Name = "Thug",
                EnemyType = 5,
                CorruptionChance = 25,
                ThreesomeChance = 25,
                AllowedFetishes = Fetish.NoFetish | Fetish.Sadist | Fetish.Masochist | Fetish.Bondage,
                CasterWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.FootPlay | Weaknesses.DoggyStyle,
                TargetWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.BreastPlay | Weaknesses.HandJobs,
                UseTag = true,
                SexTags = SexTag.NoTag
            },
            new() {
                Name = "Villain",
                EnemyType = 6,
                CorruptionChance = 5,
                ThreesomeChance = 35,
                AllowedFetishes = Fetish.NoFetish | Fetish.Dominant | Fetish.Sadist | Fetish.Romance,
                CasterWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.Orals | Weaknesses.MountingStyle,
                TargetWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.BreastPlay | Weaknesses.MissionaryStyle,
                UseTag = true,
                SexTags = SexTag.NoTag
            },
            new() {
                Name = "Succubus",
                EnemyType = 7,
                CorruptionChance = 15,
                ThreesomeChance = 40,
                AllowedFetishes = Fetish.NoFetish | Fetish.Dominant | Fetish.Romance | Fetish.Masochist | Fetish.BreathPlay,
                CasterWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.DoggyStyle | Weaknesses.MountingStyle,
                TargetWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.BreastPlay | Weaknesses.FootPlay,
                UseTag = true,
                SexTags = SexTag.Sensual | SexTag.Service | SexTag.Smothering
            },
            new() {
                Name = "Worker",
                EnemyType = 8,
                CorruptionChance = 20,
                ThreesomeChance = 10,
                AllowedFetishes = Fetish.NoFetish | Fetish.Submissive | Fetish.Romance,
                CasterWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.MountingStyle | Weaknesses.DoggyStyle,
                TargetWeaknesses = Weaknesses.NoWeaknesses | Weaknesses.Orals | Weaknesses.BreastPlay | Weaknesses.FootPlay,
                UseTag = true,
                SexTags = SexTag.NoTag
            }
        ];

        return enemySexTypes;
    }
}
