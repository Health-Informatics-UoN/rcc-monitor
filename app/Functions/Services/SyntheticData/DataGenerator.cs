using OfficeOpenXml;

namespace Functions.Services.SyntheticData;

public abstract class DataGenerator
{
    public abstract string GenerateData(string min, string max);
}

public class DateBoxGenerator : DataGenerator
{
    public override string GenerateData(string min, string max)
    {
        return "Generated Date";
    }
}

public class TextGenerator : DataGenerator
{
    public override string GenerateData(string min, string max)
    {
        return "Generated Text";
    }
}

public class NumberGenerator : DataGenerator
{
    public override string GenerateData(string min, string max)
    {
        return "Generated Text";
    }
}
