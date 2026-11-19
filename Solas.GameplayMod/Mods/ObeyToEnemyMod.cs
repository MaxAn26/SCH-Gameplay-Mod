using BaseMod.Core;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using Solas.GameplayMod.Enums;
using Solas.GameplayMod.Models;

using UnityEngine.SceneManagement;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class ObeyToEnemyMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;

    internal static ObeyToEnemyConfig ObeyToEnemy;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static bool IsActivated { get; set; }
    internal static bool IsInitialized { get; set; }
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(ObeyToEnemyMod), nameof(Enabled), false,
                "Activates the modification", new AcceptableValueList<bool>([true, false]));

            if (Enabled.Value)
            {
                if (!JsonUtils.TryDeserialize(Core.PluginConfigs, $"ObeyToEnemy.json", out ObeyToEnemyConfig obeyConfig))
                {
                    obeyConfig = new ObeyToEnemyConfig();
                    _ = JsonUtils.TrySerialize(Core.PluginConfigs, $"ObeyToEnemy.json", obeyConfig);
                }
                ObeyToEnemy = obeyConfig;
            }
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
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            if (!IsInitialized && !IsActivated)
            {
                ObeyToEnemyConfig.Conditions config = null;
                if (sexSystem.PlayerMale)
                {
                    config = ObeyToEnemy.Male;
                }
                else if (Character.adultSettingsDATA.FutaPlayerMode)
                {
                    config = ObeyToEnemy.Futa;
                }
                else
                {
                    config = ObeyToEnemy.Female;
                }

                if (config is null)
                {
                    return;
                }

                if (Character.statusDATA.IsBoundHandRestraint > 0)
                {
                    if (config.Base.HasFlag(ObeyCondition.Bound))
                    {
                        Core.LogInfo("Set ObeyToEnemy due hand bound");
                        IsActivated = true;
                    }

                    if (config.Victim.HasFlag(ObeyCondition.Bound))
                    {
                        PlayerVictim(sexSystem);
                    }
                }


                if (sexSystem.TargetActive == sexSystem.CasterActive)
                {
                    if (config.Base.HasFlag(ObeyCondition.SameRole))
                    {
                        Core.LogInfo("Set ObeyToEnemy due equal roles");
                        IsActivated = true;
                    }

                    if (config.Victim.HasFlag(ObeyCondition.SameRole))
                    {
                        PlayerVictim(sexSystem);
                    }
                }

                if (sexSystem.TargetMale == sexSystem.CasterMale)
                {
                    if (config.Base.HasFlag(ObeyCondition.SameGender))
                    {
                        Core.LogInfo("Set ObeyToEnemy due equal gender");
                        IsActivated = true;
                    }

                    if (config.Victim.HasFlag(ObeyCondition.SameGender))
                    {
                        PlayerVictim(sexSystem);
                    }
                }

                if (RandomUtils.Chance(sexSystem.EnemyPower * 2))
                {
                    if (config.Base.HasFlag(ObeyCondition.EnemyPower))
                    {
                        Core.LogInfo("Set ObeyToEnemy due enemy power random");
                        IsActivated = true;
                    }

                    if (config.Victim.HasFlag(ObeyCondition.EnemyPower))
                    {
                        PlayerVictim(sexSystem);
                    }
                }

                if (IsActivated)
                {
                    sexSystem.console.ConsoleWrite("Obeying enemy");
                }

                IsInitialized = true;
            }
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
            return;
        }
    }

    internal static void Reset()
    {
        IsActivated = false;
        IsInitialized = false;
    }

    private static void PlayerVictim(SexSystem sexSystem)
    {
        if (!SexSystem.PlayerAttacker)
        {
            return;
        }

        SexSystem.PlayerAttacker = false;
        (sexSystem.Caster, sexSystem.Target) = (sexSystem.Target, sexSystem.Caster);
        (sexSystem.CasterAnim, sexSystem.TargetAnim) = (sexSystem.TargetAnim, sexSystem.CasterAnim);
        (sexSystem.CasterActive, sexSystem.TargetActive) = (sexSystem.TargetActive, sexSystem.CasterActive);
        (sexSystem.CasterFuta, sexSystem.TargetFuta) = (sexSystem.TargetFuta, sexSystem.CasterFuta);
        (sexSystem.CasterMale, sexSystem.TargetMale) = (sexSystem.TargetMale, sexSystem.CasterMale);
    }
}
