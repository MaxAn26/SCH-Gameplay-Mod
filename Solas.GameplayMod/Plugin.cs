using System.IO;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

using BaseMod.Core.Extensions;

using HarmonyLib;

using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Solas.GameplayMod;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin {
    internal static new ManualLogSource Log;
    internal static string PluginResources;
    internal static Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load() {
        // Plugin startup logic
        Log = base.Log;
        PluginResources = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");

        SceneManager.sceneLoaded += (UnityAction<Scene, LoadSceneMode>)OnSceneLoaded;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Log.Info($"Scene loaded: Name: {scene.name}, BuildIndex: {scene.buildIndex}");
        if (scene.buildIndex >= 4) {
        }
    }
}
