using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace Functions;

public static class GenerateTestData
{
    [Function("GenerateTestData")]
    public static void Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] MyInfo myTimer, FunctionContext context)
    {
        // TODO: Get this licensing working from config
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        // Open spreadsheet
        using var pck = new ExcelPackage();
        var import = pck.Workbook.Worksheets.Add("sheet");
        var export = pck.Workbook.Worksheets.Add("ExportedSheet");
        
        var file = new FileInfo("import_dictionary.csv");
        var format = new ExcelTextFormat
        {
            TextQualifier = '"'
        };
        import.Cells["A1"].LoadFromText(file, format);
        
        // Generate header row
        var headerRows = new List<string>();
        
        // Initialize variables to keep track of the current and previous form names
        var currentFormName = string.Empty;
        var previousFormName = string.Empty;
        
        // Iterate through rows (start from the third row)
        for (var rowIndex = 3; rowIndex <= import.Dimension.End.Row; rowIndex++)
        {
            // CRF Complete Guard 
            currentFormName = import.Cells[rowIndex, 2].Text; // Column 2 contains the Form Name
            if (currentFormName != previousFormName)
            {
                if (!string.IsNullOrEmpty(previousFormName))
                {
                    // Add a new header column for the completion status of the previous form
                    headerRows.Add(previousFormName + "_complete");
                    headerRows.Add(previousFormName + "_custom_label");
                }
            }
            previousFormName = currentFormName;

            // Checkbox guard
            var cellF = import.Cells[rowIndex, 6].Text; // Column F
            var cellH = import.Cells[rowIndex, 8].Text; // Column H

            // Check if column F contains "checkbox" and column H has choices
            if (cellF == "checkbox" && !string.IsNullOrEmpty(cellH))
            {
                var choices = cellH.Split('|');

                // Generate header rows based on choices
                foreach (var choice in choices)
                {
                    var label = choice.Split(',')[0].Trim('"');
                    var header = import.Cells[rowIndex, 1].Text + "___" + label.Trim();
                    headerRows.Add(header);
                }
            }
            else // Just a regular column to add
            {
                var header = import.Cells[rowIndex, 1].Text;
                headerRows.Add(header);
            }
        }
        
        // CRF Complete - last form guard?
        if (!string.IsNullOrEmpty(currentFormName))
        {
            headerRows.Add(currentFormName + "_complete");
            headerRows.Add(previousFormName + "_custom_label");
        }
        
        // Write the header rows to the new sheet
        for (var i = 0; i < headerRows.Count; i++)
        {
            export.Cells[1, i + 1].Value = headerRows[i];
        }
        

        // Loop rows
        
        // Strongly typed rows
        
        // Get field type
        // Get validation parameters
        // Map special fields
        // Generate test data
        
        // Save spreadsheet
        // the output file
        
        var output = new FileInfo(@"export.csv");
        // format with default parameters
        var outputformat = new ExcelOutputTextFormat();
        export.Cells[1, 1, 1, headerRows.Count].SaveToText(output, outputformat);
    }
}