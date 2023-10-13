using Monitor.Services.SyntheticData;

namespace Francois.Tests.Services;

public class SyntheticDataServiceTests
{
    private const int SubjectsToGenerate = 100;
    
    [Fact]
    public void TestGenerateParticipantId()
    {
        // Arrange
        var headerRows = new List<string>();
        var subjectColumns = new List<List<string>>();
        
        // Act
        SyntheticDataService.GenerateParticipantId(headerRows, subjectColumns);
        
        // Assert
        Assert.Equal(2, headerRows.Count);
        Assert.Contains("participant_id", headerRows);
        Assert.Contains("redcap_event_name", headerRows);

        Assert.Equal(2, subjectColumns.Count);
        Assert.Equal(SubjectsToGenerate, subjectColumns[0].Count);
        Assert.Equal(SubjectsToGenerate, subjectColumns[1].Count);
    }
    
    [Fact]
    public void TestGenerateDataWithInteger()
    {
        // Arrange
        var subjectData = new List<string>();
        const string fieldType = "Number Box (Integer)";
        const int minValidation = 0;
        const int maxValidation = 5;

        // Act
        SyntheticDataService.GenerateData(subjectData, fieldType, minValidation.ToString(), maxValidation.ToString());

        // Assert
        Assert.Single(subjectData);
        Assert.NotEmpty(subjectData[0]);
        Assert.InRange(int.Parse(subjectData[0]), minValidation, maxValidation);
    }    
    
    [Fact]
    public void TestGenerateDataWithIntegerMax()
    {
        // Arrange
        var subjectData = new List<string>();
        const string fieldType = "Number Box (Integer)";
        const int maxValidation = 5;

        // Act
        SyntheticDataService.GenerateData(subjectData, fieldType, "", maxValidation.ToString());

        // Assert
        Assert.Single(subjectData);
        Assert.NotEmpty(subjectData[0]);
        Assert.InRange(int.Parse(subjectData[0]), 0, maxValidation);
    }    
    
    [Fact]
    public void TestGenerateDataWithDateBox()
    {
        // Arrange
        var subjectData = new List<string>();
        const string fieldType = "Date Box";
        var minValidation = new DateTime(2023, 1, 1).ToString("yyyy-MM-dd");
        var maxValidation = new DateTime(2023, 1, 5).ToString("yyyy-MM-dd");

        // Act
        SyntheticDataService.GenerateData(subjectData, fieldType, minValidation, maxValidation);

        // Assert
        Assert.Single(subjectData);
        var generatedDate = DateTime.Parse(subjectData[0]);
        Assert.True(generatedDate >= DateTime.Parse(minValidation) && generatedDate <= DateTime.Parse(maxValidation));
    }   
    
    [Fact]
    public void TestGenerateDataWithDateBoxMax()
    {
        // Arrange
        var subjectData = new List<string>();
        const string fieldType = "Date Box";
        var minValidation = new DateTime(2023, 1, 1).ToString("yyyy-MM-dd");
        var maxValidation = new DateTime(2023, 1, 5).ToString("yyyy-MM-dd");

        // Act
        SyntheticDataService.GenerateData(subjectData, fieldType, minValidation, maxValidation);

        // Assert
        Assert.Single(subjectData);
        var generatedDate = DateTime.Parse(subjectData[0]);
        Assert.True(generatedDate >= DateTime.Parse(minValidation) && generatedDate <= DateTime.Parse(maxValidation));
    }
    
    [Fact]
    public void TestGenerateFieldHeaderWithValidFieldName()
    {
        // Arrange
        var headerRows = new List<string>();
        const string fieldName = "Text";

        // Act
        SyntheticDataService.GenerateFieldHeader(headerRows, fieldName);

        // Assert
        Assert.Single(headerRows);
        Assert.Equal(fieldName, headerRows[0]);
    }

    [Fact]
    public void TestGenerateFieldHeaderWithCalcFieldName()
    {
        // Arrange
        var headerRows = new List<string>();
        const string fieldName = "calc";

        // Act
        SyntheticDataService.GenerateFieldHeader(headerRows, fieldName);

        // Assert
        Assert.Empty(headerRows);
    }
    
    [Fact]
    public void TestGenerateCheckboxHeaders()
    {
        // Arrange
        var headerRows = new List<string>();
        const string fieldName = "Gym";
        var choices = new List<string> { "1", "2", "A" };

        // Act
        SyntheticDataService.GenerateCheckboxHeaders(headerRows, fieldName, choices);

        // Assert
        Assert.Equal(3, headerRows.Count);

        Assert.Contains(fieldName + "___1", headerRows);
        Assert.Contains(fieldName + "___2", headerRows);
        Assert.Contains(fieldName + "___A", headerRows);
    }
    
    [Fact]
    public void TestHandleLastCrf()
    {
        // Arrange
        var headerRows = new List<string>();
        var subjectColumns = new List<List<string>>();
        const string currentFormName = "CurrentForm";
        const string previousFormName = "PreviousForm";

        // Act
        SyntheticDataService.HandleLastCrf(headerRows, subjectColumns, currentFormName, previousFormName);

        // Assert
        Assert.Equal(2, headerRows.Count);
        Assert.Contains(currentFormName + "_complete", headerRows);
        Assert.Contains(previousFormName + "_custom_label", headerRows);

        Assert.Equal(2, subjectColumns.Count);
        Assert.Equal(SubjectsToGenerate, subjectColumns[0].Count);
        Assert.Equal(SubjectsToGenerate, subjectColumns[1].Count);

        Assert.All(subjectColumns[0], val => Assert.Equal("1", val));
        Assert.All(subjectColumns[1], val => Assert.Equal("", val));
    }
    
    [Fact]
    public void TestHandleCrfChange()
    {
        // Arrange
        var headerRows = new List<string>();
        var subjectColumns = new List<List<string>>();
        const string currentFormName = "CurrentForm";
        const string previousFormName = "PreviousForm";

        // Act
        SyntheticDataService.HandleCrfChange(headerRows, subjectColumns, currentFormName, previousFormName);

        // Assert
        Assert.Equal(2, headerRows.Count);
        Assert.Contains(previousFormName + "_complete", headerRows);
        Assert.Contains(previousFormName + "_custom_label", headerRows);

        Assert.Equal(2, subjectColumns.Count);
        Assert.Equal(SubjectsToGenerate, subjectColumns[0].Count);
        Assert.Equal(SubjectsToGenerate, subjectColumns[1].Count);

        Assert.All(subjectColumns[0], val => Assert.Equal("1", val));
        Assert.All(subjectColumns[1], val => Assert.Equal("", val));
    }
}