using OfficeOpenXml;

namespace Functions.Services.SyntheticData;

public class SyntheticDataService
{
    /// <summary>
    /// Generates synthetic data to a .csv file.
    /// </summary>
    public void Generate()
    {
        const string importFilePath = "import_dictionary.csv"; 
        const string exportFilePath = "export.csv"; 

        var syntheticData = GenerateRows(importFilePath);
        
        WriteRowsToFile(syntheticData, exportFilePath);
    }
    
    /// <summary>
    /// Generates rows of synthetic data based on the import file.
    /// </summary>
    /// <param name="importFilePath">Path to the .csv to import</param>
    /// <returns>A list of synthetic data.</returns>
    private static List<string> GenerateRows(string importFilePath)
    {
        var worksheet = OpenWorksheet(importFilePath);

        var headerRow = new List<string>
        {
            "participant_id",
            "redcap_event_name"
        };
        
        var subjectRows = new List<List<string>>();

        // Keep track of the current and previous form names
        // So we can generate form complete at the end of each form.
        var currentFormName = string.Empty;
        var previousFormName = string.Empty;
        
        for (var rowIndex = 3; rowIndex <= worksheet.Dimension.End.Row; rowIndex++)
        {
            var subjectData = new List<string>();
            
            currentFormName = worksheet.Cells[rowIndex, 2].Text;

            HandleFormNameChange(headerRow, currentFormName, previousFormName);
            previousFormName = currentFormName;

            var fieldType = worksheet.Cells[rowIndex, 6].Text;
            var choices = worksheet.Cells[rowIndex, 8].Text;

            if (fieldType == "checkbox" && !string.IsNullOrEmpty(choices))
            {
                ProcessCheckboxField(headerRow, worksheet.Cells[rowIndex, 1].Text, choices);
                // TODO: generate checkbox data.
            }
            else
            {
                ProcessRegularColumn(headerRow, worksheet.Cells[rowIndex, 1].Text);

                var row = worksheet.Cells[rowIndex, 1, rowIndex, worksheet.Dimension.End.Column];
                
                GenerateData(row, subjectData);
                subjectRows.Add(subjectData);
            }
        }

        HandleLastForm(headerRow, currentFormName, previousFormName);

        return headerRow;
    }

    private static ExcelWorksheet OpenWorksheet(string importFilePath)
    {
        using var pck = new ExcelPackage();
        var worksheet = pck.Workbook.Worksheets.Add("sheet");

        var file = new FileInfo(importFilePath);
        var format = new ExcelTextFormat
        {
            TextQualifier = '"'
        };
        worksheet.Cells["A1"].LoadFromText(file, format);
        return worksheet;
    }

    /// <summary>
    /// Generates synthetic subject data for a row.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="subjectData"></param>
    private static void GenerateData(ExcelRange row, List<string> subjectData)
    {
        // Map data types to generator classes
        var dataTypeMapping = new Dictionary<string, DataGenerator>
        {
            { "Date Box", new DateBoxGenerator() },
            { "text", new TextGenerator() },
            { "Number Box (Decimal)", new NumberGenerator() },
            { "select", new TextGenerator() },
            { "file", new TextGenerator() },
            { "notes", new TextGenerator() },
            { "Phone", new TextGenerator() },
            { "E-mail", new TextGenerator() },
            { "radio", new TextGenerator() },
            { "yesno", new TextGenerator() },
            { "slider", new TextGenerator() },
            { "text", new TextGenerator() },
            { "text", new TextGenerator() },
        };
        
        // Get the relevant cells (the name, type, min max validation)
        var label = row[1, 1].Text;
        var fieldType = row[1, 6].Text;
        var minValidation = row[1, 11].Text;
        var maxValidation = row[1, 12].Text;
        
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
    /// Handles the change in form name and adds completion and custom label columns.
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
    /// <param name="cellH">The cell of the choices of the checkbox.</param>
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
        headerRows.Add(fieldName);
    }

    /// <summary>
    /// Handles the last form name and adds completion and custom label columns.
    /// </summary>
    /// <param name="headerRows">List of header rows the columns are added to.</param>
    /// <param name="currentFormName">The current form name.</param>
    /// <param name="previousFormName">The previous form name.</param>
    private static void HandleLastForm(List<string> headerRows, string currentFormName, string previousFormName)
    {
        if (string.IsNullOrEmpty(currentFormName)) return;
        headerRows.Add(currentFormName + "_complete");
        headerRows.Add(previousFormName + "_custom_label");
    }

    /// <summary>
    /// Write the rows to the spreadsheet.
    /// </summary>
    /// <param name="headerRows"></param>
    /// <param name="exportFilePath"></param>
    private static void WriteRowsToFile(List<string> headerRows, string exportFilePath)
    {
        // Write to a worksheet
        using var package = new ExcelPackage();
        var export = package.Workbook.Worksheets.Add("export");
        
        for (var i = 0; i < headerRows.Count; i++)
        {
            export.Cells[1, i + 1].Value = headerRows[i];
        }
        
        // Write to .csv
        var output = new FileInfo(exportFilePath);
        var outputFormat = new ExcelOutputTextFormat();
        export.Cells[1, 1, 1, headerRows.Count].SaveToText(output, outputFormat);
        
    }
}