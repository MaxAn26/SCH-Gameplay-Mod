using System;

using BaseMod.Core.Extensions;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using Solas.GameplayMod.Mods;

using UnityEngine;

namespace Solas.GameplayMod.Components;
internal class EnemyCharacterComponent : MonoBehaviour {
    internal MaterialPropertyBlock MaterialPropertyBlock = new();
    internal SexSystem SexSystem;
    internal EnemyAI EnemyAI;

    internal bool? IsActive;
    internal int LastEcstasy = -1;

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
                if (IsActive is not null) {

                }
            }
        } catch (Exception e) {
            Plugin.Log.Error(e);
            Destroy(this);
        }
    }

    public void LateUpdate() {
        if (LastEcstasy != EnemyAI.healthSystem.CurrentEc) {
            UpdateSmoothnessDeviate();

            LastEcstasy = EnemyAI.healthSystem.CurrentEc;
        }
    }

    private void UpdateSmoothnessDeviate() {
        if (!EnemyAI.EnSex.IsGrappled || !GlossEffectMod.IsModActive)
            return;

        float gloss = GlossEffectMod.GetEnemyGlossEffect(EnemyAI.healthSystem.CurrentEc, EnemyAI.healthSystem.MaxEc);

        UpdateMaterialPropertyBlock(EnemyAI.EnSex.Character, 0, "_SmoothnessDeviate", gloss);
        UpdateMaterialPropertyBlock(EnemyAI.EnSex.Character, 1, "_SmoothnessDeviate", gloss);

        if (EnemyAI.EnSex.EnemyFuta || EnemyAI.EnSex.EnemyMale)             
            UpdateMaterialPropertyBlock(EnemyAI.EnSex.Dick, 0, "_SmoothnessDeviate", gloss);
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