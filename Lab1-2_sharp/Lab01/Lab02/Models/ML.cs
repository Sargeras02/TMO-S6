using Lab02.Models.Obj;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using static Microsoft.ML.Transforms.OneHotEncodingEstimator;
using static Microsoft.ML.Transforms.ValueToKeyMappingEstimator;

namespace Lab02.Models
{
    internal static class ML
    {
        public static void Run()
        {
            // Создаем контекст ML.NET
            var mlContext = new MLContext();

            // Загружаем данные из CSV файл
            var data = mlContext.Data.LoadFromTextFile<CarSaleData>("resources/car_low.csv", separatorChar: ',', hasHeader: true, trimWhitespace: true);

            // Превью первых нескольких строк данных
            var preview = data.Preview(1);
            Console.WriteLine(string.Join('\t', CarSaleData.GetHeaders()));
            foreach (var row in preview.RowView)
            {
                for (int i = 0; i < preview.ColumnView.Length; i++)
                {
                    var value = row.Values[i];
                    Console.Write($"{value.Value}\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            // Получаем общую информацию о датасете
            Console.WriteLine($" > Количество строк в датасете: {data.GetRowCount()}");
            Console.WriteLine($" > Количество колонок в датасете: {data.Schema.Count}");
            Console.WriteLine($" > Типы данных");

            foreach (var column in data.Schema)
            {
                Console.WriteLine($"{column.Name}: {column.Type}");
            }

            GetNull(mlContext, data);

            // Define replacement estimator
            var replacementEstimator = mlContext.Transforms.ReplaceMissingValues("Condition", replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean);
            var x = mlContext.Transforms.No.Categorical.OneHotEncoding("", "", OutputKind.Indicator, 100, KeyOrdinality.ByOccurrence);
            // Fit data to estimator
            // Fitting generates a transformer that applies the operations of defined by estimator
            ITransformer replacementTransformer = replacementEstimator.Fit(data);

            // Transform data
            IDataView transformedData = replacementTransformer.Transform(data);
            GetNull(mlContext, transformedData);

            // Определяем параметры кодирования категориальных признаков
            var options = new ValueToKeyMappingEstimator.Options()
            {
                // Указываем имя признака для кодирования
                OutputColumnName = "MakeEncoded",
                // Указываем имя признака для исходного категориального признака
                InputColumnName = "Make",
                // Задаем метод кодирования (OneHotEncoding)
                Kind = CategoricalEncodingEstimator.CategoricalEncoding.OneHot
            };

            // Применяем кодирование категориального признака
            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding(options);

            // Обучаем кодировщик на данных
            var transformer = pipeline.Fit(data);

            // Применяем преобразование к данным
            var transformedDataEncoded = transformer.Transform(data);
        }

        static void GetNull(MLContext mlContext, IDataView data)
        {
            Console.WriteLine($" > Количество пропущенных значений");

            // Преобразуем IDataView в IEnumerable<CarSaleData>, чтобы обработать данные
            var enumerableData = mlContext.Data.CreateEnumerable<CarSaleData>(data, reuseRowObject: false);

            // Подсчитываем пропущенные значения в каждой колонке
            var columnCounts = new Dictionary<string, int>();
            foreach (var item in enumerableData)
            {
                foreach (var prop in typeof(CarSaleData).GetProperties())
                {
                    var value = prop.GetValue(item);
                    var columnName = prop.Name;
                    if (!columnCounts.ContainsKey(columnName))
                    {
                        columnCounts[columnName] = 0;
                    }
                    if (value is null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    {
                        columnCounts[columnName]++;
                    }
                }
            }

            // Выводим результаты на консоль
            foreach (var kvp in columnCounts)
            {
                Console.WriteLine($"'{kvp.Key}': {kvp.Value}");
            }
        }

        static void GetNoNull(MLContext mlContext, IDataView data)
        {
            // Filter out any row with an NaN values in either column
            var filteredData = mlContext.Data
                .FilterRowsByMissingValues(data, [.. CarSaleData.GetHeaders()]);

            // Take a look at the resulting dataset and note that rows with NaNs are
            // filtered out. Only the second data point is left
            var enumerable = mlContext.Data
                .CreateEnumerable<CarSaleData>(filteredData, reuseRowObject: true);

            Console.WriteLine(string.Join('\t', CarSaleData.GetHeaders()));

            for (int i = 0; i < Math.Min(5, enumerable.Count()); i++)
            {
                Console.WriteLine($"{enumerable.ElementAt(i)})");
            }
        }
    }
}
