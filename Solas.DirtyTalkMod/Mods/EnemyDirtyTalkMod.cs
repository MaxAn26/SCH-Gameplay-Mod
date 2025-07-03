using System;
using System.Collections.Generic;
using System.IO;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

using Solas.DirtyTalkMod.Components;
using Solas.DirtyTalkMod.Models;

using UnityEngine.SceneManagement;

namespace Solas.DirtyTalkMod.Mods;
internal class EnemyDirtyTalkMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<int> AssistChance;

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

    internal static void Load(ConfigFile config) {
        try {
            Enabled = config.Bind(nameof(EnemyDirtyTalkMod), nameof(Enabled), false,
                new ConfigDescription("Activates the modification", new AcceptableValueList<bool>([true, false])));
            AssistChance = config.Bind(nameof(EnemyDirtyTalkMod), nameof(AssistChance), 20,
                new ConfigDescription("Chance for using assist taunts instead of enemy taunt in threesome", new AcceptableValueRange<int>(0, 100)));

            if (Enabled.Value) {
                if (!JsonUtils.TryDeserialize(Plugin.PluginResources, $"EnemyMaleTaunt.json", out EnemyDirtyTalkModel maleConfig)) {
                    maleConfig = new EnemyDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(Plugin.PluginResources, $"EnemyMaleTaunt.json", maleConfig);
                }
                MaleEnemy = maleConfig;

                if (!JsonUtils.TryDeserialize(Plugin.PluginResources, $"EnemyFemaleTaunt.json", out EnemyDirtyTalkModel femaleConfig)) {
                    femaleConfig = new EnemyDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(Plugin.PluginResources, $"EnemyFemaleTaunt.json", maleConfig);
                }
                FemaleEnemy = femaleConfig;

                if (!JsonUtils.TryDeserialize(Plugin.PluginResources, $"EnemyFutaTaunt.json", out EnemyDirtyTalkModel futaConfig)) {
                    futaConfig = new EnemyDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(Plugin.PluginResources, $"EnemyFutaTaunt.json", maleConfig);
                }
                FutaEnemy = futaConfig;

                if (JsonUtils.TryDeserializeFolder(Path.Combine(Plugin.PluginResources, "ForEnemyTypes"), "Enemy*.json", out List<EnemyTypeDirtyTalkModel> list)) {
                    EnemyTypes.Clear();
                    EnemyTypes.AddRange(list);
                    Plugin.Log.Info($"Load {list.Count} taunts for enemy types");
                }
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void ApplyEnemyDirtyTalk(EnemyAI enemyAI) {
        try {
            if (!Enabled.Value || Character.adultSettingsDATA.DtalkEnemy || SceneManager.GetActiveScene().buildIndex < 4)
                return;

            if (MaleEnemy is null || FemaleEnemy is null || FutaEnemy is null)
                return;

            EnemyDirtyTalkComponent.RegisterClass(enemyAI);
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return;
        }
    }
}