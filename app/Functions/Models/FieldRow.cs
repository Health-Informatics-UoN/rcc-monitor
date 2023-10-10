namespace Functions.Models;

public class FieldRow
{
    public FieldRow(string fieldName, string fieldType, string crfName, string choices, string validationMin, string validationMax)
    {
        FieldName = fieldName;
        FieldType = fieldType;
        CrfName = crfName;
        Choices = choices;
        ValidationMin = validationMin;
        ValidationMax = validationMax;
        CleanChoices();
    }

    public string CrfName { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string Choices { get; set; } = string.Empty;
    public string ValidationMin { get; set; } = string.Empty;
    public string ValidationMax { get; set; } = string.Empty;
    public List<string> CleanedChoices { get; set; } = new List<string>();

    /// <summary>
    /// Clean the list of choices into a usable list.
    /// </summary>
    private void CleanChoices()
    {
        CleanedChoices = Choices.Split('|')
            .Select(choice => choice.Split(',')[0].Trim('"').Trim())
            .ToList();
    }
}