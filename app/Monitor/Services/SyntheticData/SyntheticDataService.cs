using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using Monitor.Models.SyntheticData;

namespace Monitor.Services.SyntheticData;

public class SyntheticDataService
{
    private const int SubjectsToGenerate = 100;
    private readonly FieldMappings _fieldMappings;

    public SyntheticDataService(IOptions<FieldMappings> fieldMappings)
    {
        _fieldMappings = fieldMappings.Value;
    }

    /// <summary>
    /// Validate the file looks like a data dictionary.
    /// </summary>
    /// <param name="file">File to validate.</param>
    /// <exception cref="InvalidDataException">The spreadsheet failed to validate.</exception>
    public void Validate(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    
        if (!csv.Read() || !csv.ReadHeader())
        {
            throw new InvalidDataException("File is empty or doesn't have a header.");
        }

        var requiredColumns = new List<string>
        {
            "Variable / Field Name", "Field Type", "Form Name", "Choices, Calculations, OR Slider Labels",
            "Text Validation Min", "Text Validation Max"
        };

        var headerRecord = csv.HeaderRecord;
        foreach (var column in requiredColumns)
        {
            if (!(headerRecord ?? throw new InvalidDataException("There is no header row.")).Contains(column))
            {
                throw new InvalidDataException($"The column '{column}' is missing in the file.");
            }
        }
    }
    
    /// <summary>
    /// Generate synthetic data from a RedCap data dictionary
    /// </summary>
    /// <param name="file">RedCap data dictionary to generate data against.</param>
    /// <param name="eventName">RedCap event name to generate for.</param>
    /// <returns>A .csv bytes of Synthetic Data.</returns>
    /// <exception cref="InvalidDataException">Data failed to generate.</exception>
    public byte[] Generate(IFormFile file, string eventName)
    {
        try
        {
            using var stream = file.OpenReadStream();
            var rows = ReadCsv(stream);
            var syntheticData = GenerateRows(rows, eventName);
            
            return ExportCsv(syntheticData.headerRow, syntheticData.subjectColumns);
        }
        catch (Exception e)
        {
            throw new  InvalidDataException("Error generating data.", e);
        }
    }
    
    /// <summary>
    /// Read a csv file to a list of RedCap fields.
    /// </summary>
    /// <param name="stream">File stream.</param>
    /// <returns>List of RedCap fields.</returns>
    private static List<FieldRow> ReadCsv(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var records = new List<FieldRow>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            ShouldSkipRecord = (row) => string.IsNullOrEmpty(row.Row[0]) || row.Row[0] == "{}"
        };
        
        using var csv = new CsvReader(reader, config);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var record = new FieldRow(
                csv.GetField("Variable / Field Name") ?? string.Empty, 
                csv.GetField("Field Type") ?? string.Empty,
                csv.GetField("Form Name") ?? string.Empty,
                csv.GetField("Choices, Calculations, OR Slider Labels") ?? string.Empty,
                csv.GetField("Text Validation Min") ?? string.Empty,
                csv.GetField("Text Validation Max") ?? string.Empty,
                csv.GetField("Measurement Unit") ?? string.Empty
            );
            records.Add(record);
        }
        return records;
    }
    
    
    /// <summary>
    /// Generates rows of synthetic data based on the import file.
    /// </summary>
    /// <remarks>
    /// It works by generating a header row, and a "subject" column of data at a time. 
    /// </remarks>
    /// <param name="rows">List of fields to generate data against.</param>
    /// <param name="eventName">RedCap event name to generate for.</param>
    /// <returns>A list of synthetic data.</returns>
    private (List<string> headerRow, List<List<string>> subjectColumns) GenerateRows(List<FieldRow> rows, string eventName)
    {
        var headerRow = new List<string>();
        var subjectColumns = new List<List<string>>();

        GenerateParticipantId(headerRow, subjectColumns, eventName);

        // Keep track of the current and previous CRF to generate completion columns
        var currentCrfName = string.Empty;
        var previousCrfName = string.Empty;
        
        foreach (var row in rows)
        {
            var subjectData = new List<string>();

            // Add any completion columns if we're changing CRF
            currentCrfName = row.CrfName;
            HandleCrfChange(headerRow, subjectColumns, currentCrfName, previousCrfName);
            previousCrfName = currentCrfName;

            // For 
            if (!string.IsNullOrEmpty(row.Choices))
            {
                // Checkboxes generate multiple columns per field so handled differently
                if (row.FieldType == "checkbox")
                {
                    GenerateCheckboxHeaders(headerRow, row.FieldName, row.CleanedChoices);
                    
                    // Generate a column for each checkbox choice
                    foreach (var _ in row.CleanedChoices.Select(_ => new List<string>()))
                    {
                        for (var i = 0; i < SubjectsToGenerate; i++)
                        {
                            GenerateData(subjectData, "yesno", "0", "1");
                        }
                        
                        // Make a copy so we can get random values for each column
                        subjectColumns.Add(new List<string>(subjectData));
                        subjectData.Clear();
                    }
                }
                else
                {
                    GenerateFieldHeader(headerRow, row.FieldName);
                    
                    for (var i = 0; i < SubjectsToGenerate; i++)
                    {
                        GenerateData(subjectData, row.CleanedChoices);
                    }
                    subjectColumns.Add(subjectData);
                }
            }
            else
            {
                GenerateFieldHeader(headerRow, row.FieldName);
                
                HandleCustomFields(row);
                
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
    /// <param name="eventName">RedCap event name to generate for.</param>
    public static void GenerateParticipantId(List<string> headerRows, List<List<string>> subjectColumns, string eventName)
    {
        headerRows.AddRange(new List<string>
        {
            "participant_id", "redcap_event_name"
        });

        var participantIds = Enumerable.Range(1, SubjectsToGenerate).ToList();
        var eventNames = Enumerable.Repeat(eventName, SubjectsToGenerate).ToList();
        subjectColumns.AddRange(new List<List<string>>
        {
            participantIds.ConvertAll(id => id.ToString()),
            eventNames,
        });
    }

    
    /// <summary>
    /// Generates synthetic subject data.
    /// </summary>
    /// <param name="subjectData">Subject the synthetic data are added to.</param>
    /// <param name="fieldType">Type of field to generate.</param>
    /// <param name="minValidation">Minimum value.</param>
    /// <param name="maxValidation">Maximum value.</param>
    public static void GenerateData(List<string> subjectData, string fieldType, string minValidation, string maxValidation)
    {
        // Map RedCap field types to generator classes
        var dataTypeMapping = new Dictionary<string, DataGenerator>
        {
            { "Date Box", new DateBoxGenerator() },
            { "text", new TextGenerator() },
            { "Number Box (Decimal)", new DecimalGenerator() },
            { "Number Box (Integer)", new IntegerGenerator() },
            { "notes", new TextGenerator() },
            { "Phone", new PhoneGenerator() },
            { "E-mail", new EmailGenerator() },
            { "yesno", new IntegerGenerator() },
            { "slider", new IntegerGenerator() },
        };
        
        // If we can't handle the data type it is skipped
        if (!dataTypeMapping.TryGetValue(fieldType, out var generator)) return;

        var generatedData = generator.GenerateData(minValidation, maxValidation);
        subjectData.Add(generatedData);
        
    }

    /// <summary>
    /// Generates synthetic subject data.
    /// </summary>
    /// <param name="subjectData">Subject the synthetic data are added to.</param>
    /// <param name="choices">List of choices to choose from.</param>
    private static void GenerateData(List<string> subjectData, List<string> choices)
    {
        var generator = new ChoicesGenerator();
        var generatedData = generator.GenerateData(choices);
        subjectData.Add(generatedData);
    }
    
    /// <summary>
    /// Generate the header column.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="fieldName">Name of the field to append.</param>
    public static void GenerateFieldHeader(List<string> headerRows, string fieldName)
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
    public static void GenerateCheckboxHeaders(List<string> headerRows, string fieldName, List<string> choices)
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
    public static void HandleCrfChange(List<string> headerRows, List<List<string>> subjectColumns,
        string currentFormName, string previousFormName)
    {
        if (currentFormName == previousFormName) return;
        if (string.IsNullOrEmpty(previousFormName)) return;
        headerRows.Add(previousFormName + "_complete");
        headerRows.Add(previousFormName + "_custom_label");

        subjectColumns.AddRange(new List<List<string>>
        {
            Enumerable.Repeat("Completed", SubjectsToGenerate).ToList(),
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
    public static void HandleLastCrf(List<string> headerRows, List<List<string>> subjectColumns,
        string currentFormName, string previousFormName)
    {
        if (string.IsNullOrEmpty(currentFormName)) return;
        headerRows.Add(currentFormName + "_complete");
        headerRows.Add(previousFormName + "_custom_label");

        subjectColumns.AddRange(new List<List<string>>
        {
            Enumerable.Repeat("Completed", SubjectsToGenerate).ToList(),
            Enumerable.Repeat("", SubjectsToGenerate).ToList(),
        });
    }

    /// <summary>
    /// Write the data to the spreadsheet.
    /// </summary>
    /// <param name="headerRow">List of header rows to write.</param>
    /// <param name="subjectColumns">List of columns of subject data to write.</param>
    private static byte[] ExportCsv(List<string> headerRow, List<List<string>> subjectColumns)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
     
        // Write header row
        writer.WriteLine(string.Join(",", headerRow));

        // Transpose subject columns
        var numRows = subjectColumns.Max(col => col.Count);
        for (var row = 0; row < numRows; row++)
        {
            var rowData = new List<string>();
            foreach (var column in subjectColumns)
            {
                rowData.Add(row < column.Count ? $"\"{column[row]}\"" : "");
            }

            writer.WriteLine(string.Join(",", rowData));
            writer.Flush();
        }
        
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Handles setting sensible defaults ranges for custom fields respecting their measurement units.
    /// </summary>
    /// <remarks>
    /// If a field already has range validation set, we do not override it.
    /// </remarks>
    /// <param name="row">The row to change.</param>
    public void HandleCustomFields(FieldRow row)
    {
        // Match if contains field name & unit is equal, or not set.
        var matchingField = _fieldMappings.Mappings?.FirstOrDefault(mapping =>
            row.FieldName.ToLower().Contains(mapping.FieldName.ToLower())
            && (string.IsNullOrEmpty(mapping.MeasurementUnit) || string.Equals(row.MeasurementUnit,
                mapping.MeasurementUnit, StringComparison.CurrentCultureIgnoreCase)));
        
        if (matchingField == null) return;
        
        // Only set validations if there were none, we don't override them.
        if (string.IsNullOrEmpty(row.ValidationMin))
        {
            row.ValidationMin = matchingField.MinValue;
        }

        if (string.IsNullOrEmpty(row.ValidationMax))
        {
            row.ValidationMax = matchingField.MaxValue;
        }
    }

}
