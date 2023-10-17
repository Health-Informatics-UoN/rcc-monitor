using Functions.Services.SyntheticData;

namespace Francois.Tests.Services;

public class DataGeneratorTests
{
    /// <summary>
    /// Test that a correct integer is generated between min/max.
    /// </summary>
    [Fact]
    public void TestGenerateDataWithInteger()
    {
        // Arrange
        const int minValidation = 5;
        const int maxValidation = 10;
        var generator = new IntegerGenerator();

        // Act
        var response = generator.GenerateData(minValidation.ToString(), maxValidation.ToString());

        // Assert
        Assert.InRange(int.Parse(response), minValidation, maxValidation);
    }  
    
    /// <summary>
    /// Test that a correct integer is generated with a minimum and no maximum.
    /// </summary>
    [Fact]
    public void TestGenerateDataWithIntegerMinRange()
    {
        // Arrange
        const int minValidation = 5;
        var generator = new IntegerGenerator();

        // Act
        var response = generator.GenerateData(minValidation.ToString(), "");

        // Assert
        Assert.InRange(int.Parse(response), minValidation, minValidation + 2);
    } 
    
    /// <summary>
    /// Test that a correct integer is generated with a maximum and no minimum.
    /// </summary>
    [Fact]
    public void TestGenerateDataWithIntegerMaxRange()
    {
        // Arrange
        const int maxValidation = 10;
        var generator = new IntegerGenerator();

        // Act
        var response = generator.GenerateData("", maxValidation.ToString());

        // Assert
        Assert.InRange(int.Parse(response), 0, maxValidation);
    }    
    
    /// <summary>
    /// Test that a correct date is generated between min/max.
    /// </summary>
    [Fact]
    public void TestGenerateDataWithDateBox()
    {
        // Arrange
        var minValidation = new DateTime(2023, 1, 1);
        var maxValidation = new DateTime(2023, 1, 5);
        var generator = new DateBoxGenerator();

        // Act
        var response = generator.GenerateData(minValidation.ToString(), maxValidation.ToString());

        // Assert
        Assert.True(DateTime.Parse(response) >= minValidation && (DateTime.Parse(response) <= maxValidation));
    }   
    
    /// <summary>
    /// Test that a correct date is generated with a minimum and no maximum.
    /// </summary>
    [Fact]
    public void TestGenerateDataWithDateBoxMin()
    {
        // Arrange
        var minValidation = new DateTime(2023, 1, 1);
        var generator = new DateBoxGenerator();

        // Act
        var response = generator.GenerateData(minValidation.ToString(), "");

        // Assert
        var max = DateTime.Now;
        Assert.True(DateTime.Parse(response) >= minValidation && (DateTime.Parse(response) <= max));
    }    
    
    /// <summary>
    /// Test that a correct date is generated with a maximum and no minimum.
    /// </summary>
    [Fact]
    public void TestGenerateDataWithDateBoxMax()
    {
        // Arrange
        var maxValidation = new DateTime(2023, 1, 5);
        var generator = new DateBoxGenerator();

        // Act
        var response = generator.GenerateData("", maxValidation.ToString());

        // Assert
        var minimum = maxValidation.AddDays(-7);
        Assert.True(DateTime.Parse(response) >= minimum && (DateTime.Parse(response) <= maxValidation));
    }
    
    /// <summary>
    /// Test that a correct date is generated with a future minimum, and no maximum.
    /// </summary>
    [Fact]
    public void TestGenerateDataWithDateBoxMinFuture()
    {
        // Arrange
        var minValidation = new DateTime(2070, 1, 1);
        var generator = new DateBoxGenerator();

        // Act
        var response = generator.GenerateData(minValidation.ToString(), "");

        // Assert
        var max = minValidation.AddDays(7);
        Assert.True(DateTime.Parse(response) >= minValidation && (DateTime.Parse(response) <= max));
    }    
    
}