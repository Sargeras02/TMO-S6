using MathNet.Numerics.Statistics;
using System.Data;

namespace Lab01.Models
{
    internal static class Explore
    {
        public static int CountNullValues(DataTable dataTable, string columnName)
        {
            int nullCount = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                if (row.IsNull(columnName))
                {
                    nullCount++;
                }
            }
            return nullCount;
        }

        public static void PrintCommonInfo(DataTable dataTable)
        {
            // .shape
            Console.WriteLine($"Shape: ({dataTable.Rows.Count}, {dataTable.Columns.Count})");

            // .columns
            Console.WriteLine("Columns of DataTable: ");
            foreach (DataColumn column in dataTable.Columns)
            {
                Console.Write($"\"{column.ColumnName}\", ");
            }
            Console.WriteLine();

            // .dtypes
            Console.WriteLine("Column types of DataTable: ");
            foreach (DataColumn column in dataTable.Columns)
            {
                Console.WriteLine($"{column.DataType}");
            }

            // .isnull
            Console.WriteLine("Количество пустых значений в DataTable:");
            foreach (DataColumn column in dataTable.Columns)
            {
                int nullCount = CountNullValues(dataTable, column.ColumnName);
                Console.WriteLine($"{column.ColumnName} - {nullCount} / {dataTable.Rows.Count}");
            }
            Console.WriteLine();

            Console.WriteLine();
        }

        public static void ComputeBasicStatistics(DataTable dataTable)
        {
            // Вывод основных статистических характеристик для каждого столбца
            foreach (DataColumn column in dataTable.Columns)
            {
                Console.WriteLine($"Column: {column.ColumnName} ({column.DataType})");
                if (column.DataType == typeof(double) || column.DataType == typeof(int))
                {
                    var values = dataTable.AsEnumerable().Select(row =>
                    {
                        var temp = row[column].ToString().Replace('.', ',');
                        if (double.TryParse(temp, out double value))
                        {
                            return value;
                        }
                        else
                        {
                            // Обработка ошибки, например, замена некорректных значений на NaN
                            return double.NaN;
                        }
                    });
                    Console.WriteLine($"Mean: {values.Mean():0.00}, Median: {values.Median():0.00}, StdDev: {values.StandardDeviation():0.00}");
                    Console.WriteLine($"Min: {values.Min():0.00}, Max: {values.Max():0.00}");
                    var notnull = values.Where(x => !double.IsNaN(x));
                    if (notnull.Any())
                        Console.WriteLine($"Example: {notnull.ElementAt(Random.Shared.Next(notnull.Count()))}");
                    Console.WriteLine();
                }
                else
                {
                    var values = dataTable.AsEnumerable().Select(row => row[column].ToString());

                    var notnull = values.Where(x => !string.IsNullOrEmpty(x));
                    if (notnull.Any())
                        Console.WriteLine($"Example: {notnull.ElementAt(Random.Shared.Next(notnull.Count()))}");
                    Console.WriteLine();
                }
            }
        }
    }
}
