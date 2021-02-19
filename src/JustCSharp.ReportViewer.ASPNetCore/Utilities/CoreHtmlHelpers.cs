﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JustCSharp.ReportViewer.ASPNetCore.Utilities
{
	public static class CoreHtmlHelpers
	{
		public static HtmlString RenderReportViewer(this IHtmlHelper helper, ReportViewerModel model, int? startPage = 1)
		{
			var sb = new StringBuilder();

			var reportServerDomainUri = new Uri(model.ServerUrl);
			var contentData = ReportServiceHelpers.ExportReportToFormat(model, ReportFormats.Html4_0, startPage, startPage);

			sb.AppendLine("<form class='form-inline' id='frmReportViewer' name='frmReportViewer'>");
			sb.AppendLine("	<div class='ReportViewer row'>");
			sb.AppendLine("		<div class='ReportViewerHeader row'>");
			sb.AppendLine("			<div class='ParametersContainer col-sm-12'>");
			sb.AppendLine("				<div class='Parameters col-sm-10'>");

			sb.AppendLine(ParametersToHtmlString(contentData.Parameters, model));

			sb.AppendLine("				</div>");

			sb.AppendLine("				<div class='ReportViewerViewReport col-sm-2 text-center'>");
			sb.AppendLine("					<button type='button' class='btn btn-primary ViewReport'>View Report</button>");
			sb.AppendLine("				</div>");
			sb.AppendLine("			</div>");

			sb.AppendLine("			<div class='ReportViewerToolbar row'>");
			sb.AppendLine("				<div class='ReportViewerPager'>");
			sb.AppendLine("					<div class='btn-toolbar'>");

			if (model.EnablePaging)
			{ 
				sb.AppendLine("						<div class='btn-group'>");
				sb.AppendLine($"							<a href='#' title='First Page' class='btn btn-default FirstPage'{(contentData.TotalPages == 1 ? " disabled='disabled'" : "")}><span class='glyphicon glyphicon-step-backward'></span></a>");
				sb.AppendLine($"							<a href='#' title='Previous Page' class='btn btn-default PreviousPage'{(contentData.TotalPages == 1 ? " disabled='disabled'" : "")}><span class='glyphicon glyphicon-chevron-left'></span></a>");
				sb.AppendLine("						</div>");
				sb.AppendLine("						<div class='btn-group'>");
				sb.AppendLine($"							<span class='PagerNumbers'><input type='text' id='ReportViewerCurrentPage' name='ReportViewerCurrentPage' class='form-control' value='{contentData.CurrentPage}' /> of <span id='ReportViewerTotalPages'>{contentData.TotalPages}</span></span>");
				sb.AppendLine("						</div>");
				sb.AppendLine("						<div class='btn-group'>");
				sb.AppendLine($"							<a href='#' title='Next Page' class='btn btn-default NextPage'{(contentData.TotalPages == 1 ? " disabled='disabled'" : "")}><span class='glyphicon glyphicon-chevron-right'></span></a>");
				sb.AppendLine($"							<a href='#' title='Last Page' class='btn btn-default LastPage'{(contentData.TotalPages == 1 ? " disabled='disabled'" : "")}><span class='glyphicon glyphicon-step-forward'></span></a>");
				sb.AppendLine("						</div>");
			}
			sb.AppendLine("						<div class='btn-group'>");
			sb.AppendLine("							<span class='SearchText'>");
			sb.AppendLine($"								<input type='text' id='ReportViewerSearchText' name='ReportViewerSearchText' class='form-control' value='' />");
			sb.AppendLine($"								<a href='#' title='Find' class='btn btn-info FindTextButton'><span class='glyphicon glyphicon-search' style='padding-right: .5em;'></span>Find</a>");
			sb.AppendLine("							</span>");
			sb.AppendLine("						</div>");
			sb.AppendLine("						<div class='btn-group'>");
			sb.AppendLine("							<a href='#' title='Export' class='dropdown-toggle btn btn-default' data-toggle='dropdown' role='button' aria-haspopup='true' area-expanded='false'>");
			sb.AppendLine("								<span class='glyphicon glyphicon-floppy-save' style='color: steelblue;'></span>");
			sb.AppendLine("								<span class='caret'></span>");
			sb.AppendLine("							</a>");
			sb.AppendLine("							<ul class='dropdown-menu'>");
			sb.AppendLine("								<li><a href='#' class='ExportCsv'>CSV (comma delimited)</a></li>");
			sb.AppendLine("								<li><a href='#' class='ExportExcelOpenXml'>Excel</a></li>");
			sb.AppendLine("								<li><a href='#' class='ExportMhtml'>MHTML (web archive)</a></li>");
			sb.AppendLine("								<li><a href='#' class='ExportPdf'>PDF</a></li>");
			sb.AppendLine("								<li><a href='#' class='ExportTiff'>TIFF file</a></li>");
			sb.AppendLine("								<li><a href='#' class='ExportWordOpenXml'>Word</a></li>");
			sb.AppendLine("								<li><a href='#' class='ExportXml'>XML file with report data</a></li>");
			sb.AppendLine("							</ul>");
			//sb.AppendLine("						</div>");
			//sb.AppendLine("						<div class='btn-group'>");
			sb.AppendLine("							<a href='#' title='Refresh' class='btn btn-default Refresh'><span class='glyphicon glyphicon-refresh' style='color: green;'></span></a>");
			//sb.AppendLine("						</div>");
			//sb.AppendLine("						<div class='btn-group'>");
			sb.AppendLine("							<a href='#' title='Print' class='btn btn-default Print'><span class='glyphicon glyphicon-print' style='color: grey;'></span></a>");
			sb.AppendLine("						</div>");
			sb.AppendLine("					</div>");
			sb.AppendLine("				</div>");
			sb.AppendLine("			</div>");
			sb.AppendLine("		</div>");
			sb.AppendLine("		<div class='ReportViewerContentContainer row'>");
			sb.AppendLine("			<div class='ReportViewerContent'>");

			if (model.IsMissingAnyRequiredParameterValues(contentData.Parameters))
			{
				sb.AppendLine("			<div class='ReportViewerInformation'>Please fill parameters and run the report...</div>");
			}
			else
			{
				if (model.AjaxLoadInitialReport)
				{
					sb.AppendLine("			<script type='text/javascript'>$(document).ready(function () { viewReportPage(1); });</script>");
				}
				else
				{
					if (contentData.ReportData == null || contentData.ReportData.Length == 0)
					{
						sb.AppendLine("");
					}
					else
					{
						var content = model.Encoding.GetString(contentData.ReportData);

						if (model.UseCustomReportImagePath && model.ReportImagePath.HasValue())
						{
							content = ReportServiceHelpers.ReplaceImageUrls(model, content);
						}

						sb.AppendLine($"			{content}");
					}
				}
			}

			sb.AppendLine("			</div>");
			sb.AppendLine("		</div>");
			sb.AppendLine("	</div>");
			sb.AppendLine("</form>");

			sb.AppendLine("<script type='text/javascript'>");
			sb.AppendLine("	function ReportViewer_Register_OnChanges() {");

			var dependencyFieldKeys = new List<string>();
			foreach (var parameter in contentData.Parameters.Where(x => x.Dependencies != null && x.Dependencies.Any()))
			{
				foreach (var key in parameter.Dependencies)
				{
					if (!dependencyFieldKeys.Contains(key))
					{
						dependencyFieldKeys.Add(key);
					}
				}
			}

			foreach (var queryParameter in contentData.Parameters.Where(x => dependencyFieldKeys.Contains(x.Name)))
			{
				sb.AppendLine("		$('#" + queryParameter.Name + "').change(function () {");
				sb.AppendLine("			reloadParameters();");
				sb.AppendLine("		});");
			}

			sb.AppendLine("	}");

			sb.AppendLine("</script>");

			return new HtmlString(sb.ToString());
		}

		public static string ParametersToHtmlString(List<ReportParameterInfo> parameters, ReportViewerModel model)
		{
			StringBuilder sb = new StringBuilder();

			if (parameters == null)
			{
				var contentData = new ReportExportResult();
				var definedParameters = ReportServiceHelpers.GetReportParameters(model, true);
				contentData.SetParameters(definedParameters, model.Parameters);
				parameters = contentData.Parameters;
			}

			//Parameters start
			foreach (var reportParameter in parameters)
			{
				sb.AppendLine("					<div class='Parameter col-md-6 col-sm-12'>");
				if (reportParameter.PromptUser || model.ShowHiddenParameters)
				{
					sb.AppendLine($"						<div class='col-sm-4'><label for='{reportParameter.Name}'>{reportParameter.Prompt.HtmlEncode()}</label></div>");

					sb.AppendLine("							<div class='col-sm-8'>");
					if (reportParameter.ValidValues != null && reportParameter.ValidValues.Any())
					{
						sb.AppendLine($"						<select id='{reportParameter.Name}' name='{reportParameter.Name}' class='form-control' {(reportParameter.MultiValue ? "multiple='multiple'" : "")}>");
						foreach (var value in reportParameter.ValidValues)
						{
							sb.AppendLine($"							<option value='{value.Value}' {(reportParameter.SelectedValues.Contains(value.Value) ? "selected='selected'" : "")}>{value.Label.HtmlEncode()}</option>");
						}
						sb.AppendLine($"						</select>");
					}
					else
					{
						var selectedValue = reportParameter.SelectedValues.FirstOrDefault();

						if (reportParameter.Type == ReportService.ParameterTypeEnum.Boolean)
						{
							sb.AppendLine($"						<input type='checkbox' id='{reportParameter.Name}' name='{reportParameter.Name}' class='form-control' {(selectedValue.ToBoolean() ? "checked='checked'" : "")} />");
						}
						else if (reportParameter.Type == ReportService.ParameterTypeEnum.DateTime)
						{
							sb.AppendLine($"						<input type='datetime' id='{reportParameter.Name}' name='{reportParameter.Name}' class='form-control' value='{selectedValue}' />");
						}
						else
						{
							sb.AppendLine($"						<input type='text' id='{reportParameter.Name}' name='{reportParameter.Name}' class='form-control' value='{selectedValue}' />");
						}
					}

					sb.AppendLine("							</div>");
				}
				else
				{
					if (reportParameter.SelectedValues != null && reportParameter.SelectedValues.Any())
					{
						var values = reportParameter.SelectedValues.Where(x => x != null).Select(x => x).ToArray();
						sb.AppendLine($"			<input type='hidden' id='{reportParameter.Name}' name='{reportParameter.Name}' value='{String.Join(",", values)}' />");
					}
				}

				sb.AppendLine($"			<input type='hidden' id='ReportViewerEnablePaging' name='ReportViewerEnablePaging' value='{model.EnablePaging}' />");

				sb.AppendLine("					</div>");
			}

			return sb.ToString();
		}
	}
}
