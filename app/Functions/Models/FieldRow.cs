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
    public List<int> CleanedChoices { get; set; } = new List<int>();

    /// <summary>
    /// Clean the list of choices into a usable list.
    /// </summary>
    private void CleanChoices()
    {
        CleanedChoices = Choices.Split('|')
            .Select(choice =>
            {
                var parts = choice.Split(',');
                if (parts.Length > 0 && int.TryParse(parts[0].Trim('"').Trim(), out var number))
                {
                    return number;
                }
                return -1; // Default value for invalid choices
            })
            .Where(number => number != -1) // Filter out invalid choices
            .OrderBy(number => number) // Sort the numbers
            .ToList();
    }
}