using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using Solas.DirtyTalkMod.Components;
using Solas.DirtyTalkMod.Models;

using UnityEngine.SceneManagement;

namespace Solas.DirtyTalkMod.Mods;
internal class EnemyDirtyTalkMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> AllowAlways;
    internal static MelonPreferences_Entry<int> AssistChance;

    internal static EnemyDirtyTalkModel FemaleEnemy;
    internal static EnemyDirtyTalkModel FutaEnemy;
    internal static EnemyDirtyTalkModel MaleEnemy;
    internal static List<EnemyTypeDirtyTalkModel> EnemyTypes = [];
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(EnemyDirtyTalkMod), nameof(Enabled), false,
                "Activates the modification", new ModConfig.AcceptableValueList<bool>([true, false]));
            AllowAlways = config.Entry(nameof(EnemyDirtyTalkMod), nameof(AllowAlways), false,
                "Allow enemy speak when they cannot do it in reality: during lick or suck, or when gagged", new ModConfig.AcceptableValueList<bool>([true, false]));
            AssistChance = config.Entry(nameof(EnemyDirtyTalkMod), nameof(AssistChance), 20,
                "Chance for using assist taunts instead of enemy taunt in threesome", new ModConfig.AcceptableValueRange<int>(0, 100));

            if (Enabled.Value)
            {
                if (!JsonUtils.TryDeserialize(DirtyTalkMod.PluginConfigs, $"EnemyMaleTaunt.json", out EnemyDirtyTalkModel maleConfig))
                {
                    maleConfig = new EnemyDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(DirtyTalkMod.PluginConfigs, $"EnemyMaleTaunt.json", maleConfig);
                }
                MaleEnemy = maleConfig;

                if (!JsonUtils.TryDeserialize(DirtyTalkMod.PluginConfigs, $"EnemyFemaleTaunt.json", out EnemyDirtyTalkModel femaleConfig))
                {
                    femaleConfig = new EnemyDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(DirtyTalkMod.PluginConfigs, $"EnemyFemaleTaunt.json", maleConfig);
                }
                FemaleEnemy = femaleConfig;

                if (!JsonUtils.TryDeserialize(DirtyTalkMod.PluginConfigs, $"EnemyFutaTaunt.json", out EnemyDirtyTalkModel futaConfig))
                {
                    futaConfig = new EnemyDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(DirtyTalkMod.PluginConfigs, $"EnemyFutaTaunt.json", maleConfig);
                }
                FutaEnemy = futaConfig;

                if (JsonUtils.TryDeserializeFolder(Path.Combine(DirtyTalkMod.PluginConfigs, "ForEnemyTypes"), "Enemy*.json", out List<EnemyTypeDirtyTalkModel> list))
                {
                    EnemyTypes.Clear();
                    EnemyTypes.AddRange(list);
                    DirtyTalkMod.Log.Msg($"Load {list.Count} taunts for enemy types");
                }
            }
        }
        catch (Exception ex)
        {
            DirtyTalkMod.Log.Error(ex.Message);
        }
    }

    internal static void ApplyEnemyDirtyTalk(EnemyAI enemyAI)
    {
        try
        {
            if (!Enabled.Value || Character.adultSettingsDATA.DtalkEnemy || SceneManager.GetActiveScene().buildIndex < 4)
            {
                return;
            }

            if (MaleEnemy is null || FemaleEnemy is null || FutaEnemy is null)
            {
                return;
            }

            enemyAI.AddModComponent<EnemyDirtyTalkComponent>();
        }
        catch (Exception ex)
        {
            DirtyTalkMod.Log.Error(ex.Message);
            return;
        }
    }
}
