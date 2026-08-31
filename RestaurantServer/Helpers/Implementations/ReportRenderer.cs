using RestaurantServer.Constants;
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
                throw new Exception(ValidationMessages.InvalidPath + reportRelativePath);
            }

            if (!File.Exists(reportPath))
            {
                throw new FileNotFoundException(ValidationMessages.ReportNotFound, reportPath);
            }

            var fileInfo = new FileInfo(reportPath);

            if (fileInfo.Length == 0)
            {
                throw new Exception(ValidationMessages.TelerikFileLengthZero);
            }

            var reportPackager = new Telerik.Reporting.ReportPackager();

            Telerik.Reporting.Report report;

            using (var stream = File.OpenRead(reportPath))
            {
                report = (Telerik.Reporting.Report)reportPackager.UnpackageDocument(stream);
            }

            if (report == null)
            {
                throw new Exception(ValidationMessages.ReportIsNull);
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
                throw new Exception(ValidationMessages.RenderReportisNull);
            }

            if (result.DocumentBytes == null)
            {
                throw new Exception(ValidationMessages.DocumentBytesisNull);
            }

            if (result.DocumentBytes.Length == 0)
            {
                throw new Exception(ValidationMessages.EmptyPdf);
            }

            return result.DocumentBytes;
        }
    }
}
