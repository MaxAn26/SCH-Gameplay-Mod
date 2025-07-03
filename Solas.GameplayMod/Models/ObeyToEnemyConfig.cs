namespace Solas.GameplayMod.Models;
public class ObeyToEnemyConfig {
    public bool OnHandRestraints { get; set; }
    public bool PlayerVictimOnHandRestraints { get; set; }
    public bool AtSameRole { get; set; }
    public bool PlayerVictimAtSameRole { get; set; }
    public bool AtSameGender { get; set; }
    public bool PlayerVictimAtSameGender { get; set; }
    public bool RandomByEnemyPower { get; set; }
    public bool PlayerVictimRandomByEnemyPower { get; set; }
}