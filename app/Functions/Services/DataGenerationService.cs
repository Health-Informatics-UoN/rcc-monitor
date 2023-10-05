using OfficeOpenXml;

namespace Functions.Services;

public class DataGenerationService
{
    public void GenerateData()
    {
        const string importFilePath = "import_dictionary.csv"; 
        const string exportFilePath = "export.csv"; 

        var syntheticData = GenerateHeaderRows(importFilePath);
        
        WriteRowsToFile(syntheticData, exportFilePath);
    }
    
    private static List<string> GenerateHeaderRows(string importFilePath)
    {
        using var pck = new ExcelPackage();
        var worksheet = pck.Workbook.Worksheets.Add("sheet");
        
        var file = new FileInfo(importFilePath);
        var format = new ExcelTextFormat
        {
            TextQualifier = '"'
        };
        worksheet.Cells["A1"].LoadFromText(file, format);

        var headerRows = new List<string>();

        // Keep track of the current and previous form names
        // So we can generate form complete at the end of each form.
        var currentFormName = string.Empty;
        var previousFormName = string.Empty;
        
        for (var rowIndex = 3; rowIndex <= worksheet.Dimension.End.Row; rowIndex++)
        {
            currentFormName = worksheet.Cells[rowIndex, 2].Text;

            HandleFormNameChange(headerRows, currentFormName, previousFormName);
            previousFormName = currentFormName;

            var cellF = worksheet.Cells[rowIndex, 6].Text;
            var cellH = worksheet.Cells[rowIndex, 8].Text;

            if (cellF == "checkbox" && !string.IsNullOrEmpty(cellH))
            {
                ProcessCheckboxField(headerRows, worksheet.Cells[rowIndex, 1].Text, cellH);
            }
            else
            {
                ProcessRegularColumn(headerRows, worksheet.Cells[rowIndex, 1].Text);
            }
        }

        HandleLastForm(headerRows, currentFormName, previousFormName);

        return headerRows;
    }

    private static void HandleFormNameChange(List<string> headerRows, string currentFormName, string previousFormName)
    {
        if (currentFormName == previousFormName) return;
        if (string.IsNullOrEmpty(previousFormName)) return;
        headerRows.Add(previousFormName + "_complete");
        headerRows.Add(previousFormName + "_custom_label");
    }

    /// <summary>
    /// Generate the header row when field is a checkbox.
    /// </summary>
    /// <remarks>
    /// A checkbox is different, multiple headers are produced from one row.
    /// </remarks>
    /// <param name="headerRows"></param>
    /// <param name="fieldName"></param>
    /// <param name="cellH"></param>
    private static void ProcessCheckboxField(List<string> headerRows, string fieldName, string cellH)
    {
        var choices = cellH.Split('|');

        foreach (var choice in choices)
        {
            var label = choice.Split(',')[0].Trim('"');
            var header = fieldName + "___" + label.Trim();
            headerRows.Add(header);
        }
    }

    private static void ProcessRegularColumn(List<string> headerRows, string fieldName)
    {
        headerRows.Add(fieldName);
    }

    private static void HandleLastForm(List<string> headerRows, string currentFormName, string previousFormName)
    {
        if (string.IsNullOrEmpty(currentFormName)) return;
        headerRows.Add(currentFormName + "_complete");
        headerRows.Add(previousFormName + "_custom_label");
    }

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