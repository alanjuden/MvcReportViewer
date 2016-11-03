using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AlanJuden.MvcReportViewer.ExampleWebsite.Startup))]
namespace AlanJuden.MvcReportViewer.ExampleWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
