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
internal class PlayerDirtyTalkMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;

    internal static PlayerDirtyTalkModel FemalePlayer;
    internal static PlayerDirtyTalkModel FutaPlayer;
    internal static PlayerDirtyTalkModel MalePlayer;
    internal static List<PlayerDirtyTalkEnemyTypeModel> PlayerEnemyTypes = [];
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    #region Storage
    internal static CharacterData Character => CharacterData.Instance;
    #endregion

    internal static void Load(ConfigFile config) {
        try {
            Enabled = config.Bind(nameof(PlayerDirtyTalkMod), nameof(Enabled), false,
                new ConfigDescription("Activates the modification", new AcceptableValueList<bool>([true, false])));

            if (Enabled.Value) {
                if (!JsonUtils.TryDeserialize(Plugin.PluginResources, $"PlayerMaleThought.json", out PlayerDirtyTalkModel maleConfig)) {
                    maleConfig = new PlayerDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(Plugin.PluginResources, $"PlayerMaleThought.json", maleConfig);
                }
                MalePlayer = maleConfig;

                if (!JsonUtils.TryDeserialize(Plugin.PluginResources, $"PlayerFemaleThought.json", out PlayerDirtyTalkModel femaleConfig)) {
                    femaleConfig = new PlayerDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(Plugin.PluginResources, $"PlayerFemaleThought.json", maleConfig);
                }
                FemalePlayer = femaleConfig;

                if (!JsonUtils.TryDeserialize(Plugin.PluginResources, $"PlayerFutaThought.json", out PlayerDirtyTalkModel futaConfig)) {
                    futaConfig = new PlayerDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(Plugin.PluginResources, $"PlayerFutaThought.json", maleConfig);
                }
                FutaPlayer = futaConfig;

                if (JsonUtils.TryDeserializeFolder(Path.Combine(Plugin.PluginResources, "ForEnemyTypes"), "Player*.json", out List<PlayerDirtyTalkEnemyTypeModel> list)) {
                    PlayerEnemyTypes.Clear();
                    PlayerEnemyTypes.AddRange(list);
                    Plugin.Log.Info($"Load {list.Count} thoughts for enemy types");
                }
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void ApplyPlayerDirtyTalk(PlayerCombat playerCombat) {
        try {
            if (!Enabled.Value || Character.adultSettingsDATA.DtalkInner || SceneManager.GetActiveScene().buildIndex < 4)
                return;

            if (MalePlayer is null || FemalePlayer is null || FutaPlayer is null)
                return;

            PlayerDirtyTalkComponent.RegisterClass(playerCombat);
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return;
        }
    }
}