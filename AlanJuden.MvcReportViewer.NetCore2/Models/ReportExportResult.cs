using System.Collections.Generic;
using AlanJuden.MvcReportViewer.NetCore2.Helpers;

namespace AlanJuden.MvcReportViewer.NetCore2.Models
{
	public class ReportExportResult
	{
		public int CurrentPage { get; set; }
		public List<string> StreamIDs { get; set; }
		public ReportServiceExecution.ExecutionInfo ExecutionInfo { get; set; }
		public byte[] ReportData { get; set; }
		public string Format { get; set; }
		public ReportFormats FormatType { get { return this.Format.NameToEnum<ReportFormats>(); } }
		public string MimeType { get; set; }
		public List<ReportServiceExecution.Warning> Warnings { get; set; }
		public int TotalPages { get; set; }
		public List<ReportParameterInfo> Parameters { get; set; }

		public ReportExportResult()
		{
			this.Parameters = new List<ReportParameterInfo>();
			this.StreamIDs = new List<string>();
			this.Warnings = new List<ReportServiceExecution.Warning>();
		}

	}
}
