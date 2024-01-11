using Microsoft.Extensions.Options;
using Monitor.Models.SyntheticData;
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
        const string eventName = "Test Event Name";
        
        // Act
        SyntheticDataService.GenerateParticipantId(headerRows, subjectColumns, eventName);
        
        // Assert Header Rows
        Assert.Equal(2, headerRows.Count);
        Assert.Contains("participant_id", headerRows);
        Assert.Contains("redcap_event_name", headerRows);

        // Assert Subjects
        Assert.Equal(2, subjectColumns.Count);
        Assert.Equal(SubjectsToGenerate, subjectColumns[0].Count);
        Assert.Equal(SubjectsToGenerate, subjectColumns[1].Count);
        Assert.Equal(eventName, subjectColumns[1][1]);
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

        Assert.All(subjectColumns[0], val => Assert.Equal("Completed", val));
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

        Assert.All(subjectColumns[0], val => Assert.Equal("Completed", val));
        Assert.All(subjectColumns[1], val => Assert.Equal("", val));
    }

    [Fact]
    public void TestHandleCustomFields_SetValidationMinMax()
    {
        // Arrange
        var options = Options.Create(new FieldMappings
        {
            Mappings = new List<FieldMapping>
            {
                new() { FieldName = "weight", MeasurementUnit = "Kg", MinValue = "50", MaxValue = "120" },
            }
        });
        var service = new SyntheticDataService(options);
        var fieldRow = new FieldRow("weight_demo", "Number Box (Integer)", "testCRF", "", "", "", "Kg");

        // Act
        service.HandleCustomFields(fieldRow);

        // Assert
        Assert.Equal("50", fieldRow.ValidationMin);
        Assert.Equal("120", fieldRow.ValidationMax);
    }
    
    [Fact]
    public void TestHandleCustomFields_DontOverrideMinMax()
    {
        // Arrange
        var options = Options.Create(new FieldMappings
        {
            Mappings = new List<FieldMapping>
            {
                new() { FieldName = "weight", MeasurementUnit = "Kg", MinValue = "50", MaxValue = "120" },
            }
        });
        var service = new SyntheticDataService(options);
        var fieldRow = new FieldRow("weight_demo", "Number Box (Integer)", "testCRF", "", "5", "10", "Kg");
    
        // Act
        service.HandleCustomFields(fieldRow);
    
        // Assert
        Assert.Equal("5", fieldRow.ValidationMin);
        Assert.Equal("10", fieldRow.ValidationMax);
    }
    
    [Fact]
    public void TestHandleCustomFields_NoUnitSetValidationMinMax()
    {
        // Arrange
        var options = Options.Create(new FieldMappings
        {
            Mappings = new List<FieldMapping>
            {
                new() { FieldName = "age", MeasurementUnit = "", MinValue = "18", MaxValue = "65" },
            }
        });
        var service = new SyntheticDataService(options);
        var fieldRow = new FieldRow("age", "Number Box (Integer)", "testCRF", "", "", "", "");
    
        // Act
        service.HandleCustomFields(fieldRow);
    
        // Assert
        Assert.Equal("18", fieldRow.ValidationMin);
        Assert.Equal("65", fieldRow.ValidationMax);
    }
}