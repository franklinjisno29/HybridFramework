using ExcelDataReader;
using Google.Test_Data_Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google.Utilities
{
    public class SearchDataRead
    {
        public static List<SearchData> ReadSearchText(string excelFilePath, string sheetName)
        {
            List<SearchData> searchDataList = new List<SearchData>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                        }
                    });

                    var dataTable = result.Tables[sheetName];

                    if (dataTable != null)
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {
                            SearchData searchData = new SearchData
                            {
                                SearchText = GetValueOrDefault(row, "searchText"),
                            };
                            searchDataList.Add(searchData);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Sheet '{sheetName}' not found in the Excel file.");
                    }
                }
            }
            return searchDataList;
        }

        static string? GetValueOrDefault(DataRow row, string columnName)
        {
            Console.WriteLine(row + "  " + columnName);
            return row.Table.Columns.Contains(columnName) ? row[columnName]?.ToString() : null;
        }
    }
}
