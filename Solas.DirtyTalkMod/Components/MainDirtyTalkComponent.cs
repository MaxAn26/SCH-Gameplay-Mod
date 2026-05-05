using System.Collections;

using BaseMod.Core.Extensions;
using BaseMod.Core.Interfaces;
using BaseMod.Core.Utils;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using Solas.DirtyTalkMod.Mods;

using UnityEngine;

namespace Solas.DirtyTalkMod.Components;
public class MainDirtyTalkComponent : MonoBehaviour, IInitializeComponent
{
    private bool _cumThoughtRunning = false;
    private SexSystem _sexSystem;
    private float _dirtyTalkTimeout = 15;
    private object _cumCheckEnumeratorState;
    private object _dirtyTalkEnumeratorState;

    static MainDirtyTalkComponent() => ClassInjector.RegisterTypeInIl2Cpp<MainDirtyTalkComponent>();

    public MainDirtyTalkComponent() : base(ClassInjector.DerivedConstructorPointer<MainDirtyTalkComponent>()) => ClassInjector.DerivedConstructorBody(this);

    public MainDirtyTalkComponent(IntPtr pointer) : base(pointer)
    {

    }

    public void Initialize()
    {
        if (Zessentials.Instance.gameObject.TryGetComponentWithCast(out SexSystem sexSystem))
        {
            _sexSystem = sexSystem;
            _dirtyTalkTimeout = CharacterData.Instance.adultSettingsDATA.Dtalkfrequency switch
            {
                0 => 15,
                1 => 30,
                2 => 60,
                _ => 15
            };

            StopCoroutines();
            _cumCheckEnumeratorState = MelonCoroutines.Start( CumCheckEnumerator());
            _dirtyTalkEnumeratorState = MelonCoroutines.Start( DirtyTalkEnumerator());
        }
        else
        {
            Destroy(this);
        }
    }

    public void OnDestroy() => StopCoroutines();

    private void StopCoroutines()
    {
        if (_cumCheckEnumeratorState != null)
        {
            MelonCoroutines.Stop(_cumCheckEnumeratorState);
        }

        if (_dirtyTalkEnumeratorState != null)
        {
            MelonCoroutines.Stop(_dirtyTalkEnumeratorState);
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator CumCheckEnumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (SexSystem.IsCumming && !_cumThoughtRunning)
            {
                MelonCoroutines.Start( CumEnumerator());
                _cumThoughtRunning = true;
            }
            else if (!SexSystem.IsCumming)
            {
                _cumThoughtRunning = false;
            }
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator DirtyTalkEnumerator()
    {
        yield return new WaitForSeconds(5f);
        while (true)
        {
            if (SexSystem.Sexstatus is SEXSTATUS.Idle)
            {
                if (CharacterData.Instance.statusDATA.IsBoundHeavyRestraint > 0)
                {
                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.IsHeavyBondage();
                }
                else if (CharacterData.Instance.statusDATA.IsBoundHandRestraint > 0 && CharacterData.Instance.statusDATA.IsBoundLegRestraint > 0)
                {
                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.IsBound();
                }
                else if (CharacterData.Instance.statusDATA.IsBoundVibrator > 0)
                {
                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.IsVibed();
                }
                else if (_sexSystem.playerHealthSystem.CurrentAr >= _sexSystem.playerHealthSystem.MaxAr * 0.75)
                {
                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.Aroused();
                }
                else
                {
                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.Idle();
                }
            }
            else if (SexSystem.Sexstatus is SEXSTATUS.Fucking && !SexSystem.IsCumming)
            {
                if (_sexSystem.playerHealthSystem.CurrentEc >= _sexSystem.playerHealthSystem.MaxEc * 0.8)
                {
                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.PlayerPrecum();
                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.PlayerPrecum();
                }
                else if (_sexSystem.enemyHealthSystem.CurrentEc >= _sexSystem.enemyHealthSystem.MaxEc * 0.8)
                {
                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.EnemyPrecum();
                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.EnemyPrecum();
                }
                else
                {
                    switch (_sexSystem.SexType)
                    {
                        case 1:
                            if (SexSystem.PlayerAttacker)
                            {
                                if (SexSystem.SexIsLickingCaster)
                                {
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralLickThought();
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOralTakerTaunt();
                                }
                                else if (SexSystem.SexIsOralCaster)
                                {
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralSuckThought();
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOralTakerTaunt();
                                }
                                else if (SexSystem.SexIsLickingTarget)
                                {
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOralLickTaunt();
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralTakerThought();
                                }
                                else if (SexSystem.SexIsOralTarget)
                                {
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOralSuckTaunt();
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralTakerThought();
                                }
                            }
                            else
                            {
                                if (SexSystem.SexIsLickingCaster)
                                {
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOralLickTaunt();
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralTakerThought();
                                }
                                else if (SexSystem.SexIsOralCaster)
                                {
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOralSuckTaunt();
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralTakerThought();
                                }
                                else if (SexSystem.SexIsLickingTarget)
                                {
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOralTakerTaunt();
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralLickThought();
                                }
                                else if (SexSystem.SexIsOralTarget)
                                {
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOralTakerTaunt();
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOralSuckThought();
                                }
                            }

                            break;
                        case 2:
                            if (SexSystem.PlayerAttacker)
                            {
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayHandjobThought();
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayHandjobTaunt();
                            }
                            else
                            {
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayHandjobTaunt();
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayHandjobThought();
                            }

                            break;
                        case 3:
                            if (SexSystem.PlayerAttacker)
                            {
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayBoobjobThought();
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayBoobjobTaunt();
                            }
                            else
                            {
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayBoobjobTaunt();
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayBoobjobThought();
                            }

                            break;
                        case 4:
                            if (SexSystem.PlayerAttacker)
                            {
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayFootjobThought();
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayFootjobTaunt();
                            }
                            else
                            {
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayFootjobTaunt();
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayFootjobThought();
                            }

                            break;
                        case 5:
                            if (SexSystem.PlayerAttacker)
                            {
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOtherThought();
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOtherTaunt();
                            }
                            else
                            {
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayOtherTaunt();
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayOtherThought();
                            }

                            break;
                        case 6:
                            if (!_sexSystem.CasterActive && !_sexSystem.TargetActive && !_sexSystem.CasterMale && !_sexSystem.TargetMale)
                            {
                                if (SexSystem.PlayerAttacker)
                                {
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexMissionaryThought();
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexMissionaryTaunt();
                                }
                                else
                                {
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexMissionaryTaunt();
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexMissionaryThought();
                                }
                            }
                            else if (SexSystem.PlayerAttacker && _sexSystem.CasterActive || !SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)
                            {
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexMissionaryPenetrateThought();
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexMissionaryPenetratedTaunt();
                            }
                            else if (!SexSystem.PlayerAttacker && _sexSystem.CasterActive || SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)
                            {
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexMissionaryPenetrateTaunt();
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexMissionaryPenetratedThought();
                            }


                            break;
                        case 7:
                            if (!_sexSystem.CasterActive && !_sexSystem.TargetActive && !_sexSystem.CasterMale && !_sexSystem.TargetMale)
                            {
                                if (SexSystem.PlayerAttacker)
                                {
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexDoggyThought();
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexDoggyTaunt();
                                }
                                else
                                {
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexDoggyTaunt();
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexDoggyThought();
                                }
                            }
                            else if (SexSystem.PlayerAttacker && _sexSystem.CasterActive || !SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)
                            {
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexDoggyPenetrateThought();
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexDoggyPenetratedTaunt();
                            }
                            else if (!SexSystem.PlayerAttacker && _sexSystem.CasterActive || SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)
                            {
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexDoggyPenetrateTaunt();
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexDoggyPenetratedThought();
                            }

                            break;
                        case 8:
                            if (!_sexSystem.CasterActive && !_sexSystem.TargetActive && !_sexSystem.CasterMale && !_sexSystem.TargetMale)
                            {
                                if (SexSystem.PlayerAttacker)
                                {
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexCowgirlThought();
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexCowgirlTaunt();
                                }
                                else
                                {
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexCowgirlTaunt();
                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexCowgirlThought();
                                }
                            }
                            else if (SexSystem.PlayerAttacker && _sexSystem.CasterActive || !SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)
                            {
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexCowgirlPenetrateThought();
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexCowgirlPenetratedTaunt();
                            }
                            else if (!SexSystem.PlayerAttacker && _sexSystem.CasterActive || SexSystem.PlayerAttacker && _sexSystem.TargetActive && !_sexSystem.CasterActive)
                            {
                                _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexCowgirlPenetrateTaunt();
                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexCowgirlPenetratedThought();
                            }

                            break;
                        case 9:
                            if (_sexSystem.IsSex)
                            {
                                if (!_sexSystem.CasterActive && !_sexSystem.TargetActive && !_sexSystem.CasterMale && !_sexSystem.TargetMale)
                                {
                                    if (RandomUtils.Chance(EnemyDirtyTalkMod.AssistChance.Value))
                                    {
                                        _sexSystem.Assist.GetComponentWithCast<EnemyDirtyTalkComponent>()?.AssistSexTaunt();
                                    }
                                    else
                                    {
                                        _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexTaunt();
                                    }

                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexThought();
                                }
                                else if (_sexSystem.CasterActive && _sexSystem.CasterDickRequired)
                                {
                                    if (RandomUtils.Chance(EnemyDirtyTalkMod.AssistChance.Value))
                                    {
                                        _sexSystem.Assist.GetComponentWithCast<EnemyDirtyTalkComponent>()?.AssistSexTaunt();
                                    }
                                    else
                                    {
                                        _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexPenetrateTaunt();
                                    }

                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexPenetratedThought();
                                }
                                else if (!_sexSystem.CasterActive && _sexSystem.TargetActive && _sexSystem.TargetDickRequired)
                                {
                                    if (RandomUtils.Chance(EnemyDirtyTalkMod.AssistChance.Value))
                                    {
                                        _sexSystem.Assist.GetComponentWithCast<EnemyDirtyTalkComponent>()?.AssistSexTaunt();
                                    }
                                    else
                                    {
                                        _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.SexPenetratedTaunt();
                                    }

                                    _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.SexPenetrateThought();
                                }
                            }
                            else
                            {
                                if (RandomUtils.Chance(EnemyDirtyTalkMod.AssistChance.Value) || SexSystem.SexIsLickingCaster || SexSystem.SexIsOralCaster)
                                {
                                    _sexSystem.Assist.GetComponentWithCast<EnemyDirtyTalkComponent>()?.AssistForeplayTaunt();
                                }
                                else
                                {
                                    _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.ForeplayTaunt();
                                }

                                _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.ForeplayThought();
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            yield return new WaitForSeconds(_dirtyTalkTimeout);
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator CumEnumerator()
    {
        yield return new WaitForSeconds(5);

        if (_sexSystem.playerHealthSystem.CurrentEc >= _sexSystem.playerHealthSystem.MaxEc)
        {
            _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.PlayerCum();
            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.PlayerCum();
        }
        else if (_sexSystem.enemyHealthSystem.CurrentEc >= _sexSystem.enemyHealthSystem.MaxEc)
        {
            _sexSystem.Enemy.GetComponentWithCast<EnemyDirtyTalkComponent>()?.EnemyCum();
            _sexSystem.Player.GetComponentWithCast<PlayerDirtyTalkComponent>()?.EnemyCum();
        }
    }
}
