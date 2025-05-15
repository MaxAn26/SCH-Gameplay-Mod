using System.IO;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

using BaseMod.Core.Extensions;

using HarmonyLib;

using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Solas.GameplayMod.Mods;
using Solas.GameplayMod.Patches;

namespace Solas.GameplayMod;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin {
    internal static new ManualLogSource Log;
    internal static string PluginConfigs;
    internal static string PluginResources;
    internal static Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load() {
        // Plugin startup logic
        Log = base.Log;
        string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        PluginConfigs = Path.Combine(baseDirectory, "Configs");
        PluginResources = Path.Combine(baseDirectory, "Resources");

        CapturedSlaveMod.Load(Config);
        CriticalHitMod.Load(Config);
        EnemyHPResetMod.Load(Config);
        ExtraBondageTrapMod.Load(Config);
        FuckMeMod.Load(Config);
        GlossEffectMod.Load(Config);
        LustCageMod.Load(Config);
        ObeyToEnemyMod.Load(Config);
        RandomEnemyRoleMod.Load(Config);
        RandomFutaMod.Load(Config);
        RandomReverseMod.Load(Config);
        SexDamageMod.Load(Config);
        SexInitiatorStateMod.Load(Config);
        SexMoveChoiceMod.Load(Config);

        Harmony.PatchAll(typeof(CaptureSystemPatch));
        Harmony.PatchAll(typeof(EnemyActionsPatch));
        Harmony.PatchAll(typeof(EnemyAIPatch));
        Harmony.PatchAll(typeof(EnemySexPatch));
        Harmony.PatchAll(typeof(HealthSystemPatch));
        Harmony.PatchAll(typeof(PlayerCombatPatch));
        Harmony.PatchAll(typeof(PlayerHealthSystemPatch));
        Harmony.PatchAll(typeof(PlayerSexPatch));
        Harmony.PatchAll(typeof(SexSystemPatch));

        SceneManager.sceneLoaded += (UnityAction<Scene, LoadSceneMode>)OnSceneLoaded;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Log.Info($"Scene loaded: Name: {scene.name}, BuildIndex: {scene.buildIndex}");
        if (scene.buildIndex >= 4) {
            SexMoveChoiceMod.Prepare();
        }
    }
}
