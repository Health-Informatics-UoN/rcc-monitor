using OfficeOpenXml;

namespace Functions.Services.SyntheticData;

public abstract class DataGenerator
{
    public abstract string GenerateData(string? min, string? max);
}

public class DateBoxGenerator : DataGenerator
{
    public override string GenerateData(string? min, string? max)
    {
        // Default min date to a week ago
        var minDate = !string.IsNullOrEmpty(min) ? DateTime.Parse(min) : DateTime.Now.AddDays(-7);

        // Default max date to today
        var maxDate = !string.IsNullOrEmpty(max) ? DateTime.Parse(max) : DateTime.Now;

        // Generate a random date between minDate and maxDate
        var random = new Random();
        var range = (maxDate - minDate).Days;
        var generatedDate = minDate.AddDays(random.Next(range));
        
        // The default RedCap import date format
        return generatedDate.ToString("yyyy-MM-dd");
    }
}

public class TextGenerator : DataGenerator
{
    public override string GenerateData(string? min, string? max)
    {
        return "Generated Text";
    }
}

public class NumberGenerator : DataGenerator
{
    public override string GenerateData(string? min = "0", string? max = "1")
    {
        var minValue = int.Parse(min ?? "0");
        var maxValue = int.Parse(max ?? "1");
        
        var random = new Random();
        var randomNumber = random.Next(minValue, maxValue + 1);
        
        return randomNumber.ToString();
    }
}

