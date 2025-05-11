using System;

using BaseMod.Core.Extensions;

using BepInEx.Configuration;

namespace Solas.GameplayMod.Mods;
internal class GlossEffectMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<float> BaseGloss;
    internal static ConfigEntry<float> MaxGloss;
    internal static ConfigEntry<float> PlayerArousalWeightFactor;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static void Load(ConfigFile config) {
        try {
            Enabled = config.Bind(nameof(GlossEffectMod), nameof(Enabled), false,
                new ConfigDescription("Activates the modification", new AcceptableValueList<bool>([true, false])));
            BaseGloss = config.Bind(nameof(GlossEffectMod), nameof(BaseGloss), 0.1f,
                new ConfigDescription("Base game gloss value", new AcceptableValueRange<float>(0.0f, 1.0f)));
            MaxGloss = config.Bind(nameof(GlossEffectMod), nameof(MaxGloss), 0.3f,
                new ConfigDescription("Maximum gloss value", new AcceptableValueRange<float>(0.0f, 1.0f)));
            PlayerArousalWeightFactor = config.Bind(nameof(GlossEffectMod), nameof(PlayerArousalWeightFactor), 0.65f,
                new ConfigDescription("Player arousal weight factor (higher values increase Arousal's influence, while the remainder goes to Ecstasy automatically)", new AcceptableValueRange<float>(0.0f, 1.0f)));
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static float GetPlayerGlossEffect(int currentArousal, int maxArousal, int currentEcstasy, int maxEcstasy) {
        float ecstasyWeight = 1.0f - PlayerArousalWeightFactor.Value;
        float level = PlayerArousalWeightFactor.Value * ((float)currentArousal / maxArousal)
                    + ecstasyWeight * ((float)currentEcstasy / maxEcstasy);

        float value = BaseGloss.Value + level * (MaxGloss.Value - BaseGloss.Value);

        return Math.Clamp(value, BaseGloss.Value, MaxGloss.Value);
    }

    internal static float GetEnemyGlossEffect(int currentEcstasy, int maxEcstasy) {
        float level = (float)currentEcstasy / maxEcstasy;
        float value = BaseGloss.Value + level * (MaxGloss.Value - BaseGloss.Value);

        return Math.Clamp(value, BaseGloss.Value, MaxGloss.Value);
    }
}