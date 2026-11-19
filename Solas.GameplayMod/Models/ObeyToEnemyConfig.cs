using Solas.GameplayMod.Enums;

namespace Solas.GameplayMod.Models;
internal class ObeyToEnemyConfig
{
    public Conditions Female { get; set; } = new Conditions();
    public Conditions Futa { get; set; } = new Conditions();
    public Conditions Male { get; set; } = new Conditions();

    internal class Conditions
    {
        public ObeyCondition Base { get; set; } = ObeyCondition.None;
        public ObeyCondition Victim { get; set; } = ObeyCondition.None;
    }
}
