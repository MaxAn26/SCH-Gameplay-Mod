using BaseMod.Core.Extensions;
using BaseMod.Core.Interfaces;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using Solas.DirtyTalkMod.Models;
using Solas.DirtyTalkMod.Mods;

using UnityEngine;

namespace Solas.DirtyTalkMod.Components;
public class PlayerDirtyTalkComponent : MonoBehaviour, IInitializeComponent 
{
    public bool IsInitialised = false;

    private SexSystem _sexSystem;
    private PlayerDirtyTalkModel _activePlayer;
    private PlayerDirtyTalkModel _passivePlayer;
    private readonly List<PlayerDirtyTalkEnemyTypeModel> _activePlayerEnemyType = [];
    private readonly List<PlayerDirtyTalkEnemyTypeModel> _passivePlayerEnemyType = [];

    static PlayerDirtyTalkComponent() => ClassInjector.RegisterTypeInIl2Cpp<PlayerDirtyTalkComponent>();

    public PlayerDirtyTalkComponent() : base(ClassInjector.DerivedConstructorPointer<PlayerDirtyTalkComponent>()) => ClassInjector.DerivedConstructorBody(this);

    public PlayerDirtyTalkComponent(IntPtr pointer) : base(pointer)
    {

    }
    public void Initialize()
    {
        if (gameObject.TryGetComponentWithCast(out PlayerCombat _))
        {
            Initialise();

        }
        else
        {
            Destroy(this);
            Core.LogInfo("PlayerComponent Destroyed");
        }
    }

    public void Initialise()
    {
        SexSystem sexSystem = GameObject.Find("Z Essentials(Clone)").GetComponentWithCast<SexSystem>();
        if (sexSystem is not null)
        {
            _sexSystem = sexSystem;

            if (_sexSystem.PlayerMale)
            {
                _activePlayer = PlayerDirtyTalkMod.MalePlayer;
                _passivePlayer = PlayerDirtyTalkMod.MalePlayer;

                _activePlayerEnemyType.Clear();
                _passivePlayerEnemyType.Clear();
                foreach (PlayerDirtyTalkEnemyTypeModel enemyType in PlayerDirtyTalkMod.PlayerEnemyTypes)
                {
                    if (enemyType.PlayerGender is CharacterGender.Male && enemyType.EnemyGender is not CharacterGender.None && enemyType.EnemyTypes.Count > 0)
                    {
                        _activePlayerEnemyType.Add(enemyType);
                        _passivePlayerEnemyType.Add(enemyType);
                    }
                }
            }
            else
            {
                _activePlayer = PlayerDirtyTalkMod.FutaPlayer;
                _passivePlayer = PlayerDirtyTalkMod.FemalePlayer;

                _activePlayerEnemyType.Clear();
                _passivePlayerEnemyType.Clear();
                foreach (PlayerDirtyTalkEnemyTypeModel enemyType in PlayerDirtyTalkMod.PlayerEnemyTypes)
                {
                    if (enemyType.PlayerGender is CharacterGender.Futa && enemyType.EnemyGender is not CharacterGender.None && enemyType.EnemyTypes.Count > 0)
                    {
                        _activePlayerEnemyType.Add(enemyType);
                    }

                    if (enemyType.PlayerGender is CharacterGender.Female && enemyType.EnemyGender is not CharacterGender.None && enemyType.EnemyTypes.Count > 0)
                    {
                        _passivePlayerEnemyType.Add(enemyType);
                    }
                }
            }

            IsInitialised = true;
        }
        else
        {
            Destroy(this);
            Core.LogInfo("PlayerComponent Destroyed");
        }
    }

    #region Idle
    public void Idle()
    {
        PlayerDirtyTalkModel dirtyTalkModel = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.Idle);
    }

    public void Aroused()
    {
        PlayerDirtyTalkModel dirtyTalkModel = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.Aroused);
    }

    public void IsVibed()
    {
        PlayerDirtyTalkModel dirtyTalkModel = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.IsVibed);
    }

    public void IsBound()
    {
        PlayerDirtyTalkModel dirtyTalkModel = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.IsBound);
    }

    public void IsHeavyBondage()
    {
        PlayerDirtyTalkModel dirtyTalkModel = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.IsHeavyBondage);
    }

    public void PlayerGrapple()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.PlayerGrapple.GetPhrases(enemyGender));
    }

    public void CaptureFirstTime()
    {
        PlayerDirtyTalkModel dirtyTalkModel = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.CapturedFirstTime);
    }

    public void CapturedAgainTime()
    {
        PlayerDirtyTalkModel dirtyTalkModel = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.CapturedAgain);
    }
    #endregion

    #region Bondage
    public void BoundGeneral()
    {
        if (_sexSystem.Enemy is null)
        {
            return;
        }

        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        List<string> phrases = dirtyTalkModel.BoundGeneral.GetPhrases(enemyGender);

        WriteMessage(phrases);
    }

    public void BoundCollar()
    {
        if (_sexSystem.Enemy is null)
        {
            return;
        }

        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        List<string> phrases = dirtyTalkModel.BoundCollar.GetPhrases(enemyGender);

        WriteMessage(phrases);
    }

    public void BoundGag()
    {
        if (_sexSystem.Enemy is null)
        {
            return;
        }

        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        List<string> phrases = dirtyTalkModel.BoundGag.GetPhrases(enemyGender);

        WriteMessage(phrases);
    }

    public void BoundVibrator()
    {
        if (_sexSystem.Enemy is null)
        {
            return;
        }

        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        List<string> phrases = dirtyTalkModel.BoundVibrator.GetPhrases(enemyGender);

        WriteMessage(phrases);
    }

    public void BoundPlug()
    {
        if (_sexSystem.Enemy is null)
        {
            return;
        }

        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        List<string> phrases = dirtyTalkModel.BoundPlug.GetPhrases(enemyGender);

        WriteMessage(phrases);
    }

    public void BoundBlindfold()
    {
        if (_sexSystem.Enemy is null)
        {
            return;
        }

        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        List<string> phrases = dirtyTalkModel.BoundBlindfold.GetPhrases(enemyGender);

        WriteMessage(phrases);
    }

    public void BoundRestraints()
    {
        if (_sexSystem.Enemy is null)
        {
            return;
        }

        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        List<string> phrases = dirtyTalkModel.BoundRestraints.GetPhrases(enemyGender);

        if (phrases.Count == 0)
        {
            BoundGeneral();
        }
        else
        {
            WriteMessage(phrases);
        }
    }

    public void BoundHandRestraints()
    {
        if (_sexSystem.Enemy is null)
        {
            return;
        }

        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        List<string> phrases = dirtyTalkModel.BoundHandRestraints.GetPhrases(enemyGender);

        if (phrases.Count == 0)
        {
            BoundRestraints();
        }
        else
        {
            WriteMessage(phrases);
        }
    }

    public void BoundLegsRestraints()
    {
        if (_sexSystem.Enemy is null)
        {
            return;
        }

        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        List<string> phrases = dirtyTalkModel.BoundLegsRestraints.GetPhrases(enemyGender);

        if (phrases.Count == 0)
        {
            BoundRestraints();
        }
        else
        {
            WriteMessage(phrases);
        }
    }
    #endregion

    #region Foreplay
    public void ForeplayThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.ForeplayDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.ForeplaySubmissive;
        }
        else
        {
            dto = dirtyTalkModel.Foreplay;
        }

        WriteMessage(dto.GetPhrases(enemyGender));
    }

    public void ForeplayOralTakerThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.OralTakerDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.OralTakerSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.OralTaker;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            ForeplayThought();
        }
    }

    public void ForeplayOralSuckThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.OralSuckDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.OralSuckSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.OralSuck;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            ForeplayThought();
        }
    }

    public void ForeplayOralLickThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.OralLickDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.OralLickSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.OralLick;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            ForeplayThought();
        }
    }

    public void ForeplayHandjobThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.HandjobDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.HandjobSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.Handjob;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            ForeplayThought();
        }
    }

    public void ForeplayBoobjobThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.BoobjobDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.BoobjobSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.Boobjob;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            ForeplayThought();
        }
    }

    public void ForeplayFootjobThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.FootjobDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.FootjobSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.Footjob;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            ForeplayThought();
        }
    }

    public void ForeplayOtherThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.OtherDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.OtherSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.Other;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            ForeplayThought();
        }
    }
    #endregion

    #region Sex
    public void SexThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.SexDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.SexSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.Sex;
        }

        WriteMessage(dto.GetPhrases(enemyGender));
    }

    public void SexPenetrateThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.SexPenetrateDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.SexPenetrateSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.SexPenetrate;
        }

        WriteMessage(dto.GetPhrases(enemyGender));
    }

    public void SexPenetratedThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.SexPenetratedDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.SexPenetratedSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.SexPenetrated;
        }

        WriteMessage(dto.GetPhrases(enemyGender));
    }

    public void SexMissionaryThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.MissionaryDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.MissionarySubmissive;
        }
        else
        {
            dto = dirtyTalkModel.Missionary;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            SexThought();
        }
    }

    public void SexMissionaryPenetrateThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.MissionaryPenetrateDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.MissionaryPenetrateSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.MissionaryPenetrate;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            SexPenetrateThought();
        }
    }

    public void SexMissionaryPenetratedThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.MissionaryPenetratedDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.MissionaryPenetratedSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.MissionaryPenetrated;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            SexPenetratedThought();
        }
    }

    public void SexDoggyThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.DoggyDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.DoggySubmissive;
        }
        else
        {
            dto = dirtyTalkModel.Doggy;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            SexThought();
        }
    }

    public void SexDoggyPenetrateThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.DoggyPenetrateDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.DoggyPenetrateSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.DoggyPenetrate;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            SexPenetrateThought();
        }
    }

    public void SexDoggyPenetratedThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.DoggyPenetratedDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.DoggyPenetratedSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.DoggyPenetrated;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            SexPenetratedThought();
        }
    }

    public void SexCowgirlThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.CowgirlDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.CowgirlSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.Cowgirl;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            SexThought();
        }
    }

    public void SexCowgirlPenetrateThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.CowgirlPenetrateDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.CowgirlPenetrateSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.CowgirlPenetrate;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            SexPenetrateThought();
        }
    }

    public void SexCowgirlPenetratedThought()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);

        if (dirtyTalkModel is null)
        {
            return;
        }

        PhraseModel dto;
        if (_sexSystem.Domination >= 750)
        {
            dto = dirtyTalkModel.CowgirlPenetratedDominant;
        }
        else if (_sexSystem.Domination <= 250)
        {
            dto = dirtyTalkModel.CowgirlPenetratedSubmissive;
        }
        else
        {
            dto = dirtyTalkModel.CowgirlPenetrated;
        }

        List<string> phrases = dto.GetPhrases(enemyGender);
        if (phrases.Count > 0)
        {
            WriteMessage(phrases);
        }
        else
        {
            SexPenetratedThought();
        }
    }
    #endregion

    #region Cum
    public void PlayerPrecum()
    {
        PlayerDirtyTalkModel dirtyTalkModel = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.PlayerPrecum);
    }

    public void PlayerCum()
    {
        PlayerDirtyTalkModel dirtyTalkModel = GetDirtyTalkModel();
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.PlayerCum);
    }

    public void EnemyPrecum()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.EnemyPrecum.GetPhrases(enemyGender));
    }

    public void EnemyCum()
    {
        int? enemyType = _sexSystem.Enemy.GetComponentWithCast<EnemyActions>()?.typeOfEnemy ?? null;
        (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) = GetDirtyTalkModel(enemyType);
        if (dirtyTalkModel is null)
        {
            return;
        }

        WriteMessage(dirtyTalkModel.EnemyCum.GetPhrases(enemyGender));
    }
    #endregion

    [HideFromIl2Cpp]
    private PlayerDirtyTalkModel GetDirtyTalkModel()
    {
        PlayerDirtyTalkModel dirtyTalkModel;
        if (_sexSystem.PlayerMale)
        {
            dirtyTalkModel = _activePlayer;
        }
        else if (CharacterData.Instance.adultSettingsDATA.FutaPlayerMode || CharacterData.Instance.adultSettingsDATA.PlayerRole > 0)
        {
            dirtyTalkModel = _activePlayer;
        }
        else
        {
            dirtyTalkModel = _passivePlayer;
        }

        return dirtyTalkModel;
    }

    [HideFromIl2Cpp]
    private (PlayerDirtyTalkModel dirtyTalkModel, CharacterGender enemyGender) GetDirtyTalkModel(int? enemyType = null)
    {
        PlayerDirtyTalkModel dirtyTalkModel = null;
        CharacterGender gender = CharacterGender.None;
        if (SexSystem.PlayerAttacker)
        {
            gender = _sexSystem.TargetMale
                ? CharacterGender.Male
                : _sexSystem.TargetActive ? CharacterGender.Futa : CharacterGender.Female;

            if (_sexSystem.CasterActive)
            {
                dirtyTalkModel = _activePlayer;
                if (enemyType is null && _activePlayerEnemyType.Any(m => m.EnemyTypes.Contains(enemyType.Value)))
                {
                    dirtyTalkModel = _activePlayerEnemyType.Where(m => m.EnemyGender == gender && m.EnemyTypes.Contains(enemyType.Value)).RandomItem();
                }
            }
            else
            {
                dirtyTalkModel = _passivePlayer;
                if (enemyType is null && _passivePlayerEnemyType.Any(m => m.EnemyTypes.Contains(enemyType.Value)))
                {
                    dirtyTalkModel = _passivePlayerEnemyType.Where(m => m.EnemyGender == gender && m.EnemyTypes.Contains(enemyType.Value)).RandomItem();
                }
            }
        }
        else
        {
            gender = _sexSystem.CasterMale
                ? CharacterGender.Male
                : _sexSystem.CasterActive ? CharacterGender.Futa : CharacterGender.Female;

            if (_sexSystem.TargetActive)
            {
                dirtyTalkModel = _activePlayer;
                if (enemyType is null && _activePlayerEnemyType.Any(m => m.EnemyTypes.Contains(enemyType.Value)))
                {
                    dirtyTalkModel = _activePlayerEnemyType.Where(m => m.EnemyGender == gender && m.EnemyTypes.Contains(enemyType.Value)).RandomItem();
                }
            }
            else
            {
                dirtyTalkModel = _passivePlayer;
                if (enemyType is null && _passivePlayerEnemyType.Any(m => m.EnemyTypes.Contains(enemyType.Value)))
                {
                    dirtyTalkModel = _passivePlayerEnemyType.Where(m => m.EnemyGender == gender && m.EnemyTypes.Contains(enemyType.Value)).RandomItem();
                }
            }
        }

        return (dirtyTalkModel, gender);
    }

    [HideFromIl2Cpp]
    private void WriteMessage(List<string> phrases)
    {
        if (phrases.Count > 0)
        {
            string phrase = phrases.RandomItem();

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                _sexSystem.console.ConsoleWriteThought(phrase);
            }
        }
    }
}
