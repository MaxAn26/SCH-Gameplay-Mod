using System.Collections.Generic;

using BaseMod.Core.Utils;

using UnityEngine;

namespace Solas.GameplayMod.Models;
internal class EnemyTraitModel {
    public int? EnemyType { get; set; }
    public string Name { get; set; }
    public TraitType TraitType { get; set; } = TraitType.General;
    public CharacterInvulnerable Invulnerable { get; set; } = CharacterInvulnerable.None;
    public List<int> SexTypes { get; set; } = [];
    public TraitOperation HealthOperation { get; set; } = TraitOperation.None;
    public int HealthValue { get; set; }
    public TraitOperation RegenerationOperation { get; set; } = TraitOperation.None;
    public int RegenerationValue { get; set; }
    public TraitOperation ArmorOperation { get; set; } = TraitOperation.None;
    public int ArmorValue { get; set; }
    public TraitOperation PowerOperation { get; set; } = TraitOperation.None;
    public int PowerValue { get; set; }
    public TraitOperation ResistanceOperation { get; set; } = TraitOperation.None;
    public int ResistanceValue { get; set; }
    public TraitOperation RestraintChanceOperation { get; set; } = TraitOperation.None;
    public int RestraintChanceValue { get; set; }
    public TraitOperation LustfulOperation { get; set; } = TraitOperation.None;
    public int LustfulValue { get; set; }

    public int CalculateValue(TraitOperation operation, int originValue, int operationValue) {
        int maxValue = 7;
        if (TraitType is TraitType.Enhanced)
            maxValue = 10;
        else if (TraitType is TraitType.MiniBoss)
            maxValue = 14;

        switch (operation) {
            case TraitOperation.Increase:
                return Mathf.Clamp(originValue + operationValue, 0, maxValue);
            case TraitOperation.Decrease:
                return Mathf.Clamp(originValue - operationValue, 0, maxValue);
            case TraitOperation.Replace:
                return Mathf.Clamp(operationValue, 0, maxValue);
            case TraitOperation.Random:
                return RandomUtils.Int32(0, maxValue);
            default:
                break;
        }

        return originValue;
    }
}

public enum TraitType {
    General,
    Enhanced,
    MiniBoss
}

public enum CharacterInvulnerable {
    None,
    SexHealth,
    Pleasure
}

public enum TraitOperation {
    None,
    Increase,
    Decrease,   
    Replace,    
    Random      // 0 - 7/10/14
}