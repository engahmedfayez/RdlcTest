using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using WebApplication1.DTO;

namespace WebApplication1.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IConfiguration _config;

        public ReportsController(IConfiguration config)
        {
            _config = config;
        }


        [HttpPost]
        [Route("Reports/viewer")]
        
        public IActionResult GenerateRDLCReport([FromBody] ReportInput reportInput)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            const string ReportDataSourceName = "Items";
            const string ReportFormat = "PDF";
            const string MimeTypePdf = "application/pdf";
            var HospitalName = _config.GetSection("ReportSettings")["HospitalName"];
            var LogoPath = _config.GetSection("ReportSettings")["LogoPath"];

            // 2. Load RDLC report
            var reportPath = Path.Combine(AppContext.BaseDirectory, "Reports", $"{reportInput.ReportName}.rdlc");//"Report.rdlc"



            // 3. Check if the report file exists
            if (!System.IO.File.Exists(reportPath))
            {
                return NotFound($"Report file ['{reportInput.ReportName}.rdlc'] not found.");
            }
            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;


            #region Set Header Data
            // 4. Set report header data
            DataTable headerDataTable = new DataTable();
            headerDataTable.Columns.Add("HospitalName", typeof(string));
            headerDataTable.Columns.Add("ImageByte", typeof(byte[]));
            // Add a row with the hospital name and logo
            headerDataTable.Rows.Add(HospitalName, System.IO.File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory+ LogoPath));
            
            localReport.DataSources.Add(new ReportDataSource("HeaderData", headerDataTable));

            #endregion



            // Deserialize the input data
            string ReportDataJson = reportInput.ReportData;
            string ReportArgsJson = reportInput.ReportArgs;
            DataTable reportDataTable;
            List<string> reportArgs;
            ReportParameter[] parameters;


            try
            {   // Convert JSON data to DataTable and List
                reportDataTable = JsonConvert.DeserializeObject<DataTable>(ReportDataJson);
                if (reportDataTable == null || reportDataTable.Rows.Count == 0)
                {
                    return BadRequest("Report data cannot be null or empty.");
                }
                localReport.DataSources.Add(new ReportDataSource(ReportDataSourceName, reportDataTable));

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error [Report Data] : {ex.Message}");
            }

            try
            {
                reportArgs = JsonConvert.DeserializeObject<List<string>>(ReportArgsJson);
                if (reportArgs == null || !reportArgs.Any())
                {
                    return BadRequest("Report arguments cannot be null or empty.");
                }
                // Convert list to ReportParameter array
                parameters = reportArgs
               .Select((value, index) => new ReportParameter($"arg{index}", value))
               .ToArray();


            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal Server Error [Report Args] : {ex.Message}");
            }
            try
            {
                // 5. Render to PDF
                localReport.SetParameters(parameters);
                var result = localReport.Render(ReportFormat);
                return File(result, MimeTypePdf, $@"{reportInput.ReportName}.pdf");
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An unexpected error occurred while generating the report [Exception Details] ." + ex.Message);
            }

        }

    }
}
