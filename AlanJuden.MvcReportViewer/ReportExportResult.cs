using System.Collections.Generic;
using System.Linq;

namespace AlanJuden.MvcReportViewer
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

		internal void SetParameters(ReportService.ReportParameter[] definedReportParameters, Dictionary<string, string[]> userParameters)
		{
			if (definedReportParameters != null)
			{
				foreach (var definedReportParameter in definedReportParameters)
				{
					var reportParameter = new ReportParameterInfo();
					reportParameter.MultiValue = definedReportParameter.MultiValue;
					reportParameter.Name = definedReportParameter.Name;
					reportParameter.Prompt = definedReportParameter.Prompt;
					reportParameter.PromptUser = ((definedReportParameter.PromptUser && !definedReportParameter.Prompt.HasValue()) ? false : definedReportParameter.PromptUser);
					reportParameter.Type = definedReportParameter.Type;

					if (definedReportParameter.ValidValues != null)
					{
						foreach (var validValue in definedReportParameter.ValidValues)
						{
							reportParameter.ValidValues.Add(validValue.Label, validValue.Value);
						}
					}

					if (userParameters != null && userParameters.ContainsKey(definedReportParameter.Name))
					{
						reportParameter.SelectedValues = userParameters[definedReportParameter.Name].ToList();
					}
					else if (definedReportParameter.DefaultValues != null && definedReportParameter.DefaultValues.Any())
					{
						reportParameter.SelectedValues = definedReportParameter.DefaultValues.ToList();
					}

					this.Parameters.Add(reportParameter);
				}
			}
		}
	}
}