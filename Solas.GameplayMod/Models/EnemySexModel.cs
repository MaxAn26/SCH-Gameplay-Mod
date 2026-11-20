using Solas.GameplayMod.Enums;

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

        return (SexTags & sexMove.SexTags) != 0;
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
