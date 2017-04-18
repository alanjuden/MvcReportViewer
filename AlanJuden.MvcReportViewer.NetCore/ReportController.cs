using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http;

namespace AlanJuden.MvcReportViewer
{
    public abstract class ReportController : Controller
    {
        protected abstract System.Net.ICredentials NetworkCredentials { get; }
        protected abstract string ReportServerUrl { get; }

        /// <summary>
        /// This indicates whether or not to replace image urls from your report server to image urls on your local site to act as a proxy
        /// *useful if your report server is not accessible publicly*
        /// </summary>
        protected virtual bool UseCustomReportImagePath => false;

        protected virtual bool AjaxLoadInitialReport => true;
        protected virtual System.Text.Encoding Encoding => System.Text.Encoding.ASCII;

        protected virtual string ReportImagePath => "/Report/ReportImage/?originalPath={0}";

        protected virtual System.ServiceModel.HttpClientCredentialType ClientCredentialType => System.ServiceModel.HttpClientCredentialType.Windows;

        protected virtual int? Timeout => null;

        public JsonResult ViewReportPage(string reportPath, int? page = 0)
        {
            var model = this.GetReportViewerModel(Request);
            model.ViewMode = ReportViewModes.View;
            model.ReportPath = reportPath;

            var contentData = ReportServiceHelpers.ExportReportToFormat(model, ReportFormats.Html4_0, page, page);
            var content = model.Encoding.GetString(contentData.ReportData);
            if (model.UseCustomReportImagePath && model.ReportImagePath.HasValue())
            {
                content = ReportServiceHelpers.ReplaceImageUrls(model, content);
            }

            var jsonResult = Json(
                new
                {
                    CurrentPage = contentData.CurrentPage,
                    Content = content,
                    TotalPages = contentData.TotalPages
                },
                new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
                }
            );

            return jsonResult;
        }

        public FileResult ExportReport(string reportPath, string format)
        {
            var model = this.GetReportViewerModel(Request);
            model.ViewMode = ReportViewModes.Export;
            model.ReportPath = reportPath;

            var extension = "";
            switch (format.ToUpper())
            {
                case "CSV":
                    format = "CSV";
                    extension = ".csv";
                    break;

                case "MHTML":
                    format = "MHTML";
                    extension = ".mht";
                    break;

                case "PDF":
                    format = "PDF";
                    extension = ".pdf";
                    break;

                case "TIFF":
                    format = "IMAGE";
                    extension = ".tif";
                    break;

                case "XML":
                    format = "XML";
                    extension = ".xml";
                    break;

                case "WORDOPENXML":
                    format = "WORDOPENXML";
                    extension = ".docx";
                    break;

                case "EXCELOPENXML":
                default:
                    format = "EXCELOPENXML";
                    extension = ".xlsx";
                    break;
            }

            var contentData = ReportServiceHelpers.ExportReportToFormat(model, format);

            var filename = reportPath;
            if (filename.Contains("/"))
            {
                filename = filename.Substring(filename.LastIndexOf("/"));
                filename = filename.Replace("/", "");
            }

            filename = filename + extension;

            return File(contentData.ReportData, contentData.MimeType, filename);
        }

        public JsonResult FindStringInReport(string reportPath, string searchText, int? page = 0)
        {
            var model = this.GetReportViewerModel(Request);
            model.ViewMode = ReportViewModes.View;
            model.ReportPath = reportPath;

            return Json(ReportServiceHelpers.FindStringInReport(model, searchText, page).ToInt32());
        }

        public JsonResult ReloadParameters(string reportPath)
        {
            var model = this.GetReportViewerModel(Request);
            model.ViewMode = ReportViewModes.View;
            model.ReportPath = reportPath;

            return Json(AlanJuden.MvcReportViewer.CoreHtmlHelpers.ParametersToHtmlString(null, model));
        }

        public ActionResult PrintReport(string reportPath)
        {
            var model = this.GetReportViewerModel(Request);
            model.ViewMode = ReportViewModes.Print;
            model.ReportPath = reportPath;

            var contentData = ReportServiceHelpers.ExportReportToFormat(model, ReportFormats.Html4_0);
            var content = model.Encoding.GetString(contentData.ReportData);
            content = ReportServiceHelpers.ReplaceImageUrls(model, content);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("	<body>");
            //sb.AppendLine($"		<img src='data:image/tiff;base64,{Convert.ToBase64String(contentData.ReportData)}' />");
            sb.AppendLine($"		{content}");
            sb.AppendLine("		<script type='text/javascript'>");
            sb.AppendLine("			(function() {");
            /*
			sb.AppendLine("				var beforePrint = function() {");
			sb.AppendLine("					console.log('Functionality to run before printing.');");
			sb.AppendLine("				};");
			*/
            sb.AppendLine("				var afterPrint = function() {");
            sb.AppendLine("					window.onfocus = function() { window.close(); };");
            sb.AppendLine("					window.onmousemove = function() { window.close(); };");
            sb.AppendLine("				};");

            sb.AppendLine("				if (window.matchMedia) {");
            sb.AppendLine("					var mediaQueryList = window.matchMedia('print');");
            sb.AppendLine("					mediaQueryList.addListener(function(mql) {");
            sb.AppendLine("						if (mql.matches) {");
            //sb.AppendLine("							beforePrint();");
            sb.AppendLine("						} else {");
            sb.AppendLine("							afterPrint();");
            sb.AppendLine("						}");
            sb.AppendLine("					});");
            sb.AppendLine("				}");

            //sb.AppendLine("				window.onbeforeprint = beforePrint;");
            sb.AppendLine("				window.onafterprint = afterPrint;");

            sb.AppendLine("			}());");
            sb.AppendLine("			window.print();");
            sb.AppendLine("		</script>");
            sb.AppendLine("	</body>");

            sb.AppendLine("<html>");

            return Content(sb.ToString(), "text/html");
        }

        public FileContentResult ReportImage(string originalPath)
        {
            var rawUrl = this.Request.GetDisplayUrl().UrlDecode();
            var startIndex = rawUrl.IndexOf(originalPath);
            if (startIndex > -1)
            {
                originalPath = rawUrl.Substring(startIndex);
            }

            var clientHandler = new HttpClientHandler { Credentials = this.NetworkCredentials };
            using (var client = new HttpClient(clientHandler))
            {
                var imageData = client.GetByteArrayAsync(originalPath).Result;

                return new FileContentResult(imageData, "image/png");
            }
        }

        protected ReportViewerModel GetReportViewerModel(HttpRequest request)
        {
            var model = new ReportViewerModel();
            model.AjaxLoadInitialReport = this.AjaxLoadInitialReport;
            model.ClientCredentialType = this.ClientCredentialType;
            model.Credentials = this.NetworkCredentials;

            var enablePagingResult = _getRequestValue(request, "ReportViewerEnablePaging");
            if (enablePagingResult.HasValue())
            {
                model.EnablePaging = enablePagingResult.ToBoolean();
            }
            else
            {
                model.EnablePaging = true;
            }
            model.Encoding = this.Encoding;
            model.ServerUrl = this.ReportServerUrl;
            model.ReportImagePath = this.ReportImagePath;
            model.Timeout = this.Timeout;
            model.UseCustomReportImagePath = this.UseCustomReportImagePath;
            model.BuildParameters(Request);

            return model;
        }

        private string _getRequestValue(HttpRequest request, string key)
        {
            if (request.Query != null && request.Query.Keys != null && request.Query.Keys.Contains(key))
            {
                var values = request.Query[key].ToSafeString().Split(',');
                if (values != null && values.Count() > 0)
                {
                    return values[0].ToSafeString();
                }
            }

            try
            {
                if (request.Form != null && request.Form.Keys != null && request.Form.Keys.Contains(key))
                {
                    return request.Form[key].ToSafeString();
                }
            }
            catch
            {
                //No need to throw errors, just no Form was passed in and it's unhappy about that
            }

            return String.Empty;
        }
    }
}
