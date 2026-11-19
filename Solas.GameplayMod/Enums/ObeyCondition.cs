namespace Solas.GameplayMod.Enums;

[Flags]
internal enum ObeyCondition
{
    None = 0,

    SameGender = 1 << 0,
    SameRole = 1 << 1,

    EnemyPower = 1 << 2,
    Bound = 1 << 3,
}
