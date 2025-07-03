using System;
using System.Collections.Generic;
using System.Linq;

using BaseMod.Core.Extensions;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using Solas.DirtyTalkMod.Models;
using Solas.DirtyTalkMod.Mods;

using UnityEngine;

namespace Solas.DirtyTalkMod.Components;
public class EnemyDirtyTalkComponent : MonoBehaviour {
    private EnemyAI _enemyAI;
    private SexSystem _sexSystem;
    private EnemyDirtyTalkModel _activeEnemy;
    private EnemyDirtyTalkModel _passiveEnemy;

    static EnemyDirtyTalkComponent() {
        ClassInjector.RegisterTypeInIl2Cpp<EnemyDirtyTalkComponent>();
    }

    public EnemyDirtyTalkComponent() : base(ClassInjector.DerivedConstructorPointer<EnemyDirtyTalkComponent>()) {
        ClassInjector.DerivedConstructorBody(this);
    }

    public EnemyDirtyTalkComponent(IntPtr pointer) : base(pointer) {

    }

    public void Initialize() {
        if (gameObject.TryGetComponentWithCast(out EnemyAI enemyAI)) {
            Initialise(enemyAI);
        } else {
            Destroy(this);
            Plugin.Log.Info("EnemyComponent Destroyed");
        }
    }

    public void Initialise(EnemyAI enemyAI) {
        SexSystem sexSystem = GameObject.Find("Z Essentials(Clone)").GetComponentWithCast<SexSystem>();
        if (enemyAI is not null && sexSystem is not null) {
            _enemyAI = enemyAI;
            _sexSystem = sexSystem;

            if (_enemyAI.EnSex.EnemyMale) {
                if (EnemyDirtyTalkMod.EnemyTypes.Any(m => m.Gender is CharacterGender.Male && m.EnemyTypes.Contains(enemyAI.typeOfEnemy))) {
                    var model = EnemyDirtyTalkMod.EnemyTypes.Where(m => m.Gender is CharacterGender.Male && m.EnemyTypes.Contains(enemyAI.typeOfEnemy)).RandomItem();
                    _activeEnemy = model;
                    _passiveEnemy = model;
                } else {
                    _activeEnemy = EnemyDirtyTalkMod.MaleEnemy;
                    _passiveEnemy = EnemyDirtyTalkMod.MaleEnemy;
                }
            } else {
                if (EnemyDirtyTalkMod.EnemyTypes.Any(m => m.Gender is CharacterGender.Female && m.EnemyTypes.Contains(enemyAI.typeOfEnemy))) {
                    _passiveEnemy = EnemyDirtyTalkMod.EnemyTypes.Where(m => m.Gender is CharacterGender.Female && m.EnemyTypes.Contains(enemyAI.typeOfEnemy)).RandomItem();
                } else {
                    _passiveEnemy = EnemyDirtyTalkMod.FemaleEnemy;
                }

                if (EnemyDirtyTalkMod.EnemyTypes.Any(m => m.Gender is CharacterGender.Futa && m.EnemyTypes.Contains(enemyAI.typeOfEnemy))) {
                    _passiveEnemy = EnemyDirtyTalkMod.EnemyTypes.Where(m => m.Gender is CharacterGender.Futa && m.EnemyTypes.Contains(enemyAI.typeOfEnemy)).RandomItem();
                } else {
                    _passiveEnemy = EnemyDirtyTalkMod.FutaEnemy;
                }
            }
        } else {
            Destroy(this);
            Plugin.Log.Info("EnemyComponent Destroyed");
        }
    }

    #region Bondage
    public void BoundGeneral() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        List<string> phrases = dirtyTalkModel.BoundGeneral.GetPhrases(playerGender);

        WriteMessage(phrases);
    }

    public void BoundCollar() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        List<string> phrases = dirtyTalkModel.BoundCollar.GetPhrases(playerGender);

        WriteMessage(phrases);
    }

    public void BoundGag() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        List<string> phrases = dirtyTalkModel.BoundGag.GetPhrases(playerGender);

        WriteMessage(phrases);
    }

    public void BoundVibrator() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        List<string> phrases = dirtyTalkModel.BoundVibrator.GetPhrases(playerGender);

        WriteMessage(phrases);
    }

    public void BoundPlug() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        List<string> phrases = dirtyTalkModel.BoundPlug.GetPhrases(playerGender);

        WriteMessage(phrases);
    }

    public void BoundBlindfold() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        List<string> phrases = dirtyTalkModel.BoundBlindfold.GetPhrases(playerGender);

        WriteMessage(phrases);
    }

    public void BoundRestraints() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        List<string> phrases = dirtyTalkModel.BoundRestraints.GetPhrases(playerGender);

        if (phrases.Count == 0)
            BoundGeneral();
        else
            WriteMessage(phrases);
    }

    public void BoundHandRestraints() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        List<string> phrases = dirtyTalkModel.BoundHandRestraints.GetPhrases(playerGender);

        if (phrases.Count == 0)
            BoundRestraints();
        else
            WriteMessage(phrases);
    }

    public void BoundLegsRestraints() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        List<string> phrases = dirtyTalkModel.BoundLegsRestraints.GetPhrases(playerGender);

        if (phrases.Count == 0)
            BoundRestraints();
        else
            WriteMessage(phrases);
    }
    #endregion

    #region Foreplay
    public void ForeplayTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.ForeplayDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.ForeplaySubmissive;
        } else {
            dto = dirtyTalkModel.Foreplay;
        }

        WriteMessage(dto.GetPhrases(playerGender));
    }

    public void ForeplayOralTakerTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.OralTakerDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.OralTakerSubmissive;
        } else {
            dto = dirtyTalkModel.OralTaker;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            ForeplayTaunt();
    }

    public void ForeplayOralSuckTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.OralSuckDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.OralSuckSubmissive;
        } else {
            dto = dirtyTalkModel.OralSuck;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            ForeplayTaunt();
    }

    public void ForeplayOralLickTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.OralLickDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.OralLickSubmissive;
        } else {
            dto = dirtyTalkModel.OralLick;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            ForeplayTaunt();
    }

    public void ForeplayHandjobTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.HandjobDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.HandjobSubmissive;
        } else {
            dto = dirtyTalkModel.Handjob;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            ForeplayTaunt();
    }

    public void ForeplayBoobjobTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.BoobjobDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.BoobjobSubmissive;
        } else {
            dto = dirtyTalkModel.Boobjob;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            ForeplayTaunt();
    }

    public void ForeplayFootjobTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.FootjobDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.FootjobSubmissive;
        } else {
            dto = dirtyTalkModel.Footjob;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            ForeplayTaunt();
    }

    public void ForeplayOtherTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.OtherDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.OtherSubmissive;
        } else {
            dto = dirtyTalkModel.Other;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            ForeplayTaunt();
    }
    #endregion

    #region Sex
    public void SexTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.SexDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.SexSubmissive;
        } else {
            dto = dirtyTalkModel.Sex;
        }

        WriteMessage(dto.GetPhrases(playerGender));
    }

    public void SexPenetrateTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.SexPenetrateDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.SexPenetrateSubmissive;
        } else {
            dto = dirtyTalkModel.SexPenetrate;
        }

        WriteMessage(dto.GetPhrases(playerGender));
    }

    public void SexPenetratedTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.SexPenetratedDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.SexPenetratedSubmissive;
        } else {
            dto = dirtyTalkModel.SexPenetrated;
        }

        WriteMessage(dto.GetPhrases(playerGender));
    }

    public void SexMissionaryTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.MissionaryDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.MissionarySubmissive;
        } else {
            dto = dirtyTalkModel.Missionary;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            SexTaunt();
    }

    public void SexMissionaryPenetrateTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.MissionaryPenetrateDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.MissionaryPenetrateSubmissive;
        } else {
            dto = dirtyTalkModel.MissionaryPenetrate;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            SexPenetrateTaunt();
    }

    public void SexMissionaryPenetratedTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.MissionaryPenetratedDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.MissionaryPenetratedSubmissive;
        } else {
            dto = dirtyTalkModel.MissionaryPenetrated;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            SexPenetratedTaunt();
    }

    public void SexDoggyTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.DoggyDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.DoggySubmissive;
        } else {
            dto = dirtyTalkModel.Doggy;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            SexTaunt();
    }

    public void SexDoggyPenetrateTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.DoggyPenetrateDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.DoggyPenetrateSubmissive;
        } else {
            dto = dirtyTalkModel.DoggyPenetrate;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            SexPenetrateTaunt();
    }

    public void SexDoggyPenetratedTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.DoggyPenetratedDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.DoggyPenetratedSubmissive;
        } else {
            dto = dirtyTalkModel.DoggyPenetrated;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            SexPenetratedTaunt();
    }

    public void SexCowgirlTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.CowgirlDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.CowgirlSubmissive;
        } else {
            dto = dirtyTalkModel.Cowgirl;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            SexTaunt();
    }

    public void SexCowgirlPenetrateTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.CowgirlPenetrateDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.CowgirlPenetrateSubmissive;
        } else {
            dto = dirtyTalkModel.CowgirlPenetrate;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            SexPenetrateTaunt();
    }

    public void SexCowgirlPenetratedTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.CowgirlPenetratedDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.CowgirlPenetratedSubmissive;
        } else {
            dto = dirtyTalkModel.CowgirlPenetrated;
        }

        List<string> phrases = dto.GetPhrases( playerGender );
        if (phrases.Count > 0)
            WriteMessage(phrases);
        else
            SexPenetratedTaunt();
    }
    #endregion

    #region Cum
    public void PlayerPrecum() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
            return;

        WriteMessage(dirtyTalkModel.PlayerPrecum.GetPhrases(playerGender));
    }

    public void PlayerCum() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
            return;

        WriteMessage(dirtyTalkModel.PlayerCum.GetPhrases(playerGender));
    }

    public void EnemyPrecum() {
        var (dirtyTalkModel, _) = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
            return;

        WriteMessage(dirtyTalkModel.EnemyPrecum);
    }

    public void EnemyCum() {
        var (dirtyTalkModel, _) = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
            return;

        WriteMessage(dirtyTalkModel.EnemyCum);
    }
    #endregion

    #region Grapple
    public void EnemyGrapple() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
            return;

        WriteMessage(dirtyTalkModel.EnemyGrapple.GetPhrases(playerGender));
    }

    public void CaptureFirstTime() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
            return;

        WriteMessage(dirtyTalkModel.CapturedFirstTime.GetPhrases(playerGender));
    }

    public void CapturedAgainTime() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
            return;

        WriteMessage(dirtyTalkModel.CapturedAgain.GetPhrases(playerGender));
    }
    #endregion

    #region Assist
    public void ThreesomeStart() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
            return;

        WriteMessage(dirtyTalkModel.ThreesomeStart.GetPhrases(playerGender));
    }

    public void AssistForeplayTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.AssistForeplayDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.AssistForeplaySubmissive;
        } else {
            dto = dirtyTalkModel.AssistForeplay;
        }

        WriteMessage(dto.GetPhrases(playerGender));
    }

    public void AssistSexTaunt() {
        var (dirtyTalkModel, playerGender) = GetDirtyTalkModel();

        if (dirtyTalkModel is null)
            return;

        PhraseModel dto;
        if (_sexSystem.Domination <= 250) {
            dto = dirtyTalkModel.AssistSexDominant;
        } else if (_sexSystem.Domination >= 750) {
            dto = dirtyTalkModel.AssistSexSubmissive;
        } else {
            dto = dirtyTalkModel.AssistSex;
        }

        WriteMessage(dto.GetPhrases(playerGender));
    }
    #endregion

    [HideFromIl2Cpp]
    private (EnemyDirtyTalkModel dirtyTalkModel, CharacterGender playerGender) GetDirtyTalkModel() {
        EnemyDirtyTalkModel dirtyTalkModel;
        CharacterGender gender;
        if (!SexSystem.PlayerAttacker) {
            gender = _sexSystem.TargetMale
                ? CharacterGender.Male
                : _sexSystem.TargetActive ? CharacterGender.Futa : CharacterGender.Female;

            if (_sexSystem.CasterActive) {
                dirtyTalkModel = _activeEnemy;
            } else {
                dirtyTalkModel = _passiveEnemy;
            }
        } else {
            gender = _sexSystem.CasterMale
                ? CharacterGender.Male
                : _sexSystem.CasterActive ? CharacterGender.Futa : CharacterGender.Female;

            if (_sexSystem.TargetActive) {
                dirtyTalkModel = _activeEnemy;
            } else {
                dirtyTalkModel = _passiveEnemy;
            }
        }

        return (dirtyTalkModel, gender);
    }

    [HideFromIl2Cpp]
    private void WriteMessage(List<string> phrases) {
        if (phrases.Count > 0) {
            string phrase = phrases.RandomItem();

            if (!string.IsNullOrWhiteSpace(phrase))
                _sexSystem.console.ConsoleWriteEnemy($"{_enemyAI.enemyName} says: {phrase}");
        }
    }

    [HideFromIl2Cpp]
    public static void RegisterClass(EnemyAI enemyAI) {
        enemyAI.gameObject.AddComponentWithAction<EnemyDirtyTalkComponent>(component => component.Initialize());
    }
}