using System.Reflection;
using BaseMod.Core;
using MelonLoader;
using MelonLoader.Utils;
using Solas.DirtyTalkMod;
using Solas.DirtyTalkMod.Mods;
using Solas.DirtyTalkMod.Patches;

[assembly: MelonInfo( typeof( DirtyTalkMod ), ModInfo.MOD_NAME, ModInfo.MOD_VERSION, ModInfo.MOD_DEVELOPER, ModInfo.MOD_URL )]
[assembly: MelonGame( ModInfo.GAME_DEVELOPER, ModInfo.GAME_NAME )]

namespace Solas.DirtyTalkMod;
public class DirtyTalkMod : MelonMod 
{
    internal static MelonLogger.Instance Log;
    internal static ModConfig PluginConfig;
    internal static string ConfigPath;
    internal static string PluginConfigs;

    public override void OnInitializeMelon() {
        base.OnInitializeMelon();

        // DirtyTalkMod startup logic
        Log = LoggerInstance;
        ConfigPath = MelonEnvironment.UserDataDirectory;
        PluginConfigs = Path.Combine( ConfigPath, ModInfo.MOD_GUID, "Configs" );

        PluginConfig = new( $"{ModInfo.MOD_GUID}.cfg" );

        EnemyDirtyTalkMod.Load( PluginConfig );
        PlayerDirtyTalkMod.Load( PluginConfig );

        HarmonyInstance.PatchAll(typeof(EnemyAIPatch));
        HarmonyInstance.PatchAll(typeof(PlayerCombatPatch));
        HarmonyInstance.PatchAll(typeof(CaptureSystemPatch));
        HarmonyInstance.PatchAll(typeof(SexSystemPatch));

        Log.Msg($"Mod {ModInfo.MOD_GUID} is loaded!");
    }

    public override void OnPreferencesSaved() {
        PluginConfig?.Save();

        base.OnPreferencesSaved();
    }
}
