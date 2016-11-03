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
		public Dictionary<string, string> ValidValues { get; set; }
		public List<string> SelectedValues { get; set; }
		public ReportService.ParameterTypeEnum Type { get; set; }

		public ReportParameterInfo()
		{
			this.ValidValues = new Dictionary<string, string>();
			this.SelectedValues = new List<string>();
		}
	}
}
