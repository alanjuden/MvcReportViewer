using System.Net;
using System.Text;
using AlanJuden.MvcReportViewer.NetCore;
using Microsoft.AspNetCore.Mvc;

namespace AlanJuden.MvcReportViewer.ExampleWebsite.NetCore.Controllers
{
    [Route("api/reports")]
    public class ReportApiController : BaseReportApiController
    {

        protected override ICredentials NetworkCredentials => CredentialCache.DefaultNetworkCredentials;

        protected override string ReportServerUrl => "https://YourReportServerUrl.com/ReportServer";
    }
}