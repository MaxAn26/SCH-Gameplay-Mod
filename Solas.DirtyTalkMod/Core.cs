using System.Runtime.CompilerServices;
using BaseMod.Core;
using BaseMod.Core.Extensions;
using Il2Cpp;
using MelonLoader;
using MelonLoader.Logging;
using MelonLoader.Utils;
using Solas.DirtyTalkMod;
using Solas.DirtyTalkMod.Components;
using Solas.DirtyTalkMod.Mods;

[assembly: HarmonyDontPatchAll]
[assembly: MelonInfo(typeof(Core), ModInfo.MOD_NAME, ModInfo.MOD_VERSION, ModInfo.MOD_DEVELOPER, ModInfo.MOD_URL)]
[assembly: MelonGame(ModInfo.GAME_DEVELOPER, ModInfo.GAME_NAME)]

namespace Solas.DirtyTalkMod;
public class Core : MelonMod 
{
    internal static ModConfig PluginConfig;
    internal static string ConfigPath;
    internal static string PluginConfigs;

    public override void OnInitializeMelon() 
    {
        base.OnInitializeMelon();

        ConfigPath = MelonEnvironment.UserDataDirectory;
        PluginConfigs = Path.Combine( ConfigPath, ModInfo.MOD_GUID, "Configs" );

        PluginConfig = new( $"{ModInfo.MOD_GUID}.cfg" );

        EnemyDirtyTalkMod.Load( PluginConfig );
        PlayerDirtyTalkMod.Load( PluginConfig );

        HarmonyInstance.PatchAll();

        LogInfo($"Mod {ModInfo.MOD_GUID} is loaded!");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        LogInfo($"Scene loaded: Name: {sceneName}, BuildIndex: {buildIndex}");
        base.OnSceneWasLoaded(buildIndex, sceneName);

        if (buildIndex < 4 || CharacterData.Instance is null)
        {
            return;
        }

        if (EnemyDirtyTalkMod.IsModActive && !CharacterData.Instance.adultSettingsDATA.DtalkEnemy 
            || PlayerDirtyTalkMod.IsModActive && CharacterData.Instance.adultSettingsDATA.DtalkInner)
        { 
            Zessentials.instance.AddModComponent<MainDirtyTalkComponent>();
        }
    }
    public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
    {
        base.OnSceneWasUnloaded(buildIndex, sceneName);

        if (buildIndex >= 4)
        { 
            Zessentials.instance.RemoveModComponent<MainDirtyTalkComponent>();
        }
    }

    public override void OnPreferencesSaved() {
        PluginConfig?.Save();

        base.OnPreferencesSaved();
    }

    public static void LogTrace(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.DarkGray, Combine(message, methodName));
    public static void LogDebug(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.Cyan, Combine(message, methodName));
    public static void LogInfo(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.White, Combine(message, methodName));
    public static void LogSuccess(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.Green, Combine(message, methodName));
    public static void LogWarning(string message, [CallerMemberName] string methodName = null) => MelonLogger.Warning(Combine(message, methodName));
    public static void LogError(Exception excepion, [CallerMemberName] string methodName = null) => LogError(excepion.Message, excepion, methodName);
    public static void LogError(string message, Exception excepion, [CallerMemberName] string methodName = null) => MelonLogger.Error(Combine(message, methodName), excepion);

    private static string Combine(string message, string methodName) => !string.IsNullOrWhiteSpace(methodName) ? $"{methodName}:> {message}" : message;
}
