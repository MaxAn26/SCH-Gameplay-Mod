using BaseMod.Core.Utils;
using Solas.GameplayMod.Enums;
using UnityEngine;

namespace Solas.GameplayMod.Models;
internal class EnemyTraitModel
{
    public int? EnemyType { get; set; }
    public string Name { get; set; }
    public TraitType TraitType { get; set; } = TraitType.General;
    public TraitInvulnerable Invulnerable { get; set; } = TraitInvulnerable.None;
    public List<int> SexTypes { get; set; } = [];
    public Trait Armor { get; set; } = new();
    public Trait Health { get; set; } = new();
    public Trait Lustful { get; set; } = new();
    public Trait Power { get; set; } = new();
    public Trait Regeneration { get; set; } = new();
    public Trait Resistance { get; set; } = new();
    public Trait RestraintChance { get; set; } = new();

    public int CalculateTrait(Trait trait, int originValue)
    {
        int maxValue = 7;
        if (TraitType is TraitType.Enhanced)
        {
            maxValue = 10;
        }
        else if (TraitType is TraitType.MiniBoss)
        {
            maxValue = 14;
        }

        return trait.Calculate(originValue, maxValue);
    }

    internal class Trait
    {
        public TraitOperation Operation { get; set; } = TraitOperation.None;
        public int Value { get; set; }

        public int Calculate(int originValue, int maxValue)
        {
            switch (Operation)
            {
                case TraitOperation.Increase:
                    return Mathf.Clamp(originValue + Value, 0, maxValue);
                case TraitOperation.Decrease:
                    return Mathf.Clamp(originValue - Value, 0, maxValue);
                case TraitOperation.Replace:
                    return Mathf.Clamp(Value, 0, maxValue);
                case TraitOperation.Random:
                    int minValue = Mathf.Max(originValue, Value);
                    int min = Mathf.Min(minValue, maxValue);
                    int max = Mathf.Max(minValue, maxValue);

                    return Mathf.Clamp(RandomUtils.Int32(min, max), 0, maxValue);
                default:
                    break;
            }

            return originValue;
        }
    }
}
