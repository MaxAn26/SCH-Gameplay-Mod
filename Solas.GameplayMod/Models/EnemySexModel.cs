using System;
using System.Collections.Generic;

namespace Solas.GameplayMod.Models;
internal class EnemySexModel
{
    public string Name { get; set; }
    public int EnemyType { get; set; }
    public int CorruptionChance { get; set; }
    public int ThreesomeChance { get; set; }
    public Fetish AllowedFetishes { get; set; } = Fetish.NoFetish;
    public Weaknesses CasterWeaknesses { get; set; } = Weaknesses.NoWeaknesses;
    public Weaknesses TargetWeaknesses { get; set; } = Weaknesses.NoWeaknesses;
    public bool UseTag { get; set; }
    public SexTag SexTags { get; set; } = SexTag.NoTag;
    public List<int> ForeplayIDs { get; set; } = [];
    public List<int> ForeplayTypes { get; set; } = [];
    public List<int> SexIDs { get; set; } = [];
    public List<int> SexTypes { get; set; } = [];

    public bool CheckSexMove(int id) => ForeplayIDs.Contains(id) || ForeplayTypes.Contains(id) || SexIDs.Contains(id) || SexTypes.Contains(id);

    public bool CheckSexTag(SexMoveExtended sexMove)
    {
        if (SexTags.HasFlag(SexTag.NoTag))
        {
            return true;
        }
        else if (SexTags.HasFlag(SexTag.Dominant) && sexMove.IsDominant)
        {
            return true;
        }
        else if (SexTags.HasFlag(SexTag.Sensual) && sexMove.IsSensual)
        {
            return true;
        }
        else if (SexTags.HasFlag(SexTag.Service) && sexMove.IsService)
        {
            return true;
        }
        else if (SexTags.HasFlag(SexTag.Smothering) && sexMove.IsSmothering)
        {
            return true;
        }
        else if (SexTags.HasFlag(SexTag.Wrestling) && sexMove.IsWresting)
        {
            return true;
        }
        else if (SexTags.HasFlag(SexTag.Punishment) && sexMove.IsSpanking)
        {
            return true;
        }

        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is not EnemySexModel enemySexTypes)
        {
            return false;
        }

        return EnemyType == enemySexTypes.EnemyType;
    }

    public override int GetHashCode() => EnemyType.GetHashCode();
}

[Flags]
internal enum SexTag
{
    None = 0,
    NoTag = 1 << 0,
    Dominant = 1 << 1,
    Sensual = 1 << 2,
    Service = 1 << 3,
    Smothering = 1 << 4,
    Wrestling = 1 << 5,
    Punishment = 1 << 6
}

[Flags]
internal enum Fetish
{
    None = 0,
    NoFetish = 1 << 0,
    Dominant = 1 << 1,
    Submissive = 1 << 2,
    Romance = 1 << 3,
    Masochist = 1 << 4,
    Sadist = 1 << 5,
    Bondage = 1 << 6,
    Wrestling = 1 << 7,
    BreathPlay = 1 << 8,
}

[Flags]
internal enum Weaknesses
{
    None = 0,
    NoWeaknesses = 1 << 0,
    Orals = 1 << 1,
    HandJobs = 1 << 2,
    BreastPlay = 1 << 3,
    FootPlay = 1 << 4,
    MissionaryStyle = 1 << 5,
    DoggyStyle = 1 << 6,
    MountingStyle = 1 << 7,
}
