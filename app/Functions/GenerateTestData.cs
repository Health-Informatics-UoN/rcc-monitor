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
        
        // Extract the first column data from the 3rd row onwards
        var columnData = import.Cells[3, 1, import.Dimension.End.Row, 1]
            .Select(cell => cell.Text)
            .ToList();
        
        // Generate header row
        var headerRows = new List<string>();
        
        // Iterate through rows (start from the third row)
        for (var rowIndex = 3; rowIndex <= import.Dimension.End.Row; rowIndex++)
        {
            var cellF = import.Cells[rowIndex, 6].Text; // Column F
            var cellH = import.Cells[rowIndex, 8].Text; // Column H

            // Check if column F contains "checkbox" and column H has choices
            if (cellF == "checkbox" && !string.IsNullOrEmpty(cellH))
            {
                var choices = cellH.Split('|');

                // Generate header rows based on choices
                // for (var i = 0; i < choices.Length; i++)
                // {
                //     var header = import.Cells[rowIndex, 1].Text + "___" + i;
                //     headerRows.Add(header);
                // }
                foreach (var choice in choices)
                {
                    var header = import.Cells[rowIndex, 1].Text + "___" + choice.Trim();
                    headerRows.Add(header);
                }
            }
            else // Just a regular column to add
            {
                var header = import.Cells[rowIndex, 1].Text;
                headerRows.Add(header);
            }
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