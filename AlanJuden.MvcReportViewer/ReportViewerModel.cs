using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AlanJuden.MvcReportViewer
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

		public ReportViewerModel()
		{
			this.Parameters = new Dictionary<string, string[]>();
		}

		public void AddParameter(string name, string value)
		{
			this.AddParameter(name, new string[1] { value });
		}

		public void AddParameter(string name, string[] values)
		{
			if (this.Parameters.ContainsKey(name))
			{
				this.Parameters[name] = values;
			}
			else
			{
				this.Parameters.Add(name, values);
			}
		}

		public void BuildParameters(HttpRequestBase request)
		{
			foreach (var key in request.QueryString.AllKeys)
			{
				this.AddParameter(key, request.QueryString[key].ToSafeString().ToStringList().ToArray());
			}

			foreach (var key in request.Form.AllKeys)
			{
				this.AddParameter(key, request.Form[key].ToSafeString().ToStringList().ToArray());
			}
		}
	}
}
