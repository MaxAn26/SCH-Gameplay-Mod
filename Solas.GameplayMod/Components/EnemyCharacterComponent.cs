using System;
using System.Collections;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Unity.IL2CPP.Utils.Collections;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using Solas.GameplayMod.Models;
using Solas.GameplayMod.Mods;

using UnityEngine;

namespace Solas.GameplayMod.Components;
internal class EnemyCharacterComponent : MonoBehaviour {
    internal MaterialPropertyBlock MaterialPropertyBlock = new();
    internal SexSystem SexSystem;
    internal EnemyAI EnemyAI;
    internal EnemyTraitModel EnemyTrait;
    internal EnemySexModel EnemySexTypes;

    internal Fetish EnemyFetish = Fetish.NoFetish;
    internal Weaknesses AsCasterWeaknesses = Weaknesses.NoWeaknesses;
    internal Weaknesses AsTargetWeaknesses = Weaknesses.NoWeaknesses;
    internal int CorruptionChance;

    internal bool? IsActive;
    private int _glossTick = 0;

    #region Il2Cpp .ctor
    static EnemyCharacterComponent() {
        ClassInjector.RegisterTypeInIl2Cpp<EnemyCharacterComponent>();
    }

    public EnemyCharacterComponent() : base(ClassInjector.DerivedConstructorPointer<EnemyCharacterComponent>()) {
        ClassInjector.DerivedConstructorBody(this);
    }

    public EnemyCharacterComponent(IntPtr pointer) : base(pointer) {

    }
    #endregion

    public void Initialize() {
        try {
            if (gameObject.TryGetComponentWithCast(out EnemyAI enemyAI)) {
                EnemyAI = enemyAI;
                SexSystem = enemyAI.Sexscript;
            } else {
                Destroy(this);
            }

            if (RandomEnemyRoleMod.IsModActive) {
                IsActive = RandomEnemyRoleMod.GetRandomRole(enemyAI.EnSex.EnemyMale);
            }

            if (EnemyTraitsMod.IsModActive) {
                var trait = EnemyTraitsMod.GetEnemyTrait(EnemyAI.typeOfEnemy);
                if (trait is not null) {
                    EnemyTraitsMod.ApplyTrait(enemyAI, trait);
                    EnemyTrait = trait;
                    Plugin.Log.Info($"Enemy '{EnemyAI.enemyName}' get {trait.Name} ({trait.TraitType}) trait");
                }
            }

            if (EnemySexExtendMod.IsModActive) {
                var sexTypes = EnemySexExtendMod.GetEnemySexModel(EnemyAI.typeOfEnemy);
                if (sexTypes is not null) {
                    EnemySexTypes = sexTypes;
                    EnemyFetish = RandomUtils.Flag(EnemySexTypes.AllowedFetishes);
                    AsCasterWeaknesses = RandomUtils.Flag(EnemySexTypes.CasterWeaknesses);
                    AsTargetWeaknesses = RandomUtils.Flag(EnemySexTypes.TargetWeaknesses);
                    CorruptionChance = EnemySexExtendMod.CalculateCorruption(EnemySexTypes.CorruptionChance, EnemyFetish);
                }
            }
        } catch (Exception e) {
            Plugin.Log.Error(e);
            Destroy(this);
        }
    }
    public void Start() {
        StartCoroutine(UpdateSmoothness().WrapToIl2Cpp());
    }

    internal IEnumerator UpdateSmoothness() {
        yield return new WaitForSeconds(5f);

        if (!UpdateSmoothnessDeviate() && !EnemyAI.isDead && !SexSystem.GameOver) {
            _glossTick++;
            StartCoroutine(UpdateSmoothness().WrapToIl2Cpp());
        }
    }

    private bool UpdateSmoothnessDeviate() {
        if (!GlossEffectMod.IsModActive)
            return true;

        float gloss = GlossEffectMod.GetGlossEffect(_glossTick);

        UpdateMaterialPropertyBlock(EnemyAI.EnSex.Character, 0, "_SmoothnessDeviate", gloss);
        UpdateMaterialPropertyBlock(EnemyAI.EnSex.Character, 1, "_SmoothnessDeviate", gloss);

        if (EnemyAI.EnSex.EnemyFuta || EnemyAI.EnSex.EnemyMale)
            UpdateMaterialPropertyBlock(EnemyAI.EnSex.Dick, 0, "_SmoothnessDeviate", gloss);

        return gloss >= GlossEffectMod.MaxGloss.Value;
    }

    private void UpdateMaterialPropertyBlock(Renderer renderer, int index, string property, float value) {
        renderer.GetPropertyBlock(MaterialPropertyBlock, index);
        MaterialPropertyBlock.SetFloat(property, value);
        renderer.SetPropertyBlock(MaterialPropertyBlock, index);
    }

    [HideFromIl2Cpp]
    public static void RegisterClass(EnemyAI enemyAI) {
        enemyAI.gameObject.AddComponentWithAction<EnemyCharacterComponent>(component => component.Initialize());
    }
}