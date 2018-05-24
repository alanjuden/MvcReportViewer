using System;

namespace AlanJuden.MvcReportViewer.ExampleWebsite.NetCore2.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}