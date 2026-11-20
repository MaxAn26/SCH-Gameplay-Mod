using System.Text.Json.Serialization;
using Il2Cpp;
using Solas.GameplayMod.Enums;

namespace Solas.GameplayMod.Models;
internal class SexMoveExtended : IComparable<SexMoveExtended>
{
    public bool IsDisabled { get; set; }
    public int Type { get; set; }
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Character Caster { get; set; } = new();
    public Character Target { get; set; } = new();
    public SexTag SexTags { get; set; } = SexTag.None;
    public bool IsCommand { get; set; }
    public bool IsPerform { get; set; }
    public bool IsDominant { get; set; }
    public bool IsSensual { get; set; }
    public bool IsService { get; set; }
    public bool IsSmothering { get; set; }
    public bool IsUniversal { get; set; }
    public bool IsWresting { get; set; }
    public bool IsSpanking { get; set; }

    [JsonIgnore]
    public bool IsForeplay => Type is >= 1 and <= 5;

    public int CompareTo(SexMoveExtended other) => other is null ? 1 : ID.CompareTo(other.ID);

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is not SexMoveExtended sexMove2)
        {
            return false;
        }

        return ID == sexMove2.ID;
    }

    public override int GetHashCode() => ID.GetHashCode();

    public static SexMoveExtended FromSexMove(SexMove sexMove) => new() { Type = sexMove.Type, ID = sexMove.ID, Name = sexMove.Name, Description = sexMove.Description };

    internal class Character
    {
        public CharacterCumCondition Cum { get; set; } = CharacterCumCondition.Always;
        public CharacterGender Gender { get; set; } = CharacterGender.Any;
        public CharacterRole Role { get; set; } = CharacterRole.Any;
    }
}
