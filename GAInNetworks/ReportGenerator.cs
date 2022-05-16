using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClosedXML.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace GAInNetworks
{
    public class Report {
        public string ReportName { get; set; }
        public List<ReportData> Data { get; set; } = new List<ReportData>();
    }

    public class ReportData { 
        public int Iteration { get; set; }
        public double TimeOfIteration { get; set; }
        public double MinFitness { get; set; }
        public double MaxFitness { get; set; }
        public double AvgOfFitness { get; set; }
    }


    public static class ReportGenerator
    {
        public static void GenerateRaport(Report report)
        {
            IXLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add($"{report.ReportName} Sheet");
            DataTable table = new DataTable();
            table.Columns.Add("Iteration", typeof(int));
            table.Columns.Add("TimeOfIteration", typeof(double));
            table.Columns.Add("MinFitness", typeof(double));
            table.Columns.Add("MaxFitness", typeof(double));
            table.Columns.Add("AvgOfFitness", typeof(double));


            for(int i = 0; i < report.Data.Count; i++)
            {
                table.Rows.Add(report.Data[i].Iteration, report.Data[i].TimeOfIteration, report.Data[i].MinFitness, report.Data[i].MaxFitness, report.Data[i].AvgOfFitness);
            }

            worksheet.Cell(1, 1).Value = $"{report.ReportName}";
            worksheet.Range(1, 1, 1, 5).Merge().AddToNamed("Titles");
            worksheet.Cell(2, 1).InsertTable(table.AsEnumerable());

            var titlesStyle = workbook.Style;
            titlesStyle.Font.Bold = true;
            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titlesStyle.Fill.BackgroundColor = XLColor.Cyan;

            // Format all titles in one shot
            workbook.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs($"{report.ReportName}.xlsx");
            CreateChart(report);
            string json = JsonSerializer.Serialize(report);
            File.WriteAllText(@$"C:\Users\Maciej\source\repos\GAInNetworks\{report.ReportName}.json", json);


        }

        private static void CreateChart(Report report)
        {
            FileInfo fi = new FileInfo(@$"C:\Users\Maciej\source\repos\GAInNetworks\GAInNetworks\bin\Debug\netcoreapp3.0\{report.ReportName}.xlsx");
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //Get a WorkSheet by index. Note that EPPlus indexes are base 1, not base 0!
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[$"{report.ReportName} Sheet"];

                ExcelLineChart lineChart = worksheet.Drawings.AddChart("lineChart", eChartType.Line) as ExcelLineChart;
                

                //set the title
                lineChart.Title.Text = "Fitness Behaviour Chart";

                //create the ranges for the chart
                var rangeLabel = worksheet.Cells["C2:E2"];
                var range1 = worksheet.Cells["C3:C302"];
                var range2 = worksheet.Cells["D3:D302"];
                var range3 = worksheet.Cells["E3:E302"];
                var rangeX = worksheet.Cells["A3:A302"];

                lineChart.Series.Add(range1, rangeX);
                lineChart.Series.Add(range2, rangeX);
                lineChart.Series.Add(range3, rangeX);


                //set the names of the legend
                lineChart.Series[0].Header = worksheet.Cells["C2"].Value.ToString();
                lineChart.Series[1].Header = worksheet.Cells["D2"].Value.ToString();
                lineChart.Series[2].Header = worksheet.Cells["E2"].Value.ToString();

                //position of the legend
                lineChart.Legend.Position = eLegendPosition.Right;
                

                //size of the chart
                lineChart.SetSize(900, 500);

                //add the chart at cell B6
                lineChart.SetPosition(5, 0, 8, 0);
                excelPackage.Save();
            }
        }

        public static void PrintInitialPopulation(List<Individual> population)
        {
            IXLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add($"Initial Population Sheet");
            DataTable table = new DataTable();
            
            table.Columns.Add("Nr", typeof(int));
            table.Columns.Add("Path", typeof(string));
            table.Columns.Add("Fitness", typeof(double));

            population.Select((x, i) => new
            {
                item = x,
                index = i
            })
            .ToList().ForEach( data =>
            {
                var path = "";
                data.item.Genes.ForEach(x => path += x + " -> ");
                table.Rows.Add(data.index, path, data.item.Fitness);
            });

            worksheet.Cell(1, 1).Value = "Initial Population Sheet";
            worksheet.Range(1, 1, 1, 3).Merge().AddToNamed("Titles");
            worksheet.Cell(2, 1).InsertTable(table.AsEnumerable());

            var titlesStyle = workbook.Style;
            titlesStyle.Font.Bold = true;
            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titlesStyle.Fill.BackgroundColor = XLColor.Cyan;
            workbook.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs("Initial Population Sheet.xlsx");

        }

        public static void PrintNetworkMatrix(int[,] matrix)
        {
            IXLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add($"Network Matrix");
            DataTable table = new DataTable();

            table.Columns.Add("Nr");
            for (int i=0;i<20;i++)
            {
                table.Columns.Add($"{i + 1}", typeof(int));
            }

            for (int i = 0; i < 20; i++)
            {
                table.Rows.Add(i ,matrix[i, 0], matrix[i, 1], matrix[i, 2], matrix[i, 3], matrix[i, 4], matrix[i, 5], matrix[i, 6],
                    matrix[i, 7], matrix[i, 8], matrix[i, 9], matrix[i, 10], matrix[i, 11], matrix[i, 12], matrix[i, 13],
                    matrix[i, 14], matrix[i, 15], matrix[i, 16], matrix[i, 17], matrix[i, 18], matrix[i, 19]);
            }

            worksheet.Cell(1, 2).Value = "Network Matrix";
            worksheet.Range(1, 2, 1, 22).Merge().AddToNamed("Titles");
            worksheet.Cell(2, 2).InsertTable(table.AsEnumerable());

            var titlesStyle = workbook.Style;
            titlesStyle.Font.Bold = true;
            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titlesStyle.Fill.BackgroundColor = XLColor.Cyan;
            workbook.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs("Network Matrix.xlsx");
        }
    }
}
