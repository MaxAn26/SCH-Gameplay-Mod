using BaseMod.Core;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using Solas.GameplayMod.Enums;
using Solas.GameplayMod.Models;
using UnityEngine.SceneManagement;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class PlayerWeaknessMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> DenyEscape;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static PlayerWeakness Weakness { get; set; } = new();
    #endregion

    internal static CharacterData Character => CharacterData.Instance;

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(PlayerWeaknessMod), nameof(Enabled), false,
                "Activates the modification", new ModConfig.AcceptableValueList<bool>([true, false]));
            DenyEscape = config.Entry(nameof(PlayerWeaknessMod), nameof(DenyEscape), true,
                "Player can't escape from interaction and should end it by cumming or using counter", new AcceptableValueList<bool>([true, false]));

            if (Enabled.Value)
            {
                if (!JsonUtils.TryDeserialize(Core.PluginConfigs, "PlayerWeaknesses.json", out PlayerWeakness weaknessConfig))
                {
                    weaknessConfig = new PlayerWeakness();
                    JsonUtils.TrySerialize(Core.PluginConfigs, "PlayerWeaknesses.json", weaknessConfig);
                }
                Weakness = weaknessConfig;
            }
            else
            {
                DenyEscape.Value = false;
            }
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static void Apply(SexSystem sexSystem)
    {
        try
        {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            if (SexMoveChoiceMod.IsModActive && SexMoveChoiceMod.LastSexMove is null || sexSystem.IsThreesome)
            {
                return;
            }

            PlayerWeakness.Config playerConfig = new();
            if (Character.Ismale)
            {
                playerConfig = Weakness.Male;
            }
            else if (Character.adultSettingsDATA.FutaPlayerMode)
            {
                playerConfig = Weakness.Futa;
            }
            else
            {
                playerConfig = Weakness.Female;
            }

            if (playerConfig.Positive.HasFlag(Weaknesses.NoWeaknesses))
            {
                return;
            }

            Character.statusDATA.WeaknessList.Clear();
            List<int> sexTypes = [];

            int sexType = SexMoveChoiceMod.LastSexMove.Type;

            switch (sexType)
            {
                case 1:
                    if (CheckCombinations([SexSystem.SexIsLickingTarget, SexSystem.SexIsOralTarget], [SexSystem.SexIsLickingCaster, SexSystem.SexIsOralCaster]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.OralTaker, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }
                    else if (CheckCombinations([SexSystem.SexIsOralCaster], [SexSystem.SexIsOralTarget]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.OralSuck, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }
                    else if (CheckCombinations([SexSystem.SexIsLickingCaster], [SexSystem.SexIsLickingTarget]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.OralLick, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }

                    break;
                case 2:
                    playerConfig.ActionByFlag(Weaknesses.HandJobs, () => SetWeakness(sexType), () => RemoveWeakness(sexType));

                    break;
                case 3:
                    playerConfig.ActionByFlag(Weaknesses.BreastPlay, () => SetWeakness(sexType), () => RemoveWeakness(sexType));

                    break;
                case 4:
                    playerConfig.ActionByFlag(Weaknesses.FootPlay, () => SetWeakness(sexType), () => RemoveWeakness(sexType));

                    break;
                case 6:
                    if (CheckCombinations(
                           [!sexSystem.CasterActive, !sexSystem.TargetActive, !sexSystem.CasterMale, !sexSystem.TargetMale],
                           [!sexSystem.CasterActive, !sexSystem.TargetActive, !sexSystem.CasterMale, !sexSystem.TargetMale]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.MissionaryLesbian, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }
                    else if (CheckCombinations([sexSystem.CasterActive], [sexSystem.TargetActive, !sexSystem.CasterActive]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.MissionaryPenetrate, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }
                    else if (CheckCombinations([sexSystem.TargetActive, !sexSystem.CasterActive], [sexSystem.CasterActive]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.MissionaryPenetrated, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }

                    break;
                case 7:
                    if (CheckCombinations(
                           [!sexSystem.CasterActive, !sexSystem.TargetActive, !sexSystem.CasterMale, !sexSystem.TargetMale],
                           [!sexSystem.CasterActive, !sexSystem.TargetActive, !sexSystem.CasterMale, !sexSystem.TargetMale]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.DoggyLesbian, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }
                    else if (CheckCombinations([sexSystem.CasterActive], [sexSystem.TargetActive, !sexSystem.CasterActive]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.DoggyPenetrate, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }
                    else if (CheckCombinations([sexSystem.TargetActive, !sexSystem.CasterActive], [sexSystem.CasterActive]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.DoggyPenetrated, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }

                    break;
                case 8:
                    if (CheckCombinations(
                           [!sexSystem.CasterActive, !sexSystem.TargetActive, !sexSystem.CasterMale, !sexSystem.TargetMale],
                           [!sexSystem.CasterActive, !sexSystem.TargetActive, !sexSystem.CasterMale, !sexSystem.TargetMale]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.MountingLesbian, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }
                    else if (CheckCombinations([sexSystem.CasterActive], [sexSystem.TargetActive, !sexSystem.CasterActive]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.MountingPenetrate, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }
                    else if (CheckCombinations([sexSystem.TargetActive, !sexSystem.CasterActive], [sexSystem.CasterActive]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.MountingPenetrated, () => SetWeakness(sexType), () => RemoveWeakness(sexType));
                    }

                    break;
                case 9:
                    playerConfig.ActionByFlag(Weaknesses.Threesome, () => SetWeaknesses([sexType, 9]), () => RemoveWeaknesses([sexType, 9]));

                    if (CheckCombinations([], [SexSystem.SexIsLickingCaster, SexSystem.SexIsOralCaster, SexSystem.SexIsLickingAssist, SexSystem.SexIsOralAssist]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.OralTaker, () => SetWeaknesses([sexType, 9]), () => RemoveWeaknesses([sexType, 9]));
                    }
                    else if (CheckCombinations([], [SexSystem.SexIsOralTarget]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.OralSuck, () => SetWeaknesses([sexType, 9]), () => RemoveWeaknesses([sexType, 9]));
                    }
                    else if (CheckCombinations([], [SexSystem.SexIsLickingTarget]))
                    {
                        playerConfig.ActionByFlag(Weaknesses.OralLick, () => SetWeaknesses([sexType, 9]), () => RemoveWeaknesses([sexType, 9]));
                    }
                    else if (sexSystem.IsSex)
                    {
                        if (CheckCombinations([], [!sexSystem.CasterActive, !sexSystem.TargetActive, !sexSystem.CasterMale, !sexSystem.TargetMale, !sexSystem.AssistActive, !sexSystem.AssistMale]))
                        {
                            playerConfig.ActionByFlag(Weaknesses.MissionaryLesbian, () => SetWeaknesses([sexType, 9]), () => RemoveWeaknesses([sexType, 9]));
                        }
                        else if (CheckCombinations([], [sexSystem.TargetActive, !sexSystem.CasterActive]))
                        {
                            playerConfig.ActionByFlag(Weaknesses.MissionaryPenetrate, () => SetWeaknesses([sexType, 9]), () => RemoveWeaknesses([sexType, 9]));
                        }
                        else if (CheckCombinations([], [sexSystem.CasterActive]))
                        {
                            playerConfig.ActionByFlag(Weaknesses.MissionaryPenetrated, () => SetWeaknesses([sexType, 9]), () => RemoveWeaknesses([sexType, 9]));
                        }
                    }

                    break;
                default:
                    break;
            }

            if (Character.statusDATA.IsBound || Character.statusDATA.IsBoundHeavyRestraint > 0)
            {
                if (playerConfig.ActionByFlag(Weaknesses.Bondage, () => SetWeaknesses([sexType, 9]), Character.statusDATA.WeaknessList.Clear))
                {
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }

        static bool CheckCombinations(List<bool> forCaster, List<bool> forTarget) => SexSystem.PlayerAttacker ? forCaster.Any(b => b) : forTarget.Any(b => b);

        static void SetWeakness(int sexType)
        {
            if (!Character.statusDATA.WeaknessList.Contains(sexType))
            {
                Core.LogDebug($"Player will have weakness to sex type: {sexType}");
                Character.statusDATA.WeaknessList.Add(sexType);
            }
        }

        static void RemoveWeakness(int sexType)
        {
            if (Character.statusDATA.WeaknessList.Contains(sexType))
            {
                Character.statusDATA.WeaknessList.Remove(sexType);
            }
        }

        static void SetWeaknesses(List<int> sexTypes) => sexTypes.ForEach(t => SetWeakness(t));
        static void RemoveWeaknesses(List<int> sexTypes) => sexTypes.ForEach(t => RemoveWeakness(t));
    }
    internal static bool BlockEscape(SexSystem sexSystem) => SceneManager.GetActiveScene().buildIndex > 4 && DenyEscape.Value && sexSystem.playerweak;
}
