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
    public override string GenerateData(string? min = "0", string? max = "2")
    {
        // Check for empty strings and set to defaults
        if (string.IsNullOrWhiteSpace(min))
        {
            min = "0";
        }

        if (string.IsNullOrWhiteSpace(max))
        {
            max = "2";
        }
        
        var minValue = double.Parse(min);
        var maxValue = double.Parse(max);

        if (IsInteger(minValue) && IsInteger(maxValue))
        {
            // Generate a random integer
            var minIntValue = (int)minValue;
            var maxIntValue = (int)maxValue;

            var random = new Random();
            var randomNumber = random.Next(minIntValue, maxIntValue);

            return randomNumber.ToString();
        }
        else
        {
            // Generate a random double/float
            var randomValue = minValue + (maxValue - minValue) * new Random().NextDouble();
            return randomValue.ToString();
        }
    }

    // Helper method to check if a double is an integer
    private static bool IsInteger(double value)
    {
        return value == Math.Floor(value);
    }
}


public class PhoneGenerator : DataGenerator
{
    public override string GenerateData(string? min, string? max)
    {
        return "01151234567";
    }
}

public class EmailGenerator : DataGenerator
{
    public override string GenerateData(string? min, string max)
    {
        return "synthetic@example.com";
    }
}

