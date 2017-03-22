using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlanJuden.MvcReportViewer
{
	public static class ReportServiceHelpers
	{
		private static System.ServiceModel.HttpBindingBase _initializeHttpBinding(string url, ReportViewerModel model)
		{
			if (url.ToLower().StartsWith("https"))
			{
				var binding = new System.ServiceModel.BasicHttpsBinding(System.ServiceModel.BasicHttpsSecurityMode.Transport);
				binding.Security.Transport.ClientCredentialType = model.ClientCredentialType;
				binding.MaxReceivedMessageSize = int.MaxValue;
				if (model.Timeout.HasValue)
				{
					if (model.Timeout == System.Threading.Timeout.Infinite)
					{
						binding.CloseTimeout = TimeSpan.MaxValue;
						binding.OpenTimeout = TimeSpan.MaxValue;
						binding.ReceiveTimeout = TimeSpan.MaxValue;
						binding.SendTimeout = TimeSpan.MaxValue;
					}
					else
					{
						binding.CloseTimeout = new TimeSpan(0, 0, model.Timeout.Value);
						binding.OpenTimeout = new TimeSpan(0, 0, model.Timeout.Value);
						binding.ReceiveTimeout = new TimeSpan(0, 0, model.Timeout.Value);
						binding.SendTimeout = new TimeSpan(0, 0, model.Timeout.Value);
					}
				}

				return binding;
			}
			else
			{
				var binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly);
				binding.Security.Transport.ClientCredentialType = model.ClientCredentialType;
				binding.MaxReceivedMessageSize = int.MaxValue;
				if (model.Timeout.HasValue)
				{
					if (model.Timeout == System.Threading.Timeout.Infinite)
					{
						binding.CloseTimeout = TimeSpan.MaxValue;
						binding.OpenTimeout = TimeSpan.MaxValue;
						binding.ReceiveTimeout = TimeSpan.MaxValue;
						binding.SendTimeout = TimeSpan.MaxValue;
					}
					else
					{
						binding.CloseTimeout = new TimeSpan(0, 0, model.Timeout.Value);
						binding.OpenTimeout = new TimeSpan(0, 0, model.Timeout.Value);
						binding.ReceiveTimeout = new TimeSpan(0, 0, model.Timeout.Value);
						binding.SendTimeout = new TimeSpan(0, 0, model.Timeout.Value);
					}
				}

				return binding;
			}
		}

		public static ReportService.ReportParameter[] GetReportParameters(ReportViewerModel model, bool forRendering = false)
		{
			var url = model.ServerUrl + ((model.ServerUrl.ToSafeString().EndsWith("/")) ? "" : "/") + "ReportService2005.asmx";

			var basicHttpBinding = _initializeHttpBinding(url, model);
			var service = new ReportService.ReportingService2005SoapClient(basicHttpBinding, new System.ServiceModel.EndpointAddress(url));
			service.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
			service.ClientCredentials.Windows.ClientCredential = (System.Net.NetworkCredential)(model.Credentials ?? System.Net.CredentialCache.DefaultCredentials);

			string historyID = null;
			ReportService.ParameterValue[] values = null;
			ReportService.DataSourceCredentials[] rsCredentials = null;

			var parameters = service.GetReportParametersAsync(model.ReportPath, historyID, forRendering, values, rsCredentials).Result;

			return parameters;
		}

		public static ReportExportResult ExportReportToFormat(ReportViewerModel model, ReportFormats format, int? startPage = 0, int? endPage = 0)
		{
			return ExportReportToFormat(model, format.GetName(), startPage, endPage);
		}

		public static ReportExportResult ExportReportToFormat(ReportViewerModel model, string format, int? startPage = 0, int? endPage = 0)
		{
			var definedReportParameters = GetReportParameters(model, true);

			var url = model.ServerUrl + ((model.ServerUrl.ToSafeString().EndsWith("/")) ? "" : "/") + "ReportExecution2005.asmx";

			var basicHttpBinding = _initializeHttpBinding(url, model);
			var service = new ReportServiceExecution.ReportExecutionServiceSoapClient(basicHttpBinding, new System.ServiceModel.EndpointAddress(url));
			service.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
			service.ClientCredentials.Windows.ClientCredential = (System.Net.NetworkCredential)(model.Credentials ?? System.Net.CredentialCache.DefaultCredentials);

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
			var deviceInfo = $"<DeviceInfo>{outputFormat}{encodingFormat}<Toolbar>False</Toolbar>{htmlFragment}</DeviceInfo>";
			if (model.ViewMode == ReportViewModes.View && startPage.HasValue && startPage > 0)
			{
				if (model.EnablePaging)
				{
					deviceInfo = $"<DeviceInfo>{outputFormat}<Toolbar>False</Toolbar>{htmlFragment}<Section>{startPage}</Section></DeviceInfo>";
				}
				else
				{
					deviceInfo = $"<DeviceInfo>{outputFormat}<Toolbar>False</Toolbar>{htmlFragment}</DeviceInfo>";
				}
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

			ReportServiceExecution.ExecutionInfo executionInfo = null;
			string extension = null;
			string encoding = null;
			string mimeType = null;
			string[] streamIDs = null;
			ReportServiceExecution.Warning[] warnings = null;

			try
			{
				string historyID = null;
				executionInfo = service.LoadReportAsync(model.ReportPath, historyID).Result;
				executionHeader.ExecutionID = executionInfo.ExecutionID;
				
				var executionParameterResult = service.SetReportParameters(executionInfo.ExecutionID, reportParameters.ToArray(), "en-us").Result;

				if (model.EnablePaging)
				{
					var renderRequest = new ReportServiceExecution.Render2Request(format, deviceInfo, ReportServiceExecution.PageCountMode.Actual);
					var result = service.Render2(executionInfo.ExecutionID, renderRequest).Result;

					extension = result.Extension;
					mimeType = result.MimeType;
					encoding = result.Encoding;
					warnings = result.Warnings;
					streamIDs = result.StreamIds;

					exportResult.ReportData = result.Result;
				}
				else
				{
					var renderRequest = new ReportServiceExecution.RenderRequest(format, deviceInfo);
					var result = service.Render(executionInfo.ExecutionID, renderRequest).Result;

					extension = result.Extension;
					mimeType = result.MimeType;
					encoding = result.Encoding;
					warnings = result.Warnings;
					streamIDs = result.StreamIds;

					exportResult.ReportData = result.Result;
				}

				executionInfo = service.GetExecutionInfo(executionHeader.ExecutionID).Result;
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
			var url = model.ServerUrl + ((model.ServerUrl.ToSafeString().EndsWith("/")) ? "" : "/") + "ReportExecution2005.asmx";

			var basicHttpBinding = _initializeHttpBinding(url, model);
			var service = new ReportServiceExecution.ReportExecutionServiceSoapClient(basicHttpBinding, new System.ServiceModel.EndpointAddress(url));
			service.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
			service.ClientCredentials.Windows.ClientCredential = (System.Net.NetworkCredential)(model.Credentials ?? System.Net.CredentialCache.DefaultCredentials);

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
			var deviceInfo = $"<DeviceInfo>{outputFormat}{encodingFormat}<Toolbar>False</Toolbar>{htmlFragment}</DeviceInfo>";
			if (model.ViewMode == ReportViewModes.View && startPage.HasValue && startPage > 0)
			{
				deviceInfo = $"<DeviceInfo>{outputFormat}<Toolbar>False</Toolbar>{htmlFragment}<Section>{startPage}</Section></DeviceInfo>";
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

			ReportServiceExecution.ExecutionInfo executionInfo = null;
			string extension = null;
			string encoding = null;
			string mimeType = null;
			string[] streamIDs = null;
			ReportServiceExecution.Warning[] warnings = null;

			try
			{
				string historyID = null;
				executionInfo = service.LoadReportAsync(model.ReportPath, historyID).Result;
				executionHeader.ExecutionID = executionInfo.ExecutionID;
				var executionParameterResult = service.SetReportParameters(executionInfo.ExecutionID, reportParameters.ToArray(), "en-us").Result;

				var renderRequest = new ReportServiceExecution.Render2Request(format, deviceInfo, ReportServiceExecution.PageCountMode.Actual);
				var result = service.Render2(executionInfo.ExecutionID, renderRequest).Result;

				extension = result.Extension;
				mimeType = result.MimeType;
				encoding = result.Encoding;
				warnings = result.Warnings;
				streamIDs = result.StreamIds;

				executionInfo = service.GetExecutionInfo(executionHeader.ExecutionID).Result;

				return service.FindString(executionInfo.ExecutionID, startPage.ToInt32(), executionInfo.NumPages, searchText).Result;
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
