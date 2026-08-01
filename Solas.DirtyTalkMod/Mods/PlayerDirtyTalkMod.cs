using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using Solas.DirtyTalkMod.Components;
using Solas.DirtyTalkMod.Models;
using UnityEngine.SceneManagement;

namespace Solas.DirtyTalkMod.Mods;
internal class PlayerDirtyTalkMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;

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

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(PlayerDirtyTalkMod), nameof(Enabled), false,
                "Activates the modification", new ModConfig.AcceptableValueList<bool>([true, false]));

            if (Enabled.Value)
            {
                if (!JsonUtils.TryDeserialize(Core.PluginConfigs, $"PlayerMaleThought.json", out PlayerDirtyTalkModel maleConfig))
                {
                    maleConfig = new PlayerDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(Core.PluginConfigs, $"PlayerMaleThought.json", maleConfig);
                }
                MalePlayer = maleConfig;

                if (!JsonUtils.TryDeserialize(Core.PluginConfigs, $"PlayerFemaleThought.json", out PlayerDirtyTalkModel femaleConfig))
                {
                    femaleConfig = new PlayerDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(Core.PluginConfigs, $"PlayerFemaleThought.json", maleConfig);
                }
                FemalePlayer = femaleConfig;

                if (!JsonUtils.TryDeserialize(Core.PluginConfigs, $"PlayerFutaThought.json", out PlayerDirtyTalkModel futaConfig))
                {
                    futaConfig = new PlayerDirtyTalkModel();
                    _ = JsonUtils.TrySerialize(Core.PluginConfigs, $"PlayerFutaThought.json", maleConfig);
                }
                FutaPlayer = futaConfig;

                if (JsonUtils.TryDeserializeFolder(Path.Combine(Core.PluginConfigs, "ForEnemyTypes"), "Player*.json", out List<PlayerDirtyTalkEnemyTypeModel> list))
                {
                    PlayerEnemyTypes.Clear();
                    PlayerEnemyTypes.AddRange(list);
                    Core.LogInfo($"Load {list.Count} thoughts for enemy types");
                }
            }
        }
        catch (Exception ex)
        {
            Core.LogError( ex );
        }
    }

    internal static void ApplyPlayerDirtyTalk(PlayerCombat playerCombat)
    {
        try
        {
            if (!Enabled.Value || Character.adultSettingsDATA.DtalkInner || SceneManager.GetActiveScene().buildIndex < 4)
            {
                return;
            }

            if (MalePlayer is null || FemalePlayer is null || FutaPlayer is null)
            {
                return;
            }

            playerCombat.AddModComponent<PlayerDirtyTalkComponent>();
        }
        catch (Exception ex)
        {
            Core.LogError( ex );
            return;
        }
    }
}
