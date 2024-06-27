using Accord;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab01.Models
{
    internal static class Vizual
    {
        private static string Output { get; set; } = "resources/output";

        public static void Scatter(DataTable dataTable, int ca, int cb, int? hueIndex = null)
        {
            var plotModel = new PlotModel
            {
                Title = $"Scatter Plot {ca}-{dataTable.Columns[ca].ColumnName} {cb}-{dataTable.Columns[cb].ColumnName}",
                Background = OxyColor.FromRgb(255, 255, 255)
            };


            // Получение данных из столбцов DataTable
            double[] fixedAcidity = GetColumnData(dataTable, ca);
            double[] volatileAcidity = GetColumnData(dataTable, cb);

            // Создание серии точек для графика рассеяния
            var scatterSeries = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColors.Blue,
                MarkerStroke = OxyColors.Black,
            };

            for (int i = 0; i < fixedAcidity.Length; i++)
            {
                scatterSeries.Points.Add(new ScatterPoint(fixedAcidity[i], volatileAcidity[i]));
            }

            // Добавление серии точек к графику
            plotModel.Series.Add(scatterSeries);

            SavePlotToFile(plotModel);
        }

        public static void ScatterTarget(DataTable dataTable, int columnIndex) => Scatter(dataTable, columnIndex, dataTable.Columns.Count - 1);

        public static void Hist(DataTable dataTable, int column, int cnts = 10)
        {
            var plotModel = new PlotModel { Title = $"Histogram for {dataTable.Columns[column]}" };
            var data = GetColumnData(dataTable, column);
            var min = data.Min();
            var max = data.Max();
            var range = (max - min) / cnts;
            var hist = new List<dynamic>();
            for (int i = 0; i < cnts; i++)
            {
                var init = min + range * i;
                var end = min + range * (i + 1);
                hist.Add(new { Init = init, End = end, Count = data.Where(x => init <= x && x < end).Count() });
            }

            // Создание гистограммы
            var histogramSeries = new HistogramSeries
            {
                FillColor = OxyColors.Blue,
                StrokeColor = OxyColors.Black
            };
            foreach (var h in hist)
            {
                histogramSeries.Items.Add(new HistogramItem(h.Init, h.End, h.Count, h.Count));
            }

            plotModel.Series.Add(histogramSeries);

            //// Установка оси X
            //plotModel.Axes.Add(new CategoryAxis
            //{
            //    Position = AxisPosition.Bottom,
            //    ItemsSource = Enumerable.Range(1, data.Length).Select(i => i.ToString()),
            //    LabelFormatter = value => (Convert.ToInt32(value) - 1).ToString()
            //});

            //// Установка оси Y
            //plotModel.Axes.Add(new LinearAxis
            //{
            //    Position = AxisPosition.Left,
            //    MinimumPadding = 0,
            //    AbsoluteMinimum = 0
            //});

            SavePlotToFile(plotModel);
        }

        static double[] GetColumnData(DataTable dataTable, int columnIndex)
        {
            var columnData = new double[dataTable.Rows.Count];
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                columnData[i] = Convert.ToDouble(dataTable.Rows[i][columnIndex].ToString().Replace('.', ','));
            }
            return columnData;
        }

        static void SavePlotToFile(PlotModel plotModel)
        {
            var filePath = Path.Combine(Output, $"{plotModel.Title.Replace(' ', '_')}.png");

            var pngExporter = new PngExporter { Width = 1200, Height = 900 };
            pngExporter.ExportToFile(plotModel, filePath);
        }
    }
}
