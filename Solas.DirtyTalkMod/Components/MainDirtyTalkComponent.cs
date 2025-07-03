using System;
using System.Collections;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Unity.IL2CPP.Utils.Collections;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using Solas.DirtyTalkMod.Mods;

using UnityEngine;

namespace Solas.DirtyTalkMod.Components;
public class MainDirtyTalkComponent : MonoBehaviour {
    private bool _cumThoughtRunning = false;
    private SexSystem _sexSystem;
    private float _dirtyTalkTimeout = 15;

    static MainDirtyTalkComponent() {
        ClassInjector.RegisterTypeInIl2Cpp<MainDirtyTalkComponent>();
    }

    public MainDirtyTalkComponent() : base(ClassInjector.DerivedConstructorPointer<MainDirtyTalkComponent>()) {
        ClassInjector.DerivedConstructorBody(this);
    }

    public MainDirtyTalkComponent(IntPtr pointer) : base(pointer) {

    }

    public void Initialize() {
        if (gameObject.TryGetComponentWithCast(out PlayerCombat playerCombat)) {
            _sexSystem = playerCombat.Sexscript;
            _dirtyTalkTimeout = CharacterData.Instance.adultSettingsDATA.Dtalkfrequency switch {
                0 => 15,
                1 => 30,
                2 => 60,
                _ => 15
            };

            StartCoroutine(CumCheckEnumerator().WrapToIl2Cpp());
            StartCoroutine(DirtyTalkEnumerator().WrapToIl2Cpp());
        } else {
            Destroy(this);
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator CumCheckEnumerator() {
        while (true) {
            yield return new WaitForSeconds(1);
            if (SexSystem.IsCumming && !_cumThoughtRunning) {
                StartCoroutine(CumEnumerator().WrapToIl2Cpp());
                _cumThoughtRunning = true;
            } else if (!SexSystem.IsCumming) {
                _cumThoughtRunning = false;
            }
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator DirtyTalkEnumerator() {
        yield return new WaitForSeconds(_dirtyTalkTimeout);
        if (SexSystem.Sexstatus is SEXSTATUS.Idle) {
            if (CharacterData.Instance.statusDATA.IsBoundHeavyRestraint > 0) {
                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.IsHeavyBondage();
            } else if (CharacterData.Instance.statusDATA.IsBoundHandRestraint > 0 && CharacterData.Instance.statusDATA.IsBoundLegRestraint > 0) {
                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.IsBound();
            } else if (CharacterData.Instance.statusDATA.IsBoundVibrator > 0) {
                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.IsVibed();
            } else if (_sexSystem.playerHealthSystem.CurrentAr >= _sexSystem.playerHealthSystem.MaxAr * 0.75) {
                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.Aroused();
            } else {
                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.Idle();
            }
        } else if (SexSystem.Sexstatus is SEXSTATUS.Fucking && !SexSystem.IsCumming) {
            if (_sexSystem.playerHealthSystem.CurrentEc >= _sexSystem.playerHealthSystem.MaxEc * 0.8) {
                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.PlayerPrecum();
                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.PlayerPrecum();
            } else if (_sexSystem.enemyHealthSystem.CurrentEc >= _sexSystem.enemyHealthSystem.MaxEc * 0.8) {
                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.EnemyPrecum();
                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.EnemyPrecum();
            } else {
                switch (_sexSystem.SexType) {
                    case 1:
                        if ((SexSystem.PlayerAttacker && (SexSystem.SexIsLickingCaster || SexSystem.SexIsOralCaster))
                            || (!SexSystem.PlayerAttacker && (SexSystem.SexIsLickingTarget || SexSystem.SexIsOralTarget))) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralTakerThought();
                        } else if ((SexSystem.PlayerAttacker && (SexSystem.SexIsLickingCaster || SexSystem.SexIsOralCaster))
                            || (!SexSystem.PlayerAttacker && (SexSystem.SexIsLickingTarget || SexSystem.SexIsOralTarget))) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralTakerThought();
                        } else if ((SexSystem.PlayerAttacker && SexSystem.SexIsOralTarget)
                            || (!SexSystem.PlayerAttacker && SexSystem.SexIsOralCaster)) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralSuckThought();
                        } else if ((SexSystem.PlayerAttacker && SexSystem.SexIsLickingTarget)
                            || (!SexSystem.PlayerAttacker && SexSystem.SexIsLickingCaster)) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralLickThought();
                        }

                        break;
                    case 2:
                        if (SexSystem.PlayerAttacker) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayHandjobThought();
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayHandjobTaunt();
                        } else {
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayHandjobTaunt();
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayHandjobThought();
                        }

                        break;
                    case 3:
                        if (SexSystem.PlayerAttacker) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayBoobjobThought();
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayBoobjobTaunt();
                        } else {
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayBoobjobTaunt();
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayBoobjobThought();
                        }

                        break;
                    case 4:
                        if (SexSystem.PlayerAttacker) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayFootjobThought();
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayFootjobTaunt();
                        } else {
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayFootjobTaunt();
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayFootjobThought();
                        }

                        break;
                    case 5:
                        if (SexSystem.PlayerAttacker) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOtherThought();
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOtherTaunt();
                        } else {
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOtherTaunt();
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOtherThought();
                        }

                        break;
                    case 6:
                        if (!_sexSystem.CasterActive && !_sexSystem.TargetActive && !_sexSystem.CasterMale && !_sexSystem.TargetMale) {
                            if (SexSystem.PlayerAttacker) {
                                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexMissionaryThought();
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexMissionaryTaunt();
                            } else {
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexMissionaryTaunt();
                                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexMissionaryThought();
                            }
                        } else if ((SexSystem.PlayerAttacker && _sexSystem.CasterActive) || (!SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexMissionaryPenetrateThought();
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexMissionaryPenetratedTaunt();
                        } else if ((!SexSystem.PlayerAttacker && _sexSystem.CasterActive) || (SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)) {
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexMissionaryPenetrateTaunt();
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexMissionaryPenetratedThought();
                        }


                        break;
                    case 7:
                        if (!_sexSystem.CasterActive && !_sexSystem.TargetActive && !_sexSystem.CasterMale && !_sexSystem.TargetMale) {
                            if (SexSystem.PlayerAttacker) {
                                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexDoggyThought();
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexDoggyTaunt();
                            } else {
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexDoggyTaunt();
                                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexDoggyThought();
                            }
                        } else if ((SexSystem.PlayerAttacker && _sexSystem.CasterActive) || (!SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexDoggyPenetrateThought();
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexDoggyPenetratedTaunt();
                        } else if ((!SexSystem.PlayerAttacker && _sexSystem.CasterActive) || (SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)) {
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexDoggyPenetrateTaunt();
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexDoggyPenetratedThought();
                        }

                        break;
                    case 8:
                        if (!_sexSystem.CasterActive && !_sexSystem.TargetActive && !_sexSystem.CasterMale && !_sexSystem.TargetMale) {
                            if (SexSystem.PlayerAttacker) {
                                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexCowgirlThought();
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexCowgirlTaunt();
                            } else {
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexCowgirlTaunt();
                                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexCowgirlThought();
                            }
                        } else if ((SexSystem.PlayerAttacker && _sexSystem.CasterActive) || (!SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)) {
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexCowgirlPenetrateThought();
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexCowgirlPenetratedTaunt();
                        } else if ((!SexSystem.PlayerAttacker && _sexSystem.CasterActive) || (SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)) {
                            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexCowgirlPenetrateTaunt();
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexCowgirlPenetratedThought();
                        }

                        break;
                    case 9:
                        if (_sexSystem.IsSex) {
                            if (!_sexSystem.CasterActive && !_sexSystem.TargetActive && !_sexSystem.CasterMale && !_sexSystem.TargetMale) {
                                if (RandomUtils.Chance(EnemyDirtyTalkMod.AssistChance.Value))
                                    _sexSystem.Assist.GetComponentWithCast<EnemyDirtyTalkComponent>()?.AssistSexTaunt();
                                else
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexTaunt();
                                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexThought();
                            } else if (_sexSystem.CasterActive && _sexSystem.CasterDickRequired) {
                                if (RandomUtils.Chance(EnemyDirtyTalkMod.AssistChance.Value))
                                    _sexSystem.Assist.GetComponentWithCast<EnemyDirtyTalkComponent>()?.AssistSexTaunt();
                                else
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexPenetrateTaunt();
                                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexPenetratedThought();
                            } else if (!_sexSystem.CasterActive && _sexSystem.TargetActive && _sexSystem.TargetDickRequired) {
                                if (RandomUtils.Chance(EnemyDirtyTalkMod.AssistChance.Value))
                                    _sexSystem.Assist.GetComponentWithCast<EnemyDirtyTalkComponent>()?.AssistSexTaunt();
                                else
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexPenetratedTaunt();
                                gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexPenetrateThought();
                            }
                        } else {
                            if (RandomUtils.Chance(EnemyDirtyTalkMod.AssistChance.Value) || SexSystem.SexIsLickingCaster || SexSystem.SexIsOralCaster)
                                _sexSystem.Assist.GetComponentWithCast<EnemyDirtyTalkComponent>()?.AssistForeplayTaunt();
                            else
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayTaunt();
                            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayThought();
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        StartCoroutine(DirtyTalkEnumerator().WrapToIl2Cpp());
    }

    [HideFromIl2Cpp]
    private IEnumerator CumEnumerator() {
        yield return new WaitForSeconds(5);

        if (_sexSystem.playerHealthSystem.CurrentEc >= _sexSystem.playerHealthSystem.MaxEc) {
            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.PlayerCum();
            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.PlayerCum();
        } else if (_sexSystem.enemyHealthSystem.CurrentEc >= _sexSystem.enemyHealthSystem.MaxEc) {
            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.EnemyCum();
            gameObject.GetComponentWithCast<PlayerDirtyTalkComponent>()?.EnemyCum();
        }
    }

    [HideFromIl2Cpp]
    public static void RegisterClass(PlayerCombat playerCombat) {
        playerCombat.gameObject.AddComponentWithAction<MainDirtyTalkComponent>(component => component.Initialize());
    }
}