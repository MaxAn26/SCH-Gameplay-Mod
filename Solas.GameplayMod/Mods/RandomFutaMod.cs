using System;
using System.Collections;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP.Utils.Collections;

using UnityEngine;

namespace Solas.GameplayMod.Mods;
internal class RandomFutaMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<int> ChanceForFuta;
    internal static ConfigEntry<int> ChanceForFullFuta;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static void Load( ConfigFile config ) {
        try {
            Enabled = config.Bind( nameof( RandomFutaMod ), nameof( Enabled ), false,
                new ConfigDescription( "Activates the modification", new AcceptableValueList<bool>( [true, false] ) ) );
            ChanceForFuta = config.Bind(nameof(RandomFutaMod), nameof(ChanceForFuta), 35,
                new ConfigDescription("Chance for female character with active or mixed role become futanari", new AcceptableValueRange<int>(0, 100)));
            ChanceForFullFuta = config.Bind(nameof(RandomFutaMod), nameof(ChanceForFullFuta), 50,
                new ConfigDescription("Chance for female futa character get full futa (dick + balls)", new AcceptableValueRange<int>(0, 100)));

        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
        }
    }

    internal static void Apply( EnemySex enemySex ) {
        try {
            if(!Enabled.Value)
                return;

            if(enemySex.EnemyMale)
                return;

            _ = enemySex.StartCoroutine( UpdateDick( enemySex ).WrapToIl2Cpp() );
        } catch(Exception ex) {
            Plugin.Log.Error( ex.Message );
            return;
        }
    }

    internal static IEnumerator UpdateDick( EnemySex enemySex ) {
        yield return new WaitForSeconds( 2f );

        if (RandomUtils.Chance(ChanceForFuta.Value)) {
            Plugin.Log.Info($"{enemySex.gameObject.GetComponentWithCast<EnemyAI>()?.enemyName} will use a dick");

            enemySex.Dick.sharedMesh = RandomUtils.Chance(ChanceForFullFuta.Value) ? enemySex.DickMesh : enemySex.DickHalfMesh;
            Material material = UnityEngine.Object.Instantiate(enemySex.DickMatF);
            enemySex.Dick.material = material;
            var color = enemySex.Character.material.GetColor("_Albedo_Tint");
            material.SetColor("_Albedo_Tint", color);
        } else {
            Plugin.Log.Info($"{enemySex.gameObject.GetComponentWithCast<EnemyAI>()?.enemyName} will use strapon");

            enemySex.Dick.sharedMesh = enemySex.StrapMesh;
            Material material = UnityEngine.Object.Instantiate(enemySex.StrapMat);
            enemySex.Dick.material = material;
            Color color = new() {
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