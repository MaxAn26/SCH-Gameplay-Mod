using BaseMod.Core;
using MelonLoader;
using MelonLoader.Utils;
using Solas.GameplayMod;
using Solas.GameplayMod.Mods;
using Solas.GameplayMod.Patches;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(GameplayMod), ModInfo.MOD_NAME, ModInfo.MOD_VERSION, ModInfo.MOD_DEVELOPER, ModInfo.MOD_URL)]
[assembly: MelonGame(ModInfo.GAME_DEVELOPER, ModInfo.GAME_NAME)]

namespace Solas.GameplayMod;
public class GameplayMod : MelonMod
{
    internal static MelonLogger.Instance Log;
    internal static ModConfig PluginConfig;
    internal static string ConfigPath;
    internal static string PluginAssets;
    internal static string PluginConfigs;
    internal static string PluginResources;

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        // GameplayMod startup logic
        Log = LoggerInstance;
        ConfigPath = MelonEnvironment.UserDataDirectory;
        PluginAssets = Path.Combine(ConfigPath, ModInfo.MOD_GUID, "Assets");
        PluginConfigs = Path.Combine(ConfigPath, ModInfo.MOD_GUID, "Configs");
        PluginResources = Path.Combine(ConfigPath, ModInfo.MOD_GUID, "Resources");

        PluginConfig = new($"{ModInfo.MOD_GUID}.cfg");

        CapturedSlaveMod.Load(PluginConfig);
        CriticalHitMod.Load(PluginConfig);
        EnemyHPResetMod.Load(PluginConfig);
        EnemySexExtendMod.Load(PluginConfig);
        EnemyTraitsMod.Load(PluginConfig);
        ExtraBondageTrapMod.Load(PluginConfig);
        FuckMeMod.Load(PluginConfig);
        GlossEffectMod.Load(PluginConfig);
        LustCageMod.Load(PluginConfig);
        ObeyToEnemyMod.Load(PluginConfig);
        OrgasmControlMod.Load(PluginConfig);
        RandomEnemyRoleMod.Load(PluginConfig);
        RandomFutaMod.Load(PluginConfig);
        RandomReverseMod.Load(PluginConfig);
        SexDamageMod.Load(PluginConfig);
        SexInitiatorStateMod.Load(PluginConfig);
        SexMoveChoiceMod.Load(PluginConfig);

        HarmonyInstance.PatchAll(typeof(CaptureSystemPatch));
        HarmonyInstance.PatchAll(typeof(EnemyActionsPatch));
        HarmonyInstance.PatchAll(typeof(EnemyAIPatch));
        HarmonyInstance.PatchAll(typeof(EnemySexPatch));
        HarmonyInstance.PatchAll(typeof(HealthSystemPatch));
        HarmonyInstance.PatchAll(typeof(PlayerCombatPatch));
        HarmonyInstance.PatchAll(typeof(PlayerHealthSystemPatch));
        HarmonyInstance.PatchAll(typeof(PlayerSexPatch));
        HarmonyInstance.PatchAll(typeof(SexSystemPatch));

        SceneManager.sceneLoaded += (UnityAction<Scene, LoadSceneMode>)OnSceneLoaded;
        Log.Msg($"Mod {ModInfo.MOD_GUID} is loaded!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Log.Msg($"Scene loaded: Name: {scene.name}, BuildIndex: {scene.buildIndex}");
        if (scene.buildIndex >= 4)
        {
            SexMoveChoiceMod.Prepare();
            EnemyTraitsMod.Reset();
        }
    }

    public override void OnPreferencesSaved()
    {
        PluginConfig?.Save();

        base.OnPreferencesSaved();
    }
}
