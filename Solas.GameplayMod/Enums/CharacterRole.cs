namespace Solas.GameplayMod.Enums;

[Flags]
internal enum CharacterRole
{
    None = 0,
    Active = 1 << 0,
    Passive = 1 << 1,
    Any = Active | Passive
}
