using System;

using BaseMod.Core.Extensions;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using UnityEngine;

namespace Solas.GameplayMod.Components;
internal class HealthComponent : MonoBehaviour {
    private PlayerHealthSystem _playerHealth;
    private HealthSystem _enemyHealth;
    private bool IsPlayer => _playerHealth is not null;


    internal float LastCumTime;
    internal bool IsCharacterSelfCum;
    internal bool CharacterCum;

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

            LastCumTime = Time.time;
        } catch (Exception e) {
            Plugin.Log.Error(e);
            Destroy(this);
        }
    }

    public void LateUpdate() {
        if (CharacterCum) {
            if (IsPlayer) {
                if (_playerHealth.CurrentEc < _playerHealth.MaxEc * 0.85 ) {
                    CharacterCum = false;
                }
            } else {
                if (_enemyHealth.CurrentEc < _enemyHealth.MaxEc * 0.85) {
                    CharacterCum = false;
                }
            }
        } else {
            if (IsPlayer) {
                if (_playerHealth.CurrentEc >= _playerHealth.MaxEc * 0.85) {
                    CharacterCum = true;
                    IsCharacterSelfCum = _playerHealth.CurrentEc >= _playerHealth.MaxEc;
                }
            } else {
                if (_enemyHealth.CurrentEc >= _enemyHealth.MaxEc * 0.85) {
                    CharacterCum = true;
                    IsCharacterSelfCum = _enemyHealth.CurrentEc >= _enemyHealth.MaxEc;
                }
            }

            LastCumTime = Time.time;
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

    public void AddHealth(int amount) {
        if (IsPlayer) {
            _playerHealth.CurrentHp = Mathf.Clamp(_playerHealth.CurrentHp + amount, 0, _playerHealth.MaxHp);
            _playerHealth.SendHealthUpdateEvent();
        } else {
            _enemyHealth.CurrentHp = Mathf.Clamp(_enemyHealth.CurrentHp + amount, 0, _enemyHealth.MaxHp);
            _enemyHealth.SendHealthUpdateEvent();
        }
    }

    public void SubstractHealth(int amount) {
        if (IsPlayer) {
            _playerHealth.CurrentHp = Mathf.Clamp(_playerHealth.CurrentHp - amount, 0, _playerHealth.MaxHp);
            
            _playerHealth.SendHealthUpdateEvent();
        } else {
            var enemyComponent = gameObject.GetComponentWithCast<EnemyCharacterComponent>();
            if (enemyComponent is not null && SexSystem.Sexstatus is SEXSTATUS.Fucking && !SexSystem.IsCumming) {
                if (enemyComponent.EnemyTrait is not null && enemyComponent.EnemyTrait.Invulnerable is Models.CharacterInvulnerable.SexHealth)
                    amount = 0;
            }

            int sub = amount * (100 - _enemyHealth.Armor) / 100;
            _enemyHealth.CurrentHp = Mathf.Clamp(_enemyHealth.CurrentHp - sub, 0, _enemyHealth.MaxHp);
            
            if (_enemyHealth.isDead()) {
                gameObject.SendMessage("Death", SendMessageOptions.DontRequireReceiver);
            }
            _enemyHealth.SendHealthUpdateEvent();
        }
    }

    public void AddPleasure(int amount) { 
        if (IsPlayer) {
            int arousalRoom = _playerHealth.MaxAr - _playerHealth.CurrentAr;
            int toArousal = Math.Min(amount, arousalRoom);
            _playerHealth.CurrentAr = Mathf.Clamp(_playerHealth.CurrentAr + toArousal, 0, _playerHealth.MaxAr);
            int toEcstasy = amount - toArousal;
            _playerHealth.CurrentEc = Mathf.Clamp(_playerHealth.CurrentEc + toEcstasy, 0, _playerHealth.MaxEc);

            if (_playerHealth.CurrentEc > 0 && _playerHealth.CurrentAr < _playerHealth.MaxAr)
                _playerHealth.CurrentEc = 0;

            PlayerHealthSystem.onArousalChange?.Invoke(_playerHealth.CurrentAr, _playerHealth.CurrentEc);
            if (_playerHealth.CurrentEc >= 675 && SexSystem.Sexstatus == SEXSTATUS.Idle) {
                SexSystem.ShouldMasturbate = true;
            }
        } else {
            var enemyComponent = gameObject.GetComponentWithCast<EnemyCharacterComponent>();
            if (enemyComponent is not null && SexSystem.Sexstatus is SEXSTATUS.Fucking) {
                if (enemyComponent.EnemyTrait is not null && enemyComponent.EnemyTrait.Invulnerable is Models.CharacterInvulnerable.Pleasure) {
                    amount = 0;
                    if (_enemyHealth.CurrentHp > 10)
                        SubstractHealth(10);
                }
            }

            _enemyHealth.CurrentEc = Mathf.Clamp(_enemyHealth.CurrentEc + amount, 0, _enemyHealth.MaxEc);

            _enemyHealth.SendEcUpdateEvent();
        }
    }

    public void ResetPleasure() {
        if (IsPlayer) {
            _playerHealth.CurrentEc = 0;

            PlayerHealthSystem.onArousalChange?.Invoke(_playerHealth.CurrentAr, _playerHealth.CurrentEc);
        } else {
            _enemyHealth.CurrentEc = Mathf.Clamp(_enemyHealth.MaxEc / 2, 0, _enemyHealth.MaxEc);

            _enemyHealth.SendEcUpdateEvent();
        }
    }

    [HideFromIl2Cpp]
    public static void RegisterClass(MonoBehaviour monoBehaviour) {
        monoBehaviour.gameObject.AddComponentWithAction<HealthComponent>(component => component.Initialize());
    }
}