using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace NS2_Discord_Bot.Services
{
    public class Charts
    {
        public static string SaveLineChart(string dataName, IList<string> dates, IList<double> dataPoints)
        {
            var properties = new
            {
                fonts = new
                {
                    TitleFont = new Font(FontFamily.GenericSansSerif, 20f, FontStyle.Underline | FontStyle.Italic),
                    LegendFont = new Font(FontFamily.GenericSansSerif, 12.5f),
                    AxisLabelFont = new Font(FontFamily.GenericSansSerif, 10f),
                    DataLabelFont = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Bold)
                },
                colors = new
                {
                    BackColorLight = System.Drawing.Color.FromArgb(55, 71, 79),
                    BackColorDark = System.Drawing.Color.FromArgb(38, 50, 56),
                    TextColor = System.Drawing.Color.FromArgb(238, 238, 238),
                    LineColor = System.Drawing.Color.FromArgb(100, 235, 235, 235),
                    GraphLineColor = Color.GetRandomPastelDrawingColor()
                }
            };

            var chartTitle = new Title($"{dataName} Chart")
            {
                Font = properties.fonts.TitleFont,
                ForeColor = properties.colors.TextColor,
                Alignment = ContentAlignment.TopCenter
            };

            var chart = new Chart
            {
                Width = 1920,
                Height = 1080,
                AntiAliasing = AntiAliasingStyles.All,
                BackColor = properties.colors.BackColorLight,
                Titles = { chartTitle }
            };

            var chartArea = new ChartArea("ChartArea")
            {
                BackColor = properties.colors.BackColorDark,
                AxisX =
                {
                    LabelStyle =
                    {
                        Font = properties.fonts.AxisLabelFont,
                        ForeColor = properties.colors.TextColor
                    },
                    MajorGrid =
                    {
                        LineColor = properties.colors.LineColor
                    }
                },
                AxisY =
                {
                    LabelStyle =
                    {
                        Font = properties.fonts.AxisLabelFont,
                        ForeColor = properties.colors.TextColor
                    },
                    MajorGrid =
                    {
                        LineColor = properties.colors.LineColor
                    },
                    Minimum = 200
                }
            };
            chart.ChartAreas.Add(chartArea);

            var legend = new Legend("ChartLegend")
            {
                BackColor = properties.colors.BackColorDark,
                ForeColor = properties.colors.TextColor,
                Font = properties.fonts.LegendFont
            };

            var series = new Series(dataName)
            {
                Color = properties.colors.GraphLineColor,
                ChartType = SeriesChartType.Line,
                Legend = "ChartLegend",
                ChartArea = "ChartArea",
                BorderWidth = 2,
                LabelForeColor = properties.colors.TextColor,
                Font = properties.fonts.DataLabelFont
            };
            series.Points.DataBindXY(dates, dataPoints);
            for (var i = 0; i < series.Points.Count; i++)
            {
                series.Points[i].Label = dataPoints[i].ToString(CultureInfo.CurrentCulture);
            }

            chart.Series.Add(series);
            chart.Legends.Add(legend);

            const string chartFileName = "chartImage.png";

            chart.DataBind();
            chart.SaveImage(@$"./{chartFileName}", ChartImageFormat.Png);

            var saveLocation = Path.GetFullPath($"./{chartFileName}");

            return saveLocation;
        }

        private static void PostToImgur(string imageFilePath)
        {
            
        }
    }
}
