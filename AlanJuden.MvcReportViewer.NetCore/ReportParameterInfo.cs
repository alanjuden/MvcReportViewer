using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlanJuden.MvcReportViewer
{
	public class ReportParameterInfo
	{
		public string Name { get; set; }
		public string Prompt { get; set; }
		public bool MultiValue { get; set; }
		public List<ValidValue> ValidValues { get; set; }
		public List<string> SelectedValues { get; set; }
		public ReportService.ParameterTypeEnum Type { get; set; }
		public bool PromptUser { get; set; }
		public bool AllowBlank { get; internal set; }

		public ReportParameterInfo()
		{
			this.ValidValues = new List<ValidValue>();
			this.SelectedValues = new List<string>();
		}
	}

	public class ValidValue
	{
		public string Label { get; set; }
		public string Value { get; set; }

		public ValidValue()
		{

		}

		public ValidValue(string label, string value)
		{
			this.Label = label;
			this.Value = value;
		}
	}
}
