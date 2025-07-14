
using Microsoft.Reporting.WinForms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<ReportItem> items = new List<ReportItem>
            {
               new ReportItem { Description = "Item 1", Price = 10.00m, Qty = 2 },
               new ReportItem { Description = "Item 2", Price = 20.00m, Qty = 1 },
               new ReportItem { Description = "Item 3", Price = 15.50m, Qty = 3 }
            };


          

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            reportViewer1.LocalReport.ReportPath = "Report.rdlc";

            ReportParameter titleParam = new ReportParameter("arg0", "Invoice 2025-05");
            reportViewer1.LocalReport.SetParameters(titleParam);


            // Optionally add data sources
            ReportDataSource rds = new ReportDataSource("Items", items);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);


            reportViewer1.RefreshReport();

            






          //  reportViewer1.RefreshReport();

            //  ExportReportViewerToPdf(reportViewer1, @"D:\1.pdf");

            /// MessageBox.Show("Report loaded successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private void Form1_Load(object sender, EventArgs e)
        {

            
        }


        private void ExportReportViewerToPdf(ReportViewer viewer, string outputPath)
        {
            try
            {
                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamIds;
                string mimeType, encoding, extension;

                byte[] bytes = viewer.LocalReport.Render(
                "PDF", null, out mimeType, out encoding, out extension,
                out streamIds, out warnings);

                File.WriteAllBytes(outputPath, bytes);

                MessageBox.Show("PDF exported to:\n" + outputPath, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Optional: Open the PDF
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = outputPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting PDF:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
