using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Solas.GameplayMod.Models;
internal class SexMoveExtended : IComparable<SexMoveExtended> {
    public bool IsDisabled { get; set; }
    public int Type { get; set; }
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CharacterCumCondition CasterCum { get; set; } = CharacterCumCondition.Always;
    public CharacterGender CasterGender { get; set; } = CharacterGender.Any;
    public CharacterRole CasterRole { get; set; } = CharacterRole.Any;
    public CharacterCumCondition TargetCum { get; set; } = CharacterCumCondition.Always;
    public CharacterGender TargetGender { get; set; } = CharacterGender.Any;
    public CharacterRole TargetRole { get; set; } = CharacterRole.Any;
    public bool IsCommand { get; set; }
    public bool IsPerform { get; set; }
    public bool IsDominant { get; set; }
    public bool IsSensual { get; set; }
    public bool IsService { get; set; }
    public bool IsSmothering { get; set; }
    public bool IsUniversal { get; set; }
    public bool IsWresting { get; set; }

    [JsonIgnore]
    public bool IsForeplay => Type is >= 1 and <= 5;

    public int CompareTo(SexMoveExtended other) => other is null ? 1 : ID.CompareTo(other.ID);

    public override bool Equals(object obj) {
        if (obj is null)
            return false;

        if (obj is not SexMoveExtended sexMove2)
            return false;

        return ID == sexMove2.ID;
    }

    public override int GetHashCode() => ID.GetHashCode();
}

internal enum CharacterGender {
    Any,
    Female,
    Male
}

internal enum CharacterRole {
    Any,
    Active,
    Passive
}

[Flags]
internal enum CharacterCumCondition {
    None            = 0,
    FullDominated   = 1 << 0,
    PreDominated    = 1 << 1,
    Idle            = 1 << 2,
    PreDominating   = 1 << 3,
    FullDominating  = 1 << 4,
    SexToy          = 1 << 5,

    Dominated       = PreDominated | FullDominated,
    Dominating      = PreDominating | FullDominating,
    Always          = Dominated | Idle | Dominating,
}