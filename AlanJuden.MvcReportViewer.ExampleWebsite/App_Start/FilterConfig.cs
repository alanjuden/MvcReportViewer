using System.Web;
using System.Web.Mvc;

namespace AlanJuden.MvcReportViewer.ExampleWebsite
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
