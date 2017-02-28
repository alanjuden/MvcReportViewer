using System;
using System.Collections.Generic;
using System.Linq;

namespace AlanJuden.MvcReportViewer
{
	public static class ReportServiceHelpers
	{
		public static ReportService.ReportParameter[] GetReportParameters(ReportViewerModel model, bool forRendering = false)
		{
			var service = new ReportService.ReportingService2005();
			service.Url = model.ServerUrl + ((model.ServerUrl.ToSafeString().EndsWith("/")) ? "" : "/") + "ReportService2005.asmx";
			service.Credentials = model.Credentials ?? System.Net.CredentialCache.DefaultCredentials;

			string historyID = null;
			ReportService.ParameterValue[] values = null;
			ReportService.DataSourceCredentials[] rsCredentials = null;

			var parameters = service.GetReportParameters(model.ReportPath, historyID, forRendering, values, rsCredentials);

			return parameters;
		}

		public static ReportExportResult ExportReportToFormat(ReportViewerModel model, ReportFormats format, int? startPage = 0, int? endPage = 0)
		{
			return ExportReportToFormat(model, format.GetName(), startPage, endPage);
		}

		public static ReportExportResult ExportReportToFormat(ReportViewerModel model, string format, int? startPage = 0, int? endPage = 0)
		{
			var service = new ReportServiceExecution.ReportExecutionService();
			service.Url = model.ServerUrl + ((model.ServerUrl.ToSafeString().EndsWith("/")) ? "" : "/") + "ReportExecution2005.asmx";
			service.Credentials = model.Credentials ?? System.Net.CredentialCache.DefaultCredentials;

			var definedReportParameters = GetReportParameters(model, true);

			var exportResult = new ReportExportResult();
			exportResult.CurrentPage = (startPage.ToInt32() <= 0 ? 1 : startPage.ToInt32());
			exportResult.SetParameters(definedReportParameters, model.Parameters);

			if (startPage == 0)
			{
				startPage = 1;
			}

			if (endPage == 0)
			{
				endPage = startPage;
			}

			var outputFormat = $"<OutputFormat>{format}</OutputFormat>";
			var encodingFormat = $"<Encoding>{model.Encoding.EncodingName}</Encoding>";
			var htmlFragment = ((format.ToUpper() == "HTML4.0" && model.UseCustomReportImagePath == false && model.ViewMode == ReportViewModes.View) ? "<HTMLFragment>true</HTMLFragment>" : "");
			var deviceInfo = $"<DeviceInfo>{outputFormat}<Toolbar>False</Toolbar>{htmlFragment}</DeviceInfo>";
			if (model.ViewMode == ReportViewModes.View && startPage.HasValue && startPage > 0)
			{
				deviceInfo = $"<DeviceInfo>{outputFormat}{encodingFormat}<Toolbar>False</Toolbar>{htmlFragment}<Section>{startPage}</Section></DeviceInfo>";
			}

			var reportParameters = new List<ReportServiceExecution.ParameterValue>();
			foreach (var parameter in exportResult.Parameters)
			{
				bool addedParameter = false;
				foreach (var value in parameter.SelectedValues)
				{
					var reportParameter = new ReportServiceExecution.ParameterValue();
					reportParameter.Name = parameter.Name;
					reportParameter.Value = value;
					reportParameters.Add(reportParameter);

					addedParameter = true;
				}

				if (!addedParameter)
				{
					var reportParameter = new ReportServiceExecution.ParameterValue();
					reportParameter.Name = parameter.Name;
					reportParameters.Add(reportParameter);
				}
			}

			var executionHeader = new ReportServiceExecution.ExecutionHeader();
			service.ExecutionHeaderValue = executionHeader;

			ReportServiceExecution.ExecutionInfo executionInfo = null;
			string extension = null;
			string encoding = null;
			string mimeType = null;
			string[] streamIDs = null;
			ReportServiceExecution.Warning[] warnings = null;

			try
			{
				string historyID = null;
				executionInfo = service.LoadReport(model.ReportPath, historyID);
				service.SetExecutionParameters(reportParameters.ToArray(), "en-us");

				var result = service.Render2(format, deviceInfo, ReportServiceExecution.PageCountMode.Actual, out extension, out mimeType, out encoding, out warnings, out streamIDs);

				executionInfo = service.GetExecutionInfo();

				exportResult.ReportData = result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			exportResult.ExecutionInfo = executionInfo;
			exportResult.Format = format;
			exportResult.MimeType = mimeType;
			exportResult.StreamIDs = (streamIDs == null ? new List<string>() : streamIDs.ToList());
			exportResult.Warnings = (warnings == null ? new List<ReportServiceExecution.Warning>() : warnings.ToList());

			if (executionInfo != null)
			{
				exportResult.TotalPages = executionInfo.NumPages;
			}

			return exportResult;
		}

		/// <summary>
		/// Searches a specific report for your provided searchText and returns the page that it located the text on.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="searchText">The text that you want to search in the report</param>
		/// <param name="startPage">Starting page for the search to begin from.</param>
		/// <returns></returns>
		public static int? FindStringInReport(ReportViewerModel model, string searchText, int? startPage = 0)
		{
			var service = new ReportServiceExecution.ReportExecutionService();
			service.Url = model.ServerUrl + ((model.ServerUrl.ToSafeString().EndsWith("/")) ? "" : "/") + "ReportExecution2005.asmx";
			service.Credentials = model.Credentials ?? System.Net.CredentialCache.DefaultCredentials;

			var definedReportParameters = GetReportParameters(model, true);

			if (!startPage.HasValue || startPage == 0)
			{
				startPage = 1;
			}

			var exportResult = new ReportExportResult();
			exportResult.CurrentPage = startPage.ToInt32();
			exportResult.SetParameters(definedReportParameters, model.Parameters);

			var format = "HTML4.0";
			var outputFormat = $"<OutputFormat>{format}</OutputFormat>";
			var encodingFormat = $"<Encoding>{model.Encoding.EncodingName}</Encoding>";
			var htmlFragment = ((format.ToUpper() == "HTML4.0" && model.UseCustomReportImagePath == false && model.ViewMode == ReportViewModes.View) ? "<HTMLFragment>true</HTMLFragment>" : "");
			var deviceInfo = $"<DeviceInfo>{outputFormat}<Toolbar>False</Toolbar>{htmlFragment}</DeviceInfo>";
			if (model.ViewMode == ReportViewModes.View && startPage.HasValue && startPage > 0)
			{
				deviceInfo = $"<DeviceInfo>{outputFormat}{encodingFormat}<Toolbar>False</Toolbar>{htmlFragment}<Section>{startPage}</Section></DeviceInfo>";
			}

			var reportParameters = new List<ReportServiceExecution.ParameterValue>();
			foreach (var parameter in exportResult.Parameters)
			{
				bool addedParameter = false;
				foreach (var value in parameter.SelectedValues)
				{
					var reportParameter = new ReportServiceExecution.ParameterValue();
					reportParameter.Name = parameter.Name;
					reportParameter.Value = value;
					reportParameters.Add(reportParameter);

					addedParameter = true;
				}

				if (!addedParameter)
				{
					var reportParameter = new ReportServiceExecution.ParameterValue();
					reportParameter.Name = parameter.Name;
					reportParameters.Add(reportParameter);
				}
			}

			var executionHeader = new ReportServiceExecution.ExecutionHeader();
			service.ExecutionHeaderValue = executionHeader;

			ReportServiceExecution.ExecutionInfo executionInfo = null;
			string extension = null;
			string encoding = null;
			string mimeType = null;
			string[] streamIDs = null;
			ReportServiceExecution.Warning[] warnings = null;

			try
			{
				string historyID = null;
				executionInfo = service.LoadReport(model.ReportPath, historyID);
				service.SetExecutionParameters(reportParameters.ToArray(), "en-us");

				var result = service.Render2(format, deviceInfo, ReportServiceExecution.PageCountMode.Actual, out extension, out mimeType, out encoding, out warnings, out streamIDs);

				executionInfo = service.GetExecutionInfo();

				return service.FindString(startPage.ToInt32(), executionInfo.NumPages, searchText);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return 0;
		}

		/// <summary>
		/// I'm using this method to run images through a "proxy" on the local site due to credentials used on the report being different than the currently running user.
		/// I ran into issues where my domain account was different than the user that executed the report so the images gave 500 errors from the website. Also my report server
		/// is only internally available so this solved that issue for me as well.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="reportContent">This is the raw html output of your report.</param>
		/// <returns></returns>
		public static string ReplaceImageUrls(ReportViewerModel model, string reportContent)
		{
			var reportServerDomainUri = new Uri(model.ServerUrl);
			var searchForUrl = $"SRC=\"{reportServerDomainUri.Scheme}://{reportServerDomainUri.DnsSafeHost}/";
			//replace image urls with image data instead due to having issues accessing the images as a different authenticated user
			var imagePathIndex = reportContent.IndexOf(searchForUrl);
			while (imagePathIndex > -1)
			{
				var endIndex = reportContent.IndexOf("\"", imagePathIndex + 5);   //account for the length of src="
				if (endIndex > -1)
				{
					var imageUrl = reportContent.Substring(imagePathIndex + 5, endIndex - (imagePathIndex + 5));
					reportContent = reportContent.Replace(imageUrl, $"{String.Format(model.ReportImagePath, imageUrl)}");
				}

				imagePathIndex = reportContent.IndexOf(searchForUrl, imagePathIndex + 5);
			}

			return reportContent;
		}
	}
}