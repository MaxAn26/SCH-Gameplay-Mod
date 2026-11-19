namespace Solas.GameplayMod.Enums;

[Flags]
internal enum CharacterGender
{
    None = 0,
    Female = 1 << 0,
    Futa = 1 << 1,
    Male = 1 << 2,
    Any = Female | Futa | Male
}
