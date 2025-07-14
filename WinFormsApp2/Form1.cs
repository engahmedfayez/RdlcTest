using Microsoft.Reporting.NETCore;
using Microsoft.Reporting.WinForms;
using Microsoft.ReportingServices.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;



namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();





        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Microsoft.Reporting.WinForms.LocalReport report = new Microsoft.Reporting.WinForms.LocalReport();

            var items = new[] { new ReportItem { Description = "Widget 6000", Price = 104.99m, Qty = 1 }, new ReportItem { Description = "Gizmo MAX", Price = 1.41m, Qty = 25 } };
            var parameters = new[] { new Microsoft.Reporting.WinForms.ReportParameter("Title", "Invoice 4/2020") };
            FileStream? fs = new FileStream("Report.rdlc", FileMode.Open);
            report.LoadReportDefinition(fs);
            report.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("items" , items) );
            report.SetParameters(parameters);

            report.Refresh();
        }
    }





}
