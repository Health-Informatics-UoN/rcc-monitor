using OfficeOpenXml;

namespace Functions.Services.SyntheticData;

public class SyntheticDataService
{
    private const int SubjectsToGenerate = 100;
    private const string EventName = "test event";

    /// <summary>
    /// Generates synthetic data to a .csv file.
    /// </summary>
    public void Generate()
    {
        const string importFilePath = "import_dictionary.csv";
        const string exportFilePath = "export.csv";

        var package = CreateWorkbook(importFilePath);
        var worksheet = package.Workbook.Worksheets["import"];
        var export = package.Workbook.Worksheets["export"];
        var syntheticData = GenerateRows(worksheet);

        WriteToFile(syntheticData.headerRow, syntheticData.subjectColumns, export);
    }

    /// <summary>
    /// Generates rows of synthetic data based on the import file.
    /// </summary>
    /// <remarks>
    /// It works by generating a header row, and a column of data at a time. 
    /// </remarks>
    /// <param name="worksheet">Path to the .csv to import</param>
    /// <returns>A list of synthetic data.</returns>
    private static (List<string> headerRow, List<List<string>> subjectColumns) GenerateRows(ExcelWorksheet worksheet)
    {
        var headerRow = new List<string>();
        var subjectColumns = new List<List<string>>();

        GenerateParticipantId(headerRow, subjectColumns);

        // Keep track of the current and previous form names
        // So we can generate complete at the end of each CRF.
        var currentFormName = string.Empty;
        var previousFormName = string.Empty;

        // Skip the header row
        for (var rowIndex = 3; rowIndex <= worksheet.Dimension.End.Row; rowIndex++)
        {
            var subjectData = new List<string>();

            currentFormName = worksheet.Cells[rowIndex, 2].Text;
            HandleFormNameChange(headerRow, subjectColumns, currentFormName, previousFormName);
            previousFormName = currentFormName;

            var fieldName = worksheet.Cells[rowIndex, 1].Text;
            var fieldType = worksheet.Cells[rowIndex, 6].Text;
            var choices = worksheet.Cells[rowIndex, 8].Text;
            var minValidation = worksheet.Cells[rowIndex, 11].Text;
            var maxValidation = worksheet.Cells[rowIndex, 12].Text;

            var cleanedChoices = choices
                .Split('|')
                .Select(choice => choice.Split(',')[0].Trim('"').Trim())
                .ToList();

            // Checkboxes generate multiple columns per field
            if (fieldType == "checkbox" && !string.IsNullOrEmpty(choices))
            {
                GenerateCheckboxHeaders(headerRow, fieldName, cleanedChoices);
                // Add 0 or 1 for each checkbox choice
                var random = new Random();
                foreach (var randomValues in cleanedChoices.Select(_ => new List<string>()))
                {
                    for (var i = 0; i < SubjectsToGenerate; i++)
                    {
                        var randomValue = random.Next(2); // Generate either 0 or 1
                        randomValues.Add(randomValue.ToString());
                    }

                    subjectColumns.Add(randomValues);
                }
            }
            else
            {
                GenerateFieldHeader(headerRow, fieldName);

                // Fix max validation to choices if there are any.
                if (!string.IsNullOrEmpty(choices))
                {
                    maxValidation = choices.Length.ToString();
                }

                // Generate subjects
                for (var i = 0; i < SubjectsToGenerate; i++)
                {
                    GenerateData(subjectData, fieldType, minValidation, maxValidation);
                }

                subjectColumns.Add(subjectData);
            }
        }

        HandleLastForm(headerRow, subjectColumns, currentFormName, previousFormName);

        return (headerRow, subjectColumns);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="headerRows"></param>
    /// <param name="subjectColumns"></param>
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
    /// <param name="subjectData"></param>
    /// <param name="fieldType"></param>
    /// <param name="minValidation"></param>
    /// <param name="maxValidation"></param>
    private static void GenerateData(List<string> subjectData, string fieldType, string minValidation,
        string maxValidation)
    {
        // Map data types to generator classes
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
        };

        if (dataTypeMapping.TryGetValue(fieldType, out var generator))
        {
            var generatedData = generator.GenerateData(minValidation, maxValidation);
            subjectData.Add(generatedData);
        }
        else
        {
            // Can't handle the datatype so we don't enter it.
        }
    }

    /// <summary>
    /// Opens the .csv into a worksheet
    /// </summary>
    /// <param name="importFilePath">Path to the .csv</param>
    /// <returns>The worksheet with data loaded.</returns>
    private static ExcelPackage CreateWorkbook(string importFilePath)
    {
        var pck = new ExcelPackage();
        var import = pck.Workbook.Worksheets.Add("import");
        pck.Workbook.Worksheets.Add("export");

        var file = new FileInfo(importFilePath);
        var format = new ExcelTextFormat
        {
            TextQualifier = '"' // Text is wrapped in quotations in data dictionary
        };
        import.Cells["A1"].LoadFromText(file, format);
        return pck;
    }

    /// <summary>
    /// Generate the header column.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="fieldName">Name of the field to append</param>
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
    /// For a checkbox multiple header columns a re produced from one row.
    /// They take field, and choice label and append to each column row.
    /// </remarks>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="fieldName">Name of the field to append.</param>
    /// <param name="choices">The choices of the checkbox.</param>
    private static void GenerateCheckboxHeaders(List<string> headerRows, string fieldName, List<string> choices)
    {
        headerRows.AddRange(choices.Select(choice => fieldName + "___" + choice));
    }

    /// <summary>
    /// Handles if there has been a change in form name, adding completion check columns.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="currentFormName">The current form name.</param>
    /// <param name="previousFormName">The previous form name.</param>
    private static void HandleFormNameChange(List<string> headerRows, List<List<string>> subjectColumns,
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
    /// <param name="currentFormName">The current form name.</param>
    /// <param name="previousFormName">The previous form name.</param>
    private static void HandleLastForm(List<string> headerRows, List<List<string>> subjectColumns,
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
    /// <param name="headerRow"></param>
    /// <param name="subjectColumns"></param>
    /// <param name="export"></param>
    private static void WriteToFile(List<string> headerRow, List<List<string>> subjectColumns, ExcelWorksheet export)
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
        var output = new FileInfo("export.csv");
        var outputFormat = new ExcelOutputTextFormat();
        export.Cells[1, 1, SubjectsToGenerate, headerRow.Count].SaveToText(output, outputFormat);
    }
}