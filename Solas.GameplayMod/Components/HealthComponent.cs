using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseMod.Core.Extensions;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using UnityEngine;

namespace Solas.GameplayMod.Components;
internal class HealthComponent : MonoBehaviour {
    private PlayerHealthSystem _playerHealth;
    private HealthSystem _enemyHealth;

    #region Il2Cpp .ctor
    static HealthComponent() {
        ClassInjector.RegisterTypeInIl2Cpp<HealthComponent>();
    }

    public HealthComponent() : base(ClassInjector.DerivedConstructorPointer<HealthComponent>()) {
        ClassInjector.DerivedConstructorBody(this);
    }

    public HealthComponent(IntPtr pointer) : base(pointer) {

    }
    #endregion

    public void Initialize() {
        try {
            if (gameObject.TryGetComponentWithCast(out PlayerHealthSystem playerHealthSystem)) {
                _playerHealth = playerHealthSystem;
            } else if (gameObject.TryGetComponentWithCast(out HealthSystem enemyHealthSystem)) {
                _enemyHealth = enemyHealthSystem;
            } else {
                Destroy(this);
            }
        } catch (Exception e) {
            Plugin.Log.Error(e);
            Destroy(this);
        }
    }

    public int GetArousal() {
        if (_playerHealth is not null)
            return _playerHealth.CurrentAr;

        if (_enemyHealth is not null){
            int maxAr = GetMaxArousal();
            return _enemyHealth.CurrentEc > maxAr ? maxAr : _enemyHealth.CurrentEc;
        }

        return 0;
    }

    public int GetMaxArousal() {
        if (_playerHealth is not null)
            return _playerHealth.MaxAr;

        if (_enemyHealth is not null)
            return _enemyHealth.MaxEc / 2;

        return 0;
    }

    public int GetEcstasy() {
        if (_playerHealth is not null)
            return _playerHealth.CurrentEc;

        if (_enemyHealth is not null)
            return _enemyHealth.CurrentEc;

        return 0;
    }

    public int GetMaxEcstasy() {
        if (_playerHealth is not null)
            return _playerHealth.MaxEc;

        if (_enemyHealth is not null)
            return _enemyHealth.MaxEc;

        return 0;
    }

    [HideFromIl2Cpp]
    public static void RegisterClass(MonoBehaviour monoBehaviour) {
        monoBehaviour.gameObject.AddComponentWithAction<HealthComponent>(component => component.Initialize());
    }
}