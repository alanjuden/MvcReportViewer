using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlanJuden.MvcReportViewer.NetCore2.Models;

namespace AlanJuden.MvcReportViewer.NetCore2.Helpers
{
    public static class ReportExportResultHelper
    {
        internal static void SetParameters(this ReportExportResult reportExportResult, ReportService.ReportParameter[] definedReportParameters, Dictionary<string, string[]> userParameters)
        {
            if (definedReportParameters != null)
            {
                foreach (var definedReportParameter in definedReportParameters)
                {
                    var reportParameter = new ReportParameterInfo
                    {
                        AllowBlank = definedReportParameter.AllowBlank,
                        Dependencies = definedReportParameter.Dependencies,
                        MultiValue = definedReportParameter.MultiValue,
                        Name = definedReportParameter.Name,
                        Nullable = definedReportParameter.Nullable,
                        Prompt = definedReportParameter.Prompt,
                        PromptUser = ((!definedReportParameter.PromptUser || definedReportParameter.Prompt.HasValue()) && definedReportParameter.PromptUser),
                        Type = definedReportParameter.Type
                    };

                    if (definedReportParameter.ValidValues != null)
                    {
                        foreach (var validValue in definedReportParameter.ValidValues)
                        {
                            reportParameter.ValidValues.Add(new ValidValue(validValue.Label, validValue.Value));
                        }
                    }

                    if (userParameters != null && userParameters.ContainsKey(definedReportParameter.Name))
                    {
                        reportParameter.SelectedValues = userParameters[definedReportParameter.Name].ToList();
                    }
                    else if (definedReportParameter.DefaultValues != null && definedReportParameter.DefaultValues.Any())
                    {
                        reportParameter.SelectedValues = definedReportParameter.DefaultValues.ToList();
                    }

                    if (!reportParameter.SelectedValues.Any() && reportParameter.Type == ReportService.ParameterTypeEnum.Boolean && !reportParameter.Nullable)
                    {
                        //Set the default value to false if it's a boolean parameter
                        reportParameter.SelectedValues = new List<string>() { "False" };
                    }

                    reportExportResult.Parameters.Add(reportParameter);
                }
            }
        }
    }
}
