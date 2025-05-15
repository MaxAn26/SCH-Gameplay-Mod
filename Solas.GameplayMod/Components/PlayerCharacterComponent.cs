using System;

using BaseMod.Core.Extensions;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using Solas.GameplayMod.Mods;

using UnityEngine;

namespace Solas.GameplayMod.Components;
internal class PlayerCharacterComponent : MonoBehaviour {
    internal MaterialPropertyBlock MaterialPropertyBlock = new();
    internal SexSystem SexSystem;
    internal PlayerCombat PlayerCombat;

    internal int LastEcstasy = -1;

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
        } catch (Exception e) {
            Plugin.Log.Error(e);
            Destroy(this);
        }
    }

    public void LateUpdate() {
        if (LastEcstasy != PlayerCombat.healthSystem.CurrentAr + PlayerCombat.healthSystem.CurrentEc) {
            UpdateSmoothnessDeviate();

            LastEcstasy = PlayerCombat.healthSystem.CurrentAr + PlayerCombat.healthSystem.CurrentEc;
        }
    }

    private void UpdateSmoothnessDeviate() {
        if (!GlossEffectMod.IsModActive)
            return;

        float gloss = GlossEffectMod.GetPlayerGlossEffect(PlayerCombat.healthSystem.CurrentAr, PlayerCombat.healthSystem.MaxAr, PlayerCombat.healthSystem.CurrentEc, PlayerCombat.healthSystem.MaxEc);

        UpdateMaterialPropertyBlock(PlayerCombat.playerSex.Character, 0, "_SmoothnessDeviate", gloss);
        UpdateMaterialPropertyBlock(PlayerCombat.playerSex.Character, 1, "_SmoothnessDeviate", gloss);

        if (CharacterData.Instance.Ismale || CharacterData.Instance.adultSettingsDATA.FutaPlayerMode)             
            UpdateMaterialPropertyBlock(PlayerCombat.playerSex.Dick, 0, "_SmoothnessDeviate", gloss);
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
