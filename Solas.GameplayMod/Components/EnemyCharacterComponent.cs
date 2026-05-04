using System.Collections;

using BaseMod.Core.Extensions;
using BaseMod.Core.Interfaces;
using BaseMod.Core.Utils;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using Solas.GameplayMod.Models;
using Solas.GameplayMod.Mods;

using UnityEngine;

namespace Solas.GameplayMod.Components;
internal class EnemyCharacterComponent : MonoBehaviour, IInitializeComponent
{
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
    private object _smoothnessCoroutine;

    #region Il2Cpp .ctor
    static EnemyCharacterComponent() => ClassInjector.RegisterTypeInIl2Cpp<EnemyCharacterComponent>();

    public EnemyCharacterComponent() : base(ClassInjector.DerivedConstructorPointer<EnemyCharacterComponent>()) => ClassInjector.DerivedConstructorBody(this);

    public EnemyCharacterComponent(IntPtr pointer) : base(pointer)
    {

    }
    #endregion

    public void Initialize()
    {
        try
        {
            if (gameObject.TryGetComponentWithCast(out EnemyAI enemyAI))
            {
                EnemyAI = enemyAI;
                SexSystem = enemyAI.Sexscript;
            }
            else
            {
                Destroy(this);
            }

            if (RandomEnemyRoleMod.IsModActive)
            {
                IsActive = RandomEnemyRoleMod.GetRandomRole(enemyAI.EnSex.EnemyMale);
            }

            if (EnemyTraitsMod.IsModActive)
            {
                EnemyTraitModel trait = EnemyTraitsMod.GetEnemyTrait(EnemyAI.typeOfEnemy);
                if (trait is not null)
                {
                    EnemyTraitsMod.ApplyTrait(enemyAI, trait);
                    EnemyTrait = trait;
                    GameplayMod.Log.Msg($"Enemy '{EnemyAI.enemyName}' get {trait.Name} ({trait.TraitType}) trait");
                }
            }

            if (EnemySexExtendMod.IsModActive)
            {
                EnemySexModel sexTypes = EnemySexExtendMod.GetEnemySexModel(EnemyAI.typeOfEnemy);
                if (sexTypes is not null)
                {
                    EnemySexTypes = sexTypes;
                    EnemyFetish = RandomUtils.Flag(EnemySexTypes.AllowedFetishes);
                    AsCasterWeaknesses = RandomUtils.Flag(EnemySexTypes.CasterWeaknesses);
                    AsTargetWeaknesses = RandomUtils.Flag(EnemySexTypes.TargetWeaknesses);
                    CorruptionChance = EnemySexExtendMod.CalculateCorruption(EnemySexTypes.CorruptionChance, EnemyFetish);
                }
            }
        }
        catch (Exception e)
        {
            GameplayMod.Log.Error(e);
            Destroy(this);
        }
    }
    public void Start()
    {
        StopCoroutine();
        _smoothnessCoroutine = MelonCoroutines.Start(UpdateSmoothness());
    }
    public void OnDestroy() => StopCoroutine();

    internal IEnumerator UpdateSmoothness()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            if (!GlossEffectMod.IsModActive || SexSystem.GameOver || EnemyAI.isDead)
            {
                yield break;
            }

            if (UpdateSmoothnessDeviate())
            {
                yield break;
            }
        }
    }

    private bool UpdateSmoothnessDeviate()
    {
        if (!GlossEffectMod.IsModActive)
        {
            return true;
        }

        _glossTick++;
        float gloss = GlossEffectMod.GetGlossEffect(_glossTick);

        UpdateMaterialPropertyBlock(EnemyAI.EnSex.Character, 0, "_SmoothnessDeviate", gloss);
        UpdateMaterialPropertyBlock(EnemyAI.EnSex.Character, 1, "_SmoothnessDeviate", gloss);

        if (EnemyAI.EnSex.EnemyFuta || EnemyAI.EnSex.EnemyMale)
        {
            UpdateMaterialPropertyBlock(EnemyAI.EnSex.Dick, 0, "_SmoothnessDeviate", gloss);
        }

        return gloss >= GlossEffectMod.MaxGloss.Value;
    }

    private void UpdateMaterialPropertyBlock(Renderer renderer, int index, string property, float value)
    {
        if (renderer == null || renderer.WasCollected)
        {
            return;
        }

        if (index < 0 || index >= renderer.sharedMaterials.Count)
        {
            return;
        }

        renderer.GetPropertyBlock(MaterialPropertyBlock, index);
        MaterialPropertyBlock.SetFloat(property, value);
        renderer.SetPropertyBlock(MaterialPropertyBlock, index);
    }

    private void StopCoroutine()
    {
        if (_smoothnessCoroutine != null)
        {
            MelonCoroutines.Stop(_smoothnessCoroutine);
            _smoothnessCoroutine = null;
        }
    }
}
