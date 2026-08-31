using RestaurantServer.DTOs.Responses;
using RestaurantServer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Telerik.Reporting;
using Telerik.Reporting.Processing;

namespace RestaurantServer.Helpers.Implementations
{
    public class ReportRenderer : IReportRenderer
    {
        public byte[] RenderTopOrderedItemsReport(
            List<TopOrderedItemResponse> data)
        {
            return RenderReport("~/Reports/TopOrderedItems.trdp", data);
        }

        public byte[] RenderFrequentlyBoughtTogetherReport(
            List<FrequentlyBoughtTogetherResponse> data)
        {
            return RenderReport("~/Reports/FrequentlyBoughtTogether.trdp", data);
        }

        private byte[] RenderReport(string reportRelativePath, object data)
        {
            var reportPath = HostingEnvironment.MapPath(reportRelativePath);

            if (string.IsNullOrWhiteSpace(reportPath))
            {
                throw new Exception("Could not map the report path: " + reportRelativePath);
            }

            if (!File.Exists(reportPath))
            {
                throw new FileNotFoundException("The Telerik report file was not found.", reportPath);
            }

            var fileInfo = new FileInfo(reportPath);

            if (fileInfo.Length == 0)
            {
                throw new Exception(
                    "The Telerik report file exists but its file size is 0 bytes. " +
                    "Open the report in the Telerik Report Designer and save it.");
            }

            var reportPackager = new Telerik.Reporting.ReportPackager();

            Telerik.Reporting.Report report;

            using (var stream = File.OpenRead(reportPath))
            {
                report = (Telerik.Reporting.Report)reportPackager.UnpackageDocument(stream);
            }

            if (report == null)
            {
                throw new Exception("Telerik could not load the report.");
            }

            var table = report.Items
                .Find("table1", true).FirstOrDefault() as Telerik.Reporting.Table;

            if (table != null)
            {
                table.DataSource = data;

                report.DataSource = null;
            }
            else
            {
                report.DataSource = data;
            }

            var reportSource = new InstanceReportSource
            {
                ReportDocument = report
            };

            var processor = new ReportProcessor();

            var result = processor.RenderReport("PDF", reportSource, null);

            if (result == null)
            {
                throw new Exception("Telerik RenderReport returned a null result.");
            }

            if (result.DocumentBytes == null)
            {
                throw new Exception("Telerik rendered the report, but DocumentBytes is null.");
            }

            if (result.DocumentBytes.Length == 0)
            {
                throw new Exception("Telerik generated an empty PDF.");
            }

            return result.DocumentBytes;
        }
    }
}
