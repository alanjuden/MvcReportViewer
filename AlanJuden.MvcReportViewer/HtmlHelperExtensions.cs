using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AlanJuden.MvcReportViewer
{
	public static class HtmlHelperExtensions
	{
		public static MvcHtmlString RenderReportViewer(this HtmlHelper helper, ReportViewerModel model, int? startPage = 1)
		{
			var sb = new StringBuilder();

			var reportServerDomainUri = new Uri(model.ServerUrl);
			var contentData = ReportServiceHelpers.ExportReportToFormat(model, ReportFormats.Html4_0, startPage, startPage);

			sb.AppendLine("<form class='form-inline' id='frmReportViewer' name='frmReportViewer'>");
			sb.AppendLine("	<div class='ReportViewer row'>");
			sb.AppendLine("		<div class='ReportViewerHeader col-sm-12'>");
			sb.AppendLine("			<div class='ParametersContainer col-sm-12'>");
			sb.AppendLine("				<div class='Parameters col-sm-10'>");
			//Parameters start
			foreach (var reportParameter in contentData.Parameters)
			{
				sb.AppendLine("					<div class='Parameter col-md-6 col-sm-12'>");
				if (reportParameter.PromptUser || model.ShowHiddenParameters)
				{
					sb.AppendLine($"						<div class='col-sm-4'><label for='{reportParameter.Name}'>{reportParameter.Prompt}</label></div>");

					sb.AppendLine("							<div class='col-sm-8'>");
					if (reportParameter.ValidValues != null && reportParameter.ValidValues.Any())
					{
						sb.AppendLine($"						<select id='{reportParameter.Name}' name='{reportParameter.Name}' class='form-control' {(reportParameter.MultiValue == true ? "multiple='multiple'" : "")}>");
						foreach (var value in reportParameter.ValidValues)
						{
							sb.AppendLine($"							<option value='{value.Value}' {(reportParameter.SelectedValues.Contains(value.Value) ? "selected='selected'" : "")}>{value.Key}</option>");
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
					if (reportParameter.ValidValues != null && reportParameter.ValidValues.Any())
					{
						var values = reportParameter.ValidValues.Where(x => x.Value != null).Select(x => x.Value).ToArray();
						sb.AppendLine($"			<input type='hidden' id='{reportParameter.Name}' name='{reportParameter.Name}' value='{String.Join(",", values)}' />");
					}
					else
					{
						var selectedValue = reportParameter.SelectedValues.FirstOrDefault();

						sb.AppendLine($"			<input type='hidden' id='{reportParameter.Name}' name='{reportParameter.Name}' value='{selectedValue}' />");
					}
				}

				sb.AppendLine("					</div>");
			}

			sb.AppendLine("				</div>");

			sb.AppendLine("				<div class='ReportViewerViewReport col-sm-2 text-center'>");
			sb.AppendLine("					<button type='button' class='btn btn-primary ViewReport'>View Report</button>");
			sb.AppendLine("				</div>");
			sb.AppendLine("			</div>");

			sb.AppendLine("			<div class='ReportViewerToolbar col-sm-12'>");
			sb.AppendLine("				<div class='ReportViewerPager'>");
			sb.AppendLine("					<div class='btn-toolbar'>");
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
			sb.AppendLine("		<div class='ReportViewerContent'>");

			if (contentData == null || contentData.ReportData == null || contentData.ReportData.Length == 0)
			{
				sb.AppendLine("");
			}
			else
			{
				var content = Encoding.ASCII.GetString(contentData.ReportData);

				content = ReportServiceHelpers.ReplaceImageUrls(model, content);

				sb.AppendLine($"		{content}");
			}

			sb.AppendLine("		</div>");
			sb.AppendLine("	</div>");
			sb.AppendLine("</form>");

			return new MvcHtmlString(sb.ToString());
		}
	}
}
