namespace Solas.GameplayMod.Enums;

[Flags]
internal enum CharacterCumCondition
{
    None = 0,
    FullDominated = 1 << 0,
    PreDominated = 1 << 1,
    Idle = 1 << 2,
    PreDominating = 1 << 3,
    FullDominating = 1 << 4,
    SexToy = 1 << 5,

    Dominated = PreDominated | FullDominated,
    Dominating = PreDominating | FullDominating,
    Always = Dominated | Idle | Dominating,
}
