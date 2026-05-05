namespace Solas.DirtyTalkMod.Models;
public class PhraseModel
{
    public List<string> Base { get; set; } = [];
    public List<string> Female { get; set; } = [];
    public List<string> Futa { get; set; } = [];
    public List<string> Male { get; set; } = [];

    public PhraseModel()
    {
        Base = [];
        Female = [];
        Futa = [];
        Male = [];
    }

    public PhraseModel(string initValue)
    {
        Base = [initValue];
        Female = [initValue];
        Futa = [initValue];
        Male = [initValue];
    }

    internal List<string> GetPhrases(CharacterGender gender)
    {
        List<string> result = [];
        switch (gender)
        {
            case CharacterGender.Female:
                result.AddRange(Female);
                break;
            case CharacterGender.Futa:
                result.AddRange(Futa);
                break;
            case CharacterGender.Male:
                result.AddRange(Male);
                break;
            default:
                break;
        }

        if (result.Count == 0)
        {
            result.AddRange(Base);
        }

        return result;
    }
}
