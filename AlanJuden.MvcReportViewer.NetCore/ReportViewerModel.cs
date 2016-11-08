using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public bool ShowHiddenParameters { get; set; }

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

		public void BuildParameters(HttpRequest request)
		{
			if (request.Query != null && request.Query.Keys != null)
			{
				foreach (var key in request.Query.Keys)
				{
					this.AddParameter(key, request.Query[key].ToSafeString().ToStringList().ToArray());
				}
			}

			try
			{
				if (request.Form != null && request.Form.Keys != null)
				{
					foreach (var key in request.Form?.Keys)
					{
						this.AddParameter(key, request.Form[key].ToSafeString().ToStringList().ToArray());
					}
				}
			}
			catch
			{
				//No need to throw errors, just no Form was passed in and it's unhappy about that
			}
		}
	}
}
