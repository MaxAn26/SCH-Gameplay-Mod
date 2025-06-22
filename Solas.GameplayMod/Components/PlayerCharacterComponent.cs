using System;
using System.Collections;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Unity.IL2CPP.Utils.Collections;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using Solas.GameplayMod.Mods;

using UnityEngine;

namespace Solas.GameplayMod.Components;
internal class PlayerCharacterComponent : MonoBehaviour {
    internal MaterialPropertyBlock MaterialPropertyBlock = new();
    internal SexSystem SexSystem;
    internal PlayerCombat PlayerCombat;

    private int _glossTick = 0;

    #region Il2Cpp .ctor
    static PlayerCharacterComponent() {
        ClassInjector.RegisterTypeInIl2Cpp<PlayerCharacterComponent>();
    }

    public PlayerCharacterComponent() : base(ClassInjector.DerivedConstructorPointer<PlayerCharacterComponent>()) {
        ClassInjector.DerivedConstructorBody(this);
    }

    public PlayerCharacterComponent(IntPtr pointer) : base(pointer) {

    }
    #endregion

    public void Initialize() {
        try {
            if (gameObject.TryGetComponentWithCast(out PlayerCombat playerCombat)) {
                PlayerCombat = playerCombat;
                SexSystem = playerCombat.Sexscript;
            } else {
                Destroy(this);
            }

            UpdateSmoothnessDeviate();
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

        if (!UpdateSmoothnessDeviate() && !SexSystem.GameOver){
            _glossTick++;
            StartCoroutine(UpdateSmoothness().WrapToIl2Cpp());
        }
    }

    private bool UpdateSmoothnessDeviate() {
        if (!GlossEffectMod.IsModActive)
            return true;

        float gloss = GlossEffectMod.GetGlossEffect(_glossTick);

        UpdateMaterialPropertyBlock(PlayerCombat.playerSex.Character, 0, "_SmoothnessDeviate", gloss);
        UpdateMaterialPropertyBlock(PlayerCombat.playerSex.Character, 1, "_SmoothnessDeviate", gloss);

        if (CharacterData.Instance.Ismale || CharacterData.Instance.adultSettingsDATA.FutaPlayerMode)             
            UpdateMaterialPropertyBlock(PlayerCombat.playerSex.Dick, 0, "_SmoothnessDeviate", gloss);

        return gloss >= GlossEffectMod.MaxGloss.Value;
    }

    private void UpdateMaterialPropertyBlock(Renderer renderer, int index, string property, float value) {
        renderer.GetPropertyBlock(MaterialPropertyBlock, index);
        MaterialPropertyBlock.SetFloat(property, value);
        renderer.SetPropertyBlock(MaterialPropertyBlock, index);
    }

    [HideFromIl2Cpp]
    public static void RegisterClass(PlayerCombat playerCombat) {
        playerCombat.gameObject.AddComponentWithAction<PlayerCharacterComponent>(component => component.Initialize());
    }
}
