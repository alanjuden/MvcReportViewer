using System;
using System.Collections.Generic;
using System.Linq;

namespace JustCSharp.ReportViewer
{
	public class ReportViewerModel
	{
		/// <summary>
		/// 
		/// </summary>
		public string ServerUrl { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ReportPath { get; set; }

		/// <summary>
		/// This indicates whether or not to replace image urls from your report server to image urls on your local site to act as a proxy
		/// *useful if your report server is not accessible publicly*
		/// </summary>
		public bool UseCustomReportImagePath { get; set; }

		/// <summary>
		/// This is the local URL on your website that will handle returning images for you, be sure to use the replacement variable {0} in your string to represent the original image URL that came from your report server.
		/// </summary>
		public string ReportImagePath { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public System.Net.ICredentials Credentials { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Dictionary<string, string[]> Parameters { get; set; }

		public bool ShowHiddenParameters { get; set; }
		public ReportViewModes ViewMode { get; set; }
		public System.Text.Encoding Encoding { get; set; }

		/// <summary>
		/// This indicates whether or not the report will be preloaded when the page loads initially or if it will be an ajax request.
		/// </summary>
		public bool AjaxLoadInitialReport { get; set; }
		/// <summary>
		/// Setting this to 'true' enables the paging control and renders a single page at a time. Setting this to 'false' removes the paging control and shows all pages at once.
		/// </summary>
		public bool EnablePaging { get; set; }
		public int? Timeout { get; set; }
		public System.ServiceModel.HttpClientCredentialType ClientCredentialType { get; set; }

		public ReportViewerModel()
		{
			this.Parameters = new Dictionary<string, string[]>();
			this.ViewMode = ReportViewModes.View;
			this.UseCustomReportImagePath = false;
		}

		public void AddParameter(string name, string value)
		{
			this.AddParameter(name, new string[] { value });
		}

		public void AddParameter(string name, string[] values)
		{
			if (!name.HasValue()) { return; }

			if (this.Parameters.ContainsKey(name))
			{
				this.Parameters[name] = values;
			}
			else
			{
				this.Parameters.Add(name, values);
			}
		}

		public static List<string> KEYS_TO_IGNORE = new List<string>() { "ReportViewerEnablePaging", "reportPath", "startPage", "searchText", "page", "format" };
		public static string[] VALUE_SEPARATORS = new string[] { "," };

		public bool IsMissingAnyRequiredParameterValues(List<ReportParameterInfo> parameters)
		{
			var nonBlankParameters = parameters.Where(x => x.AllowBlank == false);
			var matchedParameters = this.Parameters.Where(x => nonBlankParameters.Select(p => p.Name).Contains(x.Key));
			var missingValueParameters = matchedParameters.Where(x => x.Value == null || x.Value.Length == 0 || (x.Value.Where(v => v == null || v == String.Empty)).Any());

			return missingValueParameters.Any();
		}
	}
}
