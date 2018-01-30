using System;
using System.Linq;
using System.Net.Http;
using AlanJuden.MvcReportViewer.NetCore2.Helpers;
using AlanJuden.MvcReportViewer.NetCore2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using AlanJuden.MvcReportViewer.NetCore2.Constants;

namespace AlanJuden.MvcReportViewer.NetCore2
{
	public abstract class BaseReportController : Controller
	{
		protected abstract System.Net.ICredentials NetworkCredentials { get; }
		protected abstract string ReportServerUrl { get; }

		/// <summary>
		/// This indicates whether or not to replace image urls from your report server to image urls on your local site to act as a proxy
		/// *useful if your report server is not accessible publicly*
		/// </summary>
		protected virtual bool UseCustomReportImagePath { get { return false; } }
		protected virtual bool AjaxLoadInitialReport { get { return true; } }
		protected virtual System.Text.Encoding Encoding { get { return System.Text.Encoding.ASCII; } }

		protected virtual string ReportImagePath
		{
			get
			{
			    return Report.ReportImagePath; // "/Report/ReportImage/?originalPath={0}";
			}
		}

		protected virtual System.ServiceModel.HttpClientCredentialType ClientCredentialType
		{
			get
			{
				return System.ServiceModel.HttpClientCredentialType.Windows;
			}
		}

		protected virtual int? Timeout
		{
			get
			{
				return null;
			}
		}

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
				case FileTypes.Csv:
					format = FileTypes.Csv; //"CSV";
				    extension = FileTypes.CsvExtension; //".csv";
					break;

                case FileTypes.Mhtml: //; "MHTML":
					format = FileTypes.Mhtml; //"MHTML";
                    extension = FileTypes.MhtmlExtension; //".mht";
					break;

                case FileTypes.Pdf: // "PDF":
                    format = FileTypes.Pdf; //"PDF";
                    extension = FileTypes.PdfExtension; //".pdf";
					break;

                case FileTypes.Tiff: // "TIFF":
                    format = FileTypes.Image; //"IMAGE";
                    extension = FileTypes.TiffExtension; // ".tif";
					break;

                case FileTypes.Xml: //"XML":
                    format = FileTypes.Xml; //"XML";
                    extension = FileTypes.XmlExtension; //".xml";
					break;

                case FileTypes.WordOpenXml: //"WORDOPENXML":
                    format = FileTypes.WordOpenXml; //"WORDOPENXML";
                    extension = FileTypes.WordOpenXmlExtension; //".docx";
					break;

                case FileTypes.ExcelOpenXml: // "EXCELOPENXML":
				default:
                    format = FileTypes.ExcelOpenXmlFormat; // "EXCELOPENXML";
                    extension = FileTypes.ExcelOpenXmlExtension; // ".xlsx";
					break;
			}

			var contentData = ReportServiceHelpers.ExportReportToFormat(model, format);

			var filename = reportPath;
			if (filename.Contains("/"))
			{
				filename = filename.Substring(filename.LastIndexOf("/", StringComparison.Ordinal));
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

			return Json(CoreHtmlHelpers.ParametersToHtmlString(null, model));
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
			var startIndex = rawUrl.IndexOf(originalPath, StringComparison.Ordinal);
			if (startIndex > -1)
			{
				originalPath = rawUrl.Substring(startIndex);
			}

			var clientHandler = new HttpClientHandler { Credentials = this.NetworkCredentials };
			using (var client = new HttpClient(clientHandler))
			{
				var imageData = client.GetByteArrayAsync(originalPath).Result;

			    return new FileContentResult(imageData, ContentTypeResult.Png); // "image/png");

			}
		}

		protected ReportViewerModel GetReportViewerModel(HttpRequest request)
		{
		  var model = new ReportViewerModel
		  {
		    AjaxLoadInitialReport = this.AjaxLoadInitialReport,
		    ClientCredentialType = this.ClientCredentialType,
		    Credentials = this.NetworkCredentials
		  };

		    var enablePagingResult = _getRequestValue(request, Report.ReportViewerEnablePaging); //"ReportViewerEnablePaging");
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
				if (values != null && values.Any())
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
