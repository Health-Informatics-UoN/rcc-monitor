using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Functions.Models;
using OfficeOpenXml;

namespace Functions.Services.SyntheticData;

public partial class SyntheticDataService
{
    private const int SubjectsToGenerate = 100;
    private const string EventName = "test event";

    /// <summary>
    /// Generates synthetic data to a .csv file.
    /// </summary>
    public void Generate(string importFilePath = "import_dictionary.csv", string exportFilePath = "export.csv")
    {
        var rows = ReadCsv(importFilePath);
        var syntheticData = GenerateRows(rows);
        
        WriteCsv(syntheticData.headerRow, syntheticData.subjectColumns, exportFilePath);
    }

    /// <summary>
    /// Test generate data against a whole folder of these things
    /// </summary>
    public void GenerateFolder()
    {
        const string importFolder = "redcap-dictionaries/";
        var csvFiles = Directory.GetFiles(importFolder, "*.csv");
        
        
        // for file in import folder if file ends with .csv
        foreach (var file in csvFiles)
        {
            Generate(importFilePath: file, exportFilePath: $"redcap-export/export-{Path.GetFileNameWithoutExtension(file)}.csv");
        }
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static List<FieldRow> ReadCsv(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
        var records = new List<FieldRow>();
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var record = new FieldRow(
                csv.GetField("Variable / Field Name"), 
                csv.GetField("Field Type"),
                csv.GetField("Form Name"),
                csv.GetField("Choices, Calculations, OR Slider Labels"),
                csv.GetField("Text Validation Min"),
                csv.GetField("Text Validation Max")
            );
            records.Add(record);
        }
        return records;
    }
    

    /// <summary>
    /// Gets the field indexes for a set of column labels.
    /// </summary>
    /// <remarks>
    /// This is necessary as field columns are not consistent between data dictionaries,
    /// so we have to go looking for the column indexes we want.
    /// </remarks>
    /// <param name="worksheet">Worksheet to map from.</param>
    /// <returns>A model with indexes for the column names.</returns>
    private static FieldColumns GetHeaderField(ExcelWorksheet worksheet)
    {
        // Mapping between field names and property names
        var fieldMappings = new Dictionary<string, string>
        {
            { "Variable / Field Name", nameof(FieldColumns.FieldName) },
            { "Field Type", nameof(FieldColumns.FieldType) },
            { "Form Name", nameof(FieldColumns.FormName) },
            { "Choices, Calculations, OR Slider Labels", nameof(FieldColumns.Choices) },
            { "Text Validation Min", nameof(FieldColumns.ValidationMin) },
            { "Text Validation Max", nameof(FieldColumns.ValidationMax) }
        };

        var fieldColumns = new FieldColumns();

        // Search the first row, index is 1 based.
        for (var col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            var cellValue = worksheet.Cells[1, col].Text;

            if (!fieldMappings.TryGetValue(cellValue, out var propertyName)) continue;
            // Set property by name
            var propertyInfo = typeof(FieldColumns).GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(fieldColumns, col);
            }
        }

        return fieldColumns;
    }

    
    /// <summary>
    /// Generates rows of synthetic data based on the import file.
    /// </summary>
    /// <remarks>
    /// It works by generating a header row, and a "subject" column of data at a time. 
    /// </remarks>
    /// <param name="worksheet">Path to the .csv to import</param>
    /// <returns>A list of synthetic data.</returns>
    private static (List<string> headerRow, List<List<string>> subjectColumns) GenerateRows(List<FieldRow> rows)
    {
        // var headerFields = GetHeaderField(worksheet);
        var headerRow = new List<string>();
        var subjectColumns = new List<List<string>>();

        GenerateParticipantId(headerRow, subjectColumns);

        // Keep track of the current and previous CRF to generate completion columns
        var currentCrfName = string.Empty;
        var previousCrfName = string.Empty;

        // Skip the header rows
        foreach (var row in rows)
        {
            var subjectData = new List<string>();

            // Add any completion columns if we're changing CRF
            currentCrfName = row.CrfName;
            HandleCrfChange(headerRow, subjectColumns, currentCrfName, previousCrfName);
            previousCrfName = currentCrfName;
            
            // Checkboxes generate multiple columns per field so handled differently
            if (row.FieldType == "checkbox" && !string.IsNullOrEmpty(row.Choices))
            {
                GenerateCheckboxHeaders(headerRow, row.FieldName, row.CleanedChoices);
                
                // Generate a column for each checkbox choice
                foreach (var _ in row.CleanedChoices.Select(_ => new List<string>()))
                {
                    for (var i = 0; i < SubjectsToGenerate; i++)
                    {
                        GenerateData(subjectData, row.FieldType, row.ValidationMin, row.ValidationMax);
                    }
                    
                    // Make a copy so we can get random values for each column
                    subjectColumns.Add(new List<string>(subjectData));
                    subjectData.Clear();
                }
            }
            else
            {
                GenerateFieldHeader(headerRow, row.FieldName);

                // Fix max validation to the number of choices if there are any.
                if (!string.IsNullOrEmpty(row.Choices))
                {
                    row.ValidationMax = row.CleanedChoices.Count.ToString();
                }

                // Generate subjects
                for (var i = 0; i < SubjectsToGenerate; i++)
                {
                    GenerateData(subjectData, row.FieldType, row.ValidationMin, row.ValidationMax);
                }

                subjectColumns.Add(subjectData);
            }
        }

        // Final CRF needs completion columns.
        HandleLastCrf(headerRow, subjectColumns, currentCrfName, previousCrfName);

        return (headerRow, subjectColumns);
    }

    /// <summary>
    /// Generates Participant and Event Name header row and columns.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="subjectColumns">List of subjects the columns are added to.</param>
    private static void GenerateParticipantId(List<string> headerRows, List<List<string>> subjectColumns)
    {
        headerRows.AddRange(new List<string>
        {
            "participant_id", "redcap_event_name"
        });

        var participantIds = Enumerable.Range(1, SubjectsToGenerate).ToList();
        var eventNames = Enumerable.Repeat(EventName, SubjectsToGenerate).ToList();
        subjectColumns.AddRange(new List<List<string>>
        {
            participantIds.ConvertAll(id => id.ToString()),
            eventNames,
        });
    }

    /// <summary>
    /// Generates synthetic subject data.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="subjectData">Subject the synthetic data are added to.</param>
    /// <param name="fieldType">The type of field to be generated.</param>
    /// <param name="minValidation">The minimum value to be generated.</param>
    /// <param name="maxValidation">The maximum value to be generated.</param>
    private static void GenerateData(List<string> subjectData, string fieldType, string minValidation,
        string maxValidation)
    {
        // Map RedCap field types to generator classes
        var dataTypeMapping = new Dictionary<string, DataGenerator>
        {
            { "Date Box", new DateBoxGenerator() },
            { "text", new TextGenerator() },
            { "Number Box (Decimal)", new NumberGenerator() },
            { "select", new NumberGenerator() },
            { "notes", new TextGenerator() },
            { "Phone", new PhoneGenerator() },
            { "E-mail", new EmailGenerator() },
            { "radio", new NumberGenerator() },
            { "yesno", new NumberGenerator() },
            { "slider", new NumberGenerator() },
            { "checkbox", new NumberGenerator() },
        };

        // If we can't handle the data type it is skipped
        if (!dataTypeMapping.TryGetValue(fieldType, out var generator)) return;
        var generatedData = generator.GenerateData(minValidation, maxValidation);
        subjectData.Add(generatedData);
    }

    
    /// <summary>
    /// Generate the header column.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="fieldName">Name of the field to append.</param>
    private static void GenerateFieldHeader(List<string> headerRows, string fieldName)
    {
        // Skip calculated fields.
        if (fieldName == "calc") return;
        headerRows.Add(fieldName);
    }

    /// <summary>
    /// Generate the header column when field is a checkbox.
    /// </summary>
    /// <remarks>
    /// For a checkbox multiple header columns are produced from one row.
    /// Combines the field and choice label, and append to each column row.
    /// </remarks>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="fieldName">Name of the field to append.</param>
    /// <param name="choices">The choices of the checkbox.</param>
    private static void GenerateCheckboxHeaders(List<string> headerRows, string fieldName, List<string> choices)
    {
        headerRows.AddRange(choices.Select(choice => fieldName + "___" + choice));
    }

    /// <summary>
    /// Handles if there has been a change in CRF, adding completion check columns.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="subjectColumns">List of subjects the columns are added to.</param>
    /// <param name="currentFormName">The current CRF name.</param>
    /// <param name="previousFormName">The previous CRF name.</param>
    private static void HandleCrfChange(List<string> headerRows, List<List<string>> subjectColumns,
        string currentFormName, string previousFormName)
    {
        if (currentFormName == previousFormName) return;
        if (string.IsNullOrEmpty(previousFormName)) return;
        headerRows.Add(previousFormName + "_complete");
        headerRows.Add(previousFormName + "_custom_label");

        subjectColumns.AddRange(new List<List<string>>
        {
            Enumerable.Repeat("1", SubjectsToGenerate).ToList(),
            Enumerable.Repeat("", SubjectsToGenerate).ToList(),
        });
    }

    /// <summary>
    /// Handles if its the last CRF, adding completion check columns.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="subjectColumns">List of subjects the columns are added to.</param>
    /// <param name="currentFormName">The current CRF name.</param>
    /// <param name="previousFormName">The previous CRF name.</param>
    private static void HandleLastCrf(List<string> headerRows, List<List<string>> subjectColumns,
        string currentFormName, string previousFormName)
    {
        if (string.IsNullOrEmpty(currentFormName)) return;
        headerRows.Add(currentFormName + "_complete");
        headerRows.Add(previousFormName + "_custom_label");

        subjectColumns.AddRange(new List<List<string>>
        {
            Enumerable.Repeat("1", SubjectsToGenerate).ToList(),
            Enumerable.Repeat("", SubjectsToGenerate).ToList(),
        });
    }

    /// <summary>
    /// Write the data to the spreadsheet.
    /// </summary>
    /// <param name="headerRow">List of header rows to write.</param>
    /// <param name="subjectColumns">List of columns of subject data to write.</param>
    /// <param name="export">The worksheet to export with.</param>
    private static void WriteCsv(List<string> headerRow, List<List<string>> subjectColumns, string exportFilePath)
    {
        // Write headers
        for (var i = 0; i < headerRow.Count; i++)
        {
            export.Cells[1, i + 1].Value = headerRow[i];
        }

        // Write subject data
        for (var i = 0; i < subjectColumns.Count; i++)
        {
            for (var j = 0; j < subjectColumns[i].Count; j++)
            {
                export.Cells[j + 2, i + 1].Value = subjectColumns[i][j];
            }
        }

        // Write to .csv
        var output = new FileInfo(exportFilePath);
        var outputFormat = new ExcelOutputTextFormat();
        export.Cells[1, 1, SubjectsToGenerate, headerRow.Count].SaveToText(output, outputFormat);
    }
    
}
