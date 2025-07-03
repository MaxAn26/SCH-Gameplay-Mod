using System.Collections.Generic;

namespace Solas.DirtyTalkMod.Models;
public class PlayerDirtyTalkModel {
    #region Bondage
    public PhraseModel BoundGeneral { get; set; } = new() { Base = ["Stop it!", "Hey no, not restraints!", "Oh no, more restraints!", "These restraints won't stop me...", "Why this keeps happening to me?"] };
    public PhraseModel BoundCollar { get; set; } = new();
    public PhraseModel BoundGag { get; set; } = new();
    public PhraseModel BoundVibrator { get; set; } = new();
    public PhraseModel BoundPlug { get; set; } = new();
    public PhraseModel BoundBlindfold { get; set; } = new();
    public PhraseModel BoundRestraints { get; set; } = new() { Base = ["This changes nothing!", "Crap, I must escape from these restraints!", "Oh this is bad..."] };
    public PhraseModel BoundHandRestraints { get; set; } = new();
    public PhraseModel BoundLegsRestraints { get; set; } = new();
    #endregion

    #region Foreplay
    public PhraseModel Foreplay { get; set; } = new() { Base = ["This feels so good...", "I shouldn't be enjoying this so much...", "I gotta dominate them...", "This is so hot..."] };
    public PhraseModel ForeplayDominant { get; set; } = new() { Base = ["Somehow, I got this!", "Yes, I'm dominating!", "No one can resist me!", "Damn I'm good!"] };
    public PhraseModel ForeplaySubmissive { get; set; } = new() { Base = ["Oh no...I must escape!", "Damn, this is hot...", "Subdue me...", "I can't fight back..."] };

    public PhraseModel OralTaker { get; set; } = new();
    public PhraseModel OralTakerDominant { get; set; } = new();
    public PhraseModel OralTakerSubmissive { get; set; } = new();
    public PhraseModel OralSuck { get; set; } = new();
    public PhraseModel OralSuckDominant { get; set; } = new();
    public PhraseModel OralSuckSubmissive { get; set; } = new();
    public PhraseModel OralLick { get; set; } = new();
    public PhraseModel OralLickDominant { get; set; } = new();
    public PhraseModel OralLickSubmissive { get; set; } = new();

    public PhraseModel Handjob { get; set; } = new();
    public PhraseModel HandjobDominant { get; set; } = new();
    public PhraseModel HandjobSubmissive { get; set; } = new();

    public PhraseModel Boobjob { get; set; } = new();
    public PhraseModel BoobjobDominant { get; set; } = new();
    public PhraseModel BoobjobSubmissive { get; set; } = new();

    public PhraseModel Footjob { get; set; } = new();
    public PhraseModel FootjobDominant { get; set; } = new();
    public PhraseModel FootjobSubmissive { get; set; } = new();

    public PhraseModel Other { get; set; } = new();
    public PhraseModel OtherDominant { get; set; } = new();
    public PhraseModel OtherSubmissive { get; set; } = new();
    #endregion

    #region Sex
    public PhraseModel Sex { get; set; } = new() { Base = ["This feels so good...", "I shouldn't be enjoying this so much...", "I gotta dominate them...", "This is so hot..."] };
    public PhraseModel SexDominant { get; set; } = new() { Base = ["Somehow, I got this!", "Yes, I'm dominating!", "No one can resist me!", "Damn I'm good!"] };
    public PhraseModel SexSubmissive { get; set; } = new() { Base = ["Oh no...I must escape!", "Damn, this is hot...", "Subdue me...", "I can't fight back..."] };
    public PhraseModel SexPenetrate { get; set; } = new();
    public PhraseModel SexPenetrateDominant { get; set; } = new();
    public PhraseModel SexPenetrateSubmissive { get; set; } = new();
    public PhraseModel SexPenetrated { get; set; } = new();
    public PhraseModel SexPenetratedDominant { get; set; } = new();
    public PhraseModel SexPenetratedSubmissive { get; set; } = new();

    public PhraseModel Missionary { get; set; } = new();
    public PhraseModel MissionaryDominant { get; set; } = new();
    public PhraseModel MissionarySubmissive { get; set; } = new();
    public PhraseModel MissionaryPenetrate { get; set; } = new();
    public PhraseModel MissionaryPenetrateDominant { get; set; } = new();
    public PhraseModel MissionaryPenetrateSubmissive { get; set; } = new();
    public PhraseModel MissionaryPenetrated { get; set; } = new();
    public PhraseModel MissionaryPenetratedDominant { get; set; } = new();
    public PhraseModel MissionaryPenetratedSubmissive { get; set; } = new();

    public PhraseModel Doggy { get; set; } = new();
    public PhraseModel DoggyDominant { get; set; } = new();
    public PhraseModel DoggySubmissive { get; set; } = new();
    public PhraseModel DoggyPenetrate { get; set; } = new();
    public PhraseModel DoggyPenetrateDominant { get; set; } = new();
    public PhraseModel DoggyPenetrateSubmissive { get; set; } = new();
    public PhraseModel DoggyPenetrated { get; set; } = new();
    public PhraseModel DoggyPenetratedDominant { get; set; } = new();
    public PhraseModel DoggyPenetratedSubmissive { get; set; } = new();

    public PhraseModel Cowgirl { get; set; } = new();
    public PhraseModel CowgirlDominant { get; set; } = new();
    public PhraseModel CowgirlSubmissive { get; set; } = new();
    public PhraseModel CowgirlPenetrate { get; set; } = new();
    public PhraseModel CowgirlPenetrateDominant { get; set; } = new();
    public PhraseModel CowgirlPenetrateSubmissive { get; set; } = new();
    public PhraseModel CowgirlPenetrated { get; set; } = new();
    public PhraseModel CowgirlPenetratedDominant { get; set; } = new();
    public PhraseModel CowgirlPenetratedSubmissive { get; set; } = new();
    #endregion

    #region Cum
    public PhraseModel EnemyPrecum { get; set; } = new() { Base = ["Seems you ready to cum", "Wanna cum?"] };
    public PhraseModel EnemyCum { get; set; } = new() { Base = ["I'm better in this", "You don't have any chance"] };

    public List<string> PlayerPrecum { get; set; } = ["I'm too close...", "I'm ready to cum..."];
    public List<string> PlayerCum { get; set; } = ["I'm gonna cum!", "Oh no, I'm gonna cum!", "Cummingggggg!"];
    #endregion

    #region Grapple
    public PhraseModel PlayerGrapple { get; set; } = new() { Base = ["Let's have some fun", ""] };
    #endregion

    #region Idle
    public List<string> Idle { get; set; } = ["Let's go!", "I'm doing very good today...", "This is too easy...", "Time to do heroic stuff...", "Here comes the bad guys...", "I'm awesome!", "Just gotta keep beating these villains for a bit more..."];
    public List<string> Aroused { get; set; } = ["Why I'm so horny?", "I'm supposed to be fighting, but I'm too horny!", "Why are the bad guys so hot today?"];
    public List<string> IsVibed { get; set; } = [];
    public List<string> IsBound { get; set; } = ["Oh no, I'm fully bound and completely defenseless!", "I can't do anything in these bonds!", "Damnit, I can't escape these bonds!"];
    public List<string> IsHeavyBondage { get; set; } = ["What a hell??", "I should escape. NOW!"];
    public List<string> CapturedFirstTime { get; set; } = ["Where am I?", "Why I'm bound?", "What is this place? who are they?"];
    public List<string> CapturedAgain { get; set; } = ["Captured again? damn it!", "Oh no, again..."];
    #endregion
}

public class PlayerDirtyTalkEnemyTypeModel : PlayerDirtyTalkModel {
    public CharacterGender PlayerGender { get; set; } = CharacterGender.None;
    public CharacterGender EnemyGender { get; set; } = CharacterGender.None;
    public List<int> EnemyTypes { get; set; } = [];
}