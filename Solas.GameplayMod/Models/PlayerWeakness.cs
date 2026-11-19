using Solas.GameplayMod.Enums;

namespace Solas.GameplayMod.Models;
internal class PlayerWeakness
{
    public Config Female { get; set; } = new();
    public Config Futa { get; set; } = new();
    public Config Male { get; set; } = new();

    internal class Config
    {
        public Weaknesses Positive { get; set; } = Weaknesses.None;
        public Weaknesses Negative { get; set; } = Weaknesses.None;

        internal bool ActionByFlag(Enum flag, Action onPositive, Action onNegative)
        {
            if (Positive.HasFlag(flag))
            {
                onPositive?.Invoke();
                return true;
            }
            else if (Negative.HasFlag(flag))
            {
                onNegative?.Invoke();
                return true;
            }

            return false;
        }
    }
}
