using BaseMod.Core;
using MelonLoader;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class GlossEffectMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<float> BaseGloss;
    internal static MelonPreferences_Entry<float> MaxGloss;
    internal static MelonPreferences_Entry<float> PlayerArousalWeightFactor;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(GlossEffectMod), nameof(Enabled), false,
                "Activates the modification", new AcceptableValueList<bool>([true, false]));
            BaseGloss = config.Entry(nameof(GlossEffectMod), nameof(BaseGloss), 0.1f,
                "Base game gloss value", new AcceptableValueRange<float>(0.0f, 1.0f));
            MaxGloss = config.Entry(nameof(GlossEffectMod), nameof(MaxGloss), 0.3f,
                "Maximum gloss value", new AcceptableValueRange<float>(0.0f, 1.0f));
            PlayerArousalWeightFactor = config.Entry(nameof(GlossEffectMod), nameof(PlayerArousalWeightFactor), 0.65f,
                "Player arousal weight factor (higher values increase Arousal's influence, while the remainder goes to Ecstasy automatically)", new AcceptableValueRange<float>(0.0f, 1.0f));
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static float GetPlayerGlossEffect(int currentArousal, int maxArousal, int currentEcstasy, int maxEcstasy)
    {
        float ecstasyWeight = 1.0f - PlayerArousalWeightFactor.Value;
        float level = PlayerArousalWeightFactor.Value * ((float)currentArousal / maxArousal)
                    + ecstasyWeight * ((float)currentEcstasy / maxEcstasy);

        float value = BaseGloss.Value + level * (MaxGloss.Value - BaseGloss.Value);

        return Math.Clamp(value, BaseGloss.Value, MaxGloss.Value);
    }

    internal static float GetEnemyGlossEffect(int currentEcstasy, int maxEcstasy)
    {
        float level = (float)currentEcstasy / maxEcstasy;
        float value = BaseGloss.Value + level * (MaxGloss.Value - BaseGloss.Value);

        return Math.Clamp(value, BaseGloss.Value, MaxGloss.Value);
    }

    internal static float GetGlossEffect(int currentTick)
    {
        float inc = currentTick * 0.01f;
        float value = BaseGloss.Value + inc;

        return Math.Clamp(value, BaseGloss.Value, MaxGloss.Value);
    }
}
