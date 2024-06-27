using Accord;
using Accord.IO;
using Lab01.Models;
using MathNet.Numerics.Statistics;
using OxyPlot;
using System.Data;
using System.Diagnostics.Metrics;
using System.Formats.Asn1;
using System.Globalization;

namespace Lab01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Загрузка данных из CSV файла или другого источника
            DataTable dataTable = new CsvReader("resources/winequality-red.csv", true).ToTable();
            // LoadDataFromCSV("resources/winequality-red.csv");

            Explore.PrintCommonInfo(dataTable);

            // Основные характеристики датасета
            Explore.ComputeBasicStatistics(dataTable);

            // Визуальное исследование датасета
            //VisualizeData(dataTable);
            Vizual.Scatter(dataTable, 5, 6, 11);
            //for (int i = 0; i < dataTable.Columns.Count - 1; i++)
            //{
            //    Vizual.ScatterTarget(dataTable, i);
            //}
            //Vizual.Hist(dataTable, 11, 5);

            // Информация о корреляции признаков
            ComputeCorrelation(dataTable);
        }

        static DataTable LoadDataFromCSV(string filePath)
        {
            DataTable dataTable = new DataTable();
            try
            {
                // Чтение данных из CSV файла
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string[] headers = sr.ReadLine().Split(','); // Предполагается, что заголовки в первой строке
                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header.Trim()); // Добавление столбцов на основе заголовков
                    }

                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(','); // Предполагается, что данные разделены запятыми
                        DataRow dataRow = dataTable.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dataRow[i] = rows[i].Trim(); // Заполнение строк данными
                        }
                        dataTable.Rows.Add(dataRow); // Добавление строк в DataTable
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при чтении файла: " + ex.Message);
            }

            return dataTable;
        }

        static void ComputeCorrelation(DataTable dataTable)
        {
            // Вычисление корреляционной матрицы
            var correlationMatrix = new double[dataTable.Columns.Count, dataTable.Columns.Count];
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    var values1 = dataTable.AsEnumerable().Select(row => Convert.ToDouble(row[i].ToString().Replace('.', ',')));
                    var values2 = dataTable.AsEnumerable().Select(row => Convert.ToDouble(row[j].ToString().Replace('.', ',')));
                    correlationMatrix[i, j] = Correlation.Pearson(values1, values2);
                }
            }

            // Вывод тепловой карты корреляции
            Console.WriteLine("Correlation Matrix:");
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    Console.Write(correlationMatrix[i, j].ToString("0.000") + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}
