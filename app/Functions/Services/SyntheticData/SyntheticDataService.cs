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
        
        for (var rowIndex = 3; rowIndex <= worksheet.Dimension.End.Row; rowIndex++)
        {
            var subjectData = new List<string>();
            
            currentFormName = worksheet.Cells[rowIndex, 2].Text;
            HandleFormNameChange(headerRow, currentFormName, previousFormName);
            previousFormName = currentFormName;

            var fieldName = worksheet.Cells[rowIndex, 1].Text;
            var fieldType = worksheet.Cells[rowIndex, 6].Text;
            var choices = worksheet.Cells[rowIndex, 8].Text;
            var minValidation = worksheet.Cells[rowIndex, 11].Text;
            var maxValidation = worksheet.Cells[rowIndex, 12].Text;

            if (fieldType == "checkbox" && !string.IsNullOrEmpty(choices))
            {
                ProcessCheckboxField(headerRow, fieldName, choices);
                // TODO: generate checkbox data.
            }
            else
            {
                ProcessRegularColumn(headerRow, fieldName);
                
                // TODO: Check if choices is not null, and then change min/max validation to their lengths.
                
                // Generate subjects
                for (var i = 0; i < SubjectsToGenerate; i++)
                {
                    GenerateData(subjectData, fieldType, minValidation, maxValidation);
                }
                subjectColumns.Add(subjectData);
            }
        }

        HandleLastForm(headerRow, currentFormName, previousFormName);

        return (headerRow, subjectColumns);
    }

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
    private static void GenerateData(List<string> subjectData, string fieldType, string minValidation, string maxValidation)
    {
        // Map data types to generator classes
        var dataTypeMapping = new Dictionary<string, DataGenerator>
        {
            { "Date Box", new DateBoxGenerator() },
            { "text", new TextGenerator() },
            { "Number Box (Decimal)", new NumberGenerator() },
            { "select", new NumberGenerator() },
            { "notes", new TextGenerator() },
            { "Phone", new NumberGenerator() },
            { "E-mail", new TextGenerator() },
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
            TextQualifier = '"'
        };
        import.Cells["A1"].LoadFromText(file, format);
        return pck;
    }
    
    /// <summary>
    /// Generate the header column when field is a checkbox.
    /// </summary>
    /// <remarks>
    /// For a checkbox multiple header columns are produced from one row.
    /// They take field, and choice label and append to each column row.
    /// </remarks>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="fieldName">Name of the field to append.</param>
    /// <param name="choices">The choices of the checkbox.</param>
    private static void ProcessCheckboxField(List<string> headerRows, string fieldName, string choices)
    {
        foreach (var choice in choices.Split('|'))
        {
            var label = choice.Split(',')[0].Trim('"');
            var header = fieldName + "___" + label.Trim();
            headerRows.Add(header);
        }
    }

    /// <summary>
    /// Generate the header column.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="fieldName">Name of the field to append</param>
    private static void ProcessRegularColumn(List<string> headerRows, string fieldName)
    {
        // Skip calculated fields.
        if (fieldName == "calc") return;
        headerRows.Add(fieldName);
    }
    
    /// <summary>
    /// Handles if there has been a change in form name, adding completion check columns.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="currentFormName">The current form name.</param>
    /// <param name="previousFormName">The previous form name.</param>
    private static void HandleFormNameChange(List<string> headerRows, string currentFormName, string previousFormName)
    {
        if (currentFormName == previousFormName) return;
        if (string.IsNullOrEmpty(previousFormName)) return;
        headerRows.Add(previousFormName + "_complete");
        headerRows.Add(previousFormName + "_custom_label");
        // TODO: We probably need to add subject data for these columns here.
        // You'll actually want to pass subjectColumns - as we're adding a missing column. 
    }

    /// <summary>
    /// Handles if its the last CRF, adding completion check columns.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="currentFormName">The current form name.</param>
    /// <param name="previousFormName">The previous form name.</param>
    private static void HandleLastForm(List<string> headerRows, string currentFormName, string previousFormName)
    {
        if (string.IsNullOrEmpty(currentFormName)) return;
        headerRows.Add(currentFormName + "_complete");
        headerRows.Add(previousFormName + "_custom_label");
        // TODO: Also probably need to do it here.
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