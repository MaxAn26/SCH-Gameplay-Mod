using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using Solas.GameplayMod.Components;
using Solas.GameplayMod.Enums;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class OrgasmControlMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> SelfControl;
    internal static MelonPreferences_Entry<int> SelfControlChance;
    internal static MelonPreferences_Entry<bool> DeniedOrgasm;
    internal static MelonPreferences_Entry<int> DeniedOrgasmChance;
    internal static MelonPreferences_Entry<bool> PunishmentOrgasm;
    internal static MelonPreferences_Entry<int> PunishmentOrgasmChance;
    internal static MelonPreferences_Entry<int> PunishmentOrgasmLimit;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static OrgasmControl OrgasmControl = OrgasmControl.None;
    internal static int PunishmentOrgasmCount;
    internal static Sprite PunishmentOrgasmSprite;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(OrgasmControlMod), nameof(Enabled), false,
                "Activates the modification", new AcceptableValueList<bool>([true, false]));
            SelfControl = config.Entry(nameof(OrgasmControlMod), nameof(SelfControl), true,
                "Target character may control his own orgasm and will cum with caster", new AcceptableValueList<bool>([true, false]));
            SelfControlChance = config.Entry(nameof(OrgasmControlMod), nameof(SelfControlChance), 10,
                "Chance for target character to control his own orgasm", new AcceptableValueRange<int>(0, 100));
            DeniedOrgasm = config.Entry(nameof(OrgasmControlMod), nameof(DeniedOrgasm), true,
                "Caster may deny target to cum when target reached maximum ecstasy if first time during current sex position", new AcceptableValueList<bool>([true, false]));
            DeniedOrgasmChance = config.Entry(nameof(OrgasmControlMod), nameof(DeniedOrgasmChance), 10,
                "Chance for caster deny target to cum", new AcceptableValueRange<int>(0, 100));
            PunishmentOrgasm = config.Entry(nameof(OrgasmControlMod), nameof(PunishmentOrgasm), true,
                "Enemy may deny player to cum in next several times", new AcceptableValueList<bool>([true, false]));
            PunishmentOrgasmChance = config.Entry(nameof(OrgasmControlMod), nameof(PunishmentOrgasmChance), 10,
                "Chance for enemy deny player to cum", new AcceptableValueRange<int>(0, 100));
            PunishmentOrgasmLimit = config.Entry(nameof(OrgasmControlMod), nameof(PunishmentOrgasmLimit), 10,
                "Count of player reach max ecstasy before he will be punished", new AcceptableValueRange<int>(0, 100));

            if (!Enabled.Value)
            {
                SelfControl.Value = false;
                PunishmentOrgasm.Value = false;
            }
            else
            {
                PunishmentOrgasmSprite = ResourcesUtils.LoadSprite(Core.PluginAssets, "belt.png");
            }
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static void ApplySexControl(SexSystem sexSystem)
    {
        try
        {
            if (!Enabled.Value || !sexSystem.IsSex || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            if (OrgasmControl is not OrgasmControl.None)
            {
                return;
            }

            EnemyCharacterComponent enemyComponent = sexSystem.Enemy.GetComponentWithCast<EnemyCharacterComponent>();
            string enemyName = enemyComponent?.EnemyAI.enemyName ?? "Enemy";
            TraitInvulnerable enemyPleasure = enemyComponent?.EnemyTrait?.Invulnerable ?? TraitInvulnerable.None;
            if (SelfControl.Value && enemyPleasure is not TraitInvulnerable.Pleasure && RandomUtils.Chance(SelfControlChance.Value))
            {
                OrgasmControl = OrgasmControl.SelfControl;
                string text = SexSystem.PlayerAttacker
                    ? $"{enemyName} controls their arousal in order to climax together with you"
                    : "You control your arousal in order to climax together with your partner";

                Core.LogDebug(text);
                sexSystem.console.ConsoleWrite(text);
            }
            else if (DeniedOrgasm.Value && RandomUtils.Chance(DeniedOrgasmChance.Value))
            {
                OrgasmControl = OrgasmControl.DeniedOrgasm;
                string text = SexSystem.PlayerAttacker
                    ? $"{enemyName} controls their arousal by delaying their orgasm"
                    : "You control your arousal by delaying your orgasm";

                Core.LogDebug(text);
                sexSystem.console.ConsoleWrite(text);
            }
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
            return;
        }
    }

    internal static void CheckOrgasmControl(int currentEcstasy, int maxEcstasy, ref int ecstasy)
    {
        try
        {
            if (!Enabled.Value || SexSystem.Sexstatus is not SEXSTATUS.Fucking || ecstasy <= 1)
            {
                return;
            }

            if (currentEcstasy > maxEcstasy * 0.85)
            {
                switch (OrgasmControl)
                {
                    case OrgasmControl.SelfControl:
                        ecstasy = Mathf.RoundToInt(maxEcstasy * 0.9f);
                        break;
                    case OrgasmControl.DeniedOrgasm:
                        ecstasy = 0;
                        ResetDeniedOrgasm();
                        break;
                    default:
                        break;
                }
            }

        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static void ResetSelfControl()
    {
        if (OrgasmControl is OrgasmControl.SelfControl)
        {
            OrgasmControl = OrgasmControl.None;
        }
    }

    internal static void ResetDeniedOrgasm()
    {
        if (OrgasmControl is OrgasmControl.DeniedOrgasm)
        {
            OrgasmControl = OrgasmControl.None;
        }
    }

    internal static bool ApplyPunishmentOrgasm(SexSystem sexSystem)
    {
        try
        {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return false;
            }

            if (OrgasmControl is not OrgasmControl.None)
            {
                return false;
            }

            if (PunishmentOrgasmSprite is not null && PunishmentOrgasm.Value && RandomUtils.Chance(PunishmentOrgasmChance.Value))
            {
                OrgasmControl = OrgasmControl.PunishmentOrgasm;
                string enemyName = sexSystem.Enemy.TryGetComponentWithCast(out EnemyAI enemyAI) ? enemyAI.enemyName : "Enemy";
                AddIcon(sexSystem, PunishmentOrgasmSprite);
                Core.LogDebug($"{enemyName} put on Player Spanking Orgasm buff");
                sexSystem.console.ConsoleWrite($"{enemyName} put on you Spanking Orgasm buff");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
            return false;
        }
    }

    internal static bool UpdateAndCheckPunishmentOrgasm(bool reset = false)
    {
        if (OrgasmControl is OrgasmControl.PunishmentOrgasm)
        {
            PunishmentOrgasmCount++;
            if (PunishmentOrgasmCount >= PunishmentOrgasmLimit.Value)
            {
                if (reset)
                {
                    ResetPunishmentOrgasm();
                }

                return true;
            }
        }
        else
        {
            PunishmentOrgasmCount = 0;
        }

        return false;
    }

    internal static void BreakPunishmentOrgasm(SexSystem sexSystem)
    {
        if (OrgasmControl is OrgasmControl.PunishmentOrgasm)
        {
            ResetPunishmentOrgasm();
            RemoveIcon(sexSystem, PunishmentOrgasmSprite);
            sexSystem.console.ConsoleWrite("You managed to break Spanking Orgasm!");
        }
    }

    internal static void ResetPunishmentOrgasm()
    {
        if (OrgasmControl is OrgasmControl.PunishmentOrgasm)
        {
            OrgasmControl = OrgasmControl.None;
        }

        PunishmentOrgasmCount = 0;
    }

    internal static void AddIcon(SexSystem sexSystem, Sprite sprite)
    {
        if (sprite is null)
        {
            return;
        }

        Transform prefabTransform = sexSystem.buffsystem.transform;
        GameObject gameObject = UnityEngine.Object.Instantiate(sexSystem.buffsystem.TextPrefab, prefabTransform);
        gameObject.GetComponentWithCast<Image>().sprite = sprite;
        sexSystem.buffsystem.Buffs.Add(gameObject);
    }

    internal static void RemoveIcon(SexSystem sexSystem, Sprite sprite)
    {
        if (sprite is null)
        {
            return;
        }

        for (int i = 0; i < sexSystem.buffsystem.Buffs.Count; i++)
        {
            if (sexSystem.buffsystem.Buffs[i].GetComponentWithCast<Image>().sprite == sprite)
            {
                UnityEngine.Object.Destroy(sexSystem.buffsystem.Buffs[i].gameObject);
                sexSystem.buffsystem.Buffs.RemoveAt(i);
            }
        }
    }
}

internal enum OrgasmControl
{
    None,
    SelfControl,
    DeniedOrgasm,
    PunishmentOrgasm
}
