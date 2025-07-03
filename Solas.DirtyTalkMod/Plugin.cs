using System.IO;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;

using Solas.DirtyTalkMod.Mods;
using Solas.DirtyTalkMod.Patches;

namespace Solas.DirtyTalkMod;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin {
    internal static new ManualLogSource Log;
    internal static string PluginAssets;
    internal static string PluginConfigs;
    internal static string PluginResources;
    internal static Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load() {
        // Plugin startup logic
        Log = base.Log;
        string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        PluginAssets = Path.Combine(baseDirectory, "Assets");
        PluginConfigs = Path.Combine(baseDirectory, "Configs");
        PluginResources = Path.Combine(baseDirectory, "Resources");

        EnemyDirtyTalkMod.Load(Config);
        PlayerDirtyTalkMod.Load(Config);

        Harmony.PatchAll(typeof(EnemyAIPatch));
        Harmony.PatchAll(typeof(PlayerCombatPatch));
        Harmony.PatchAll(typeof(CaptureSystemPatch));
        Harmony.PatchAll(typeof(SexSystemPatch));

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
