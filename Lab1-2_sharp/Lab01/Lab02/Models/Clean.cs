using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab02.Models
{
    internal static class Clean
    {
        public static DataTable RemoveColumnsWithMissingValues(DataTable dataTable)
        {
            // Удаление столбцов с пустыми значениями
            var columnsToRemove = dataTable.Columns.Cast<DataColumn>()
                .Where(col => dataTable.AsEnumerable().Any(row => row.IsNull(col)))
                .ToList();

            foreach (var column in columnsToRemove)
            {
                dataTable.Columns.Remove(column);
            }

            return dataTable;
        }

        public static DataTable RemoveRowsWithMissingValues(DataTable dataTable)
        {
            // Удаление строк с пустыми значениями
            var rowsToRemove = dataTable.AsEnumerable()
                .Where(row => row.ItemArray.Any(field => field is DBNull || field.ToString() == ""))
                .ToList();

            foreach (var row in rowsToRemove)
            {
                dataTable.Rows.Remove(row);
            }

            return dataTable;
        }
    }
}
