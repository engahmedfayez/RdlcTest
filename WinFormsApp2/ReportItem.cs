﻿namespace WinFormsApp2
{
    public class ReportItem
    {
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal Total => Price * Qty;
    }
}
