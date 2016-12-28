using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AlanJuden.MvcReportViewer.ExampleWebsite.NetCore.Controllers
{
    public class ReportController : AlanJuden.MvcReportViewer.ReportController
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
				//return "https://YourReportServerUrl.com/ReportServer";
				return "http://reports.epctech.com/ReportServer";
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

		public ActionResult PendingReceiversByWarehouseReport(int? warehouse = null, bool? excludeProjectsOnHold = false)
		{
			var model = this.GetReportViewerModel(Request);
			model.ReportPath = "/Receiving/Pending Receivers by Warehouse";

			if (warehouse == null || warehouse == 0)
			{
				warehouse = 1;
			}

			model.AddParameter("Warehouse", warehouse.ToString());
			model.AddParameter("ExcludeProjectsOnHold", excludeProjectsOnHold.ToString());

			return View("ReportViewer", model);
		}

		public ActionResult FulfillmentOrderReport(string documentName, string warehouses)
		{
			var model = this.GetReportViewerModel(Request);
			model.ReportPath = "/Sales/Fulfillment Order Report";
			model.AddParameter("DocumentName", documentName);
			model.AddParameter("Warehouse", warehouses);

			return View("ReportViewer", model);
		}
	}
}
