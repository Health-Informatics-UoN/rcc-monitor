namespace Functions.Services.SyntheticData;

public abstract class DataGenerator
{
    /// <summary>
    /// Generate data between 2 values.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public virtual string GenerateData(string min, string max)
    {
        return "";
    }

    public virtual string GenerateData(List<string> choices)
    {
        return "";
    }
}

public class DateBoxGenerator : DataGenerator
{
    /// <summary>
    /// Generate a random date between two dates.
    /// If there are no min/max provided, default to between a week ago and today.
    /// </summary>
    /// <param name="min">Minimum date.</param>
    /// <param name="max">Maximum date. </param>
    /// <returns></returns>
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
public class ChoicesGenerator : DataGenerator
{
    /// <summary>
    /// Given a list of choices, picks a random choice.
    /// </summary>
    /// <param name="choices"></param>
    /// <returns>The random choice.</returns>
    public override string GenerateData(List<string> choices)
    {
        var random = new Random();
        return choices[random.Next(0, choices.Count)];
    }
}

public class DecimalGenerator : DataGenerator
{
    /// <summary>
    /// Generates a random decimal between two values.
    /// </summary>
    /// <param name="min">The minimum number.</param>
    /// <param name="max">The maximum number.</param>
    /// <returns>A random number between the min/max</returns>
    public override string GenerateData(string min, string max)
    {
        // Safely unwrap min/max and profile defaults if either, or both are empty strings.
        double minValue = string.IsNullOrWhiteSpace(min) ? 0 : double.TryParse(min, out minValue) ? minValue : 0;
        double maxValue = string.IsNullOrWhiteSpace(max) ? (minValue + 2) : double.TryParse(max, out maxValue) ? maxValue : (minValue + 2);
        
        var random = new Random();
        var randomNumber = minValue + (maxValue - minValue) * random.NextDouble();
    
        return randomNumber.ToString();
    }
    
}

public class IntegerGenerator : DataGenerator
{
    /// <summary>
    /// Generates a random integer between two values.
    /// </summary>
    /// <param name="min">The minimum number.</param>
    /// <param name="max">The maximum number.</param>
    /// <returns>A random number between the min/max</returns>
    public override string GenerateData(string min, string max)
    {
        // Safely unwrap min/max and profile defaults if either, or both are empty strings.
        int minValue = string.IsNullOrWhiteSpace(min) ? 0 : int.TryParse(min, out minValue) ? minValue : 0;
        int maxValue = string.IsNullOrWhiteSpace(max) ? (minValue + 2) : int.TryParse(max, out maxValue) ? maxValue : (minValue + 2);
    
        var random = new Random();
        var randomNumber = random.Next(minValue, maxValue + 1);
    
        return randomNumber.ToString();
    }
    
}

public class TextGenerator : DataGenerator
{
    public override string GenerateData(string min, string max)
    {
        return "Generated Text";
    }
}

public class PhoneGenerator : DataGenerator
{
    /// <summary>
    /// Generates a sample phone number.
    /// </summary>
    /// <param name="min">Unused minimum.</param>
    /// <param name="max">Unused maximum.</param>
    /// <returns>A phone number as a string.</returns>
    public override string GenerateData(string min, string max)
    {
        return "01151234567";
    }
}

public class EmailGenerator : DataGenerator
{
    /// <summary>
    /// Generates a sample email address.
    /// </summary>
    /// <param name="min">Unused minimum.</param>
    /// <param name="max">Unused maximum.</param>
    /// <returns>An email address as a string.</returns>
    public override string GenerateData(string min, string max)
    {
        return "synthetic@example.com";
    }
}

