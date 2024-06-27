using Accord.IO;
using Lab01.Models;
using Lab02.Models;
using System.Data;

namespace Lab02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ML.Run();
        }

        static void Analysis()
        {
            DataTable dataTable = new CsvReader("resources/car_prices.csv", true).ToTable();
            Explore.PrintCommonInfo(dataTable);
            UpdateColumnDataTypes(dataTable, 0.9);

            Explore.PrintCommonInfo(dataTable);

            dataTable = Clean.RemoveRowsWithMissingValues(dataTable);

            Explore.PrintCommonInfo(dataTable);

            Explore.ComputeBasicStatistics(dataTable);
        }

        static void UpdateColumnDataTypes(DataTable dataTable, double limit)
        {
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var column = dataTable.Columns[i];

                int nullValue = 0;
                int intCount = 0;
                int doubleCount = 0;
                int dateTimeCount = 0;

                var temp = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    object value = row[column];
                    temp.Add(value.ToString());
                    if (value != DBNull.Value)
                    {
                        if (int.TryParse(value.ToString(), out int intValue))
                        {
                            intCount++;
                        }
                        if (double.TryParse(value.ToString().Replace('.', ','), out double doubleValue))
                        {
                            doubleCount++;
                        }
                        if (DateTime.TryParse(value.ToString(), out DateTime dateTimeValue))
                        {
                            dateTimeCount++;
                        }
                    }
                    else
                    {
                        nullValue++;
                    }
                }

                var lmt = limit * (dataTable.Rows.Count - nullValue);
                if (intCount > lmt)
                {
                    // Удаление старого столбца
                    dataTable.Columns.Remove(column.ColumnName);

                    var newColumn = new DataColumn(column.ColumnName, typeof(int));
                    newColumn.AllowDBNull = true;

                    // Добавление нового столбца в ту же позицию
                    dataTable.Columns.Add(newColumn);
                    newColumn.SetOrdinal(i);

                    int j = 0;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (int.TryParse(temp[j], out int value))
                            row[newColumn] = value;
                        else
                            row[newColumn] = DBNull.Value;
                        j++;
                    }
                }
                else if (doubleCount > lmt)
                {
                    // Удаление старого столбца
                    dataTable.Columns.Remove(column.ColumnName);

                    var newColumn = new DataColumn(column.ColumnName, typeof(double));
                    newColumn.AllowDBNull = true;

                    // Добавление нового столбца в ту же позицию
                    dataTable.Columns.Add(newColumn);
                    newColumn.SetOrdinal(i);

                    int j = 0;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (double.TryParse(temp[j], out double value))
                            row[newColumn] = value;
                        else
                            row[newColumn] = DBNull.Value;
                        j++;
                    }
                }
                else if (dateTimeCount > lmt)
                {
                    // Удаление старого столбца
                    dataTable.Columns.Remove(column.ColumnName);

                    var newColumn = new DataColumn(column.ColumnName, typeof(DateTime));
                    newColumn.AllowDBNull = true;

                    // Добавление нового столбца в ту же позицию
                    dataTable.Columns.Add(newColumn);
                    newColumn.SetOrdinal(i);

                    int j = 0;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (DateTime.TryParse(temp[j], out DateTime value))
                            row[newColumn] = value;
                        else
                            row[newColumn] = DBNull.Value;
                        j++;
                    }
                }
            }

            static void ReplaceColumn(DataTable table, string columnName, Type newType)
            {
                // Создание нового столбца с новым типом данных
                DataColumn newColumn = new DataColumn(columnName, newType);
                newColumn.AllowDBNull = true;

                var index = table.Columns.IndexOf(columnName);
                // Удаление старого столбца
                table.Columns.Remove(columnName);

                // Добавление нового столбца в ту же позицию
                table.Columns.Add(newColumn);
                newColumn.SetOrdinal(index);
            }
        }
    }
}
