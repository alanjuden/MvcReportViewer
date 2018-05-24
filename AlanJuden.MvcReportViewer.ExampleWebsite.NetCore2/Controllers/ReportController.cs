using System.Net;
using AlanJuden.MvcReportViewer.NetCore2;
using AlanJuden.MvcReportViewer.NetCore2.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AlanJuden.MvcReportViewer.ExampleWebsite.NetCore2.Controllers
{
    public class ReportController : BaseReportController
    {
        protected override ICredentials NetworkCredentials
		{
			get
			{
				//Custom Domain authentication (be sure to pull the info from a config file)
				//return new System.Net.NetworkCredential("username", "password", "domain");

				//Default domain credentials (windows authentication)
				return System.Net.CredentialCache.DefaultNetworkCredentials;
			}
		}

		protected override string ReportServerUrl
		{
			get
			{
				//You don't want to put the full API path here, just the path to the report server's ReportServer directory that it creates (you should be able to access this path from your browser: https://YourReportServerUrl.com/ReportServer/ReportExecution2005.asmx )
				return "https://YourReportServerUrl.com/ReportServer";
			}
		}

		public ActionResult MyReport(string namedParameter1, string namedParameter2)
		{
			var model = this.GetReportViewerModel(Request);
			model.ReportPath = "/Folder/Report File Name";
			model.AddParameter("Parameter1", namedParameter1);
			model.AddParameter("Parameter2", namedParameter2);

			return View("ReportViewer", model);
		}
	}
}
