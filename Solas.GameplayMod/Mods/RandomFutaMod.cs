using System.Collections;
using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class RandomFutaMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<int> ChanceForFuta;
    internal static MelonPreferences_Entry<int> ChanceForFullFuta;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(RandomFutaMod), nameof(Enabled), false,
                "Activates the modification", new AcceptableValueList<bool>([true, false]));
            ChanceForFuta = config.Entry(nameof(RandomFutaMod), nameof(ChanceForFuta), 35,
                "Chance for female character with active or mixed role become futanari", new AcceptableValueRange<int>(0, 100));
            ChanceForFullFuta = config.Entry(nameof(RandomFutaMod), nameof(ChanceForFullFuta), 50,
                "Chance for female futa character get full futa (dick + balls)", new AcceptableValueRange<int>(0, 100));

        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static void Apply(EnemySex enemySex)
    {
        try
        {
            if (!Enabled.Value)
            {
                return;
            }

            if (enemySex.EnemyMale)
            {
                return;
            }

            MelonCoroutines.Start(UpdateDick(enemySex));
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
            return;
        }
    }

    internal static IEnumerator UpdateDick(EnemySex enemySex)
    {
        yield return new WaitForSeconds(2f);

        if (RandomUtils.Chance(ChanceForFuta.Value))
        {
            Core.LogInfo($"{enemySex.gameObject.GetComponentWithCast<EnemyAI>()?.enemyName} will use a dick");

            enemySex.Dick.sharedMesh = RandomUtils.Chance(ChanceForFullFuta.Value) ? enemySex.DickMesh : enemySex.DickHalfMesh;
            Material material = UnityEngine.Object.Instantiate(enemySex.DickMatF);
            enemySex.Dick.material = material;
            Color color = enemySex.Character.material.GetColor("_Albedo_Tint");
            material.SetColor("_Albedo_Tint", color);
        }
        else
        {
            Core.LogInfo($"{enemySex.gameObject.GetComponentWithCast<EnemyAI>()?.enemyName} will use strapon");

            enemySex.Dick.sharedMesh = enemySex.StrapMesh;
            Material material = UnityEngine.Object.Instantiate(enemySex.StrapMat);
            enemySex.Dick.material = material;
            Color color = new()
            {
                r = RandomUtils.Float(0.0f, 1.0f),
                g = RandomUtils.Float(0.0f, 1.0f),
                b = RandomUtils.Float(0.0f, 1.0f),
                a = 1.0f
            };
            material.SetColor("_Albedo_Tint", color);
        }

        Vector3 dickscale = enemySex.Dick.transform.localScale;
        float size = RandomUtils.Float(0.9f, 1.2f);
        dickscale.z = size;
        dickscale.x = size;
        dickscale.y = size;
        enemySex.Dick.transform.localScale = dickscale;
    }
}
