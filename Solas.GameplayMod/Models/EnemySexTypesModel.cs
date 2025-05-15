using System;
using System.Collections.Generic;

namespace Solas.GameplayMod.Models;
internal class EnemySexTypesModel {
    public string Name { get; set; }
    public int EnemyType { get; set; }
    public SexTag SexTags { get; set; } = SexTag.None;
    public bool UseTag { get; set; }
    public List<int> ForeplayIDs { get; set; } = [];
    public List<int> ForeplayTypes { get; set; } = [];
    public List<int> SexIDs { get; set; } = [];
    public List<int> SexTypes { get; set; } = [];

    public bool CheckSexMove( int id ) => ForeplayIDs.Contains( id ) || ForeplayTypes.Contains(id) || SexIDs.Contains(id) || SexTypes.Contains(id);

    public bool CheckSexTag( SexMoveExtended sexMove ) {
        if (SexTags.HasFlag(SexTag.Dominant) && sexMove.IsDominant)
            return true;
        else if (SexTags.HasFlag(SexTag.Sensual) && sexMove.IsSensual)
            return true;
        else if (SexTags.HasFlag(SexTag.Service) && sexMove.IsService)
            return true;
        else if (SexTags.HasFlag(SexTag.Smothering) && sexMove.IsSmothering)
            return true;
        else if (SexTags.HasFlag(SexTag.Wrestling) && sexMove.IsWresting)
            return true;

        return false;
    }

    public override bool Equals(object obj) {
        if (obj is null)
            return false;

        if (obj is not EnemySexTypesModel enemySexTypes)
            return false;

        return EnemyType == enemySexTypes.EnemyType;
    }

    public override int GetHashCode() => EnemyType.GetHashCode();
}

[Flags]
internal enum SexTag {
    None        = 0,
    Dominant    = 1 << 0,
    Sensual     = 1 << 1,
    Service     = 1 << 2,
    Smothering  = 1 << 3,
    Wrestling   = 1 << 4,
}