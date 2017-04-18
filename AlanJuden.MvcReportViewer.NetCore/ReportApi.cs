// -----------------------------------------------------------------------
// <copyright file="ReportApi.cs">
// Copyright © Andrei Tserakhau, Inc. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AlanJuden.MvcReportViewer.NetCore
{
    public abstract class BaseReportApiController : ReportController
    {
        [HttpPost]
        public JsonResult Execute(string reportPath, [FromBody] Dictionary<string, string[]> parameters, int page = 0)
        {
            try
            {
                var model = this.GetReportViewerModel(Request);

                model.ViewMode = ReportViewModes.View;
                model.ReportPath = reportPath;

                if (parameters.Any())
                {
                    model.Parameters = parameters;
                }

                var contentData = ReportServiceHelpers.ExportReportToFormat(model, ReportFormats.Html4_0, page, page);
                var content = model.Encoding.GetString(contentData.ReportData ?? new byte[] { });
                if (model.UseCustomReportImagePath && model.ReportImagePath.HasValue())
                {
                    content = ReportServiceHelpers.ReplaceImageUrls(model, content);
                }

                var jsonResult = Json(
                    new
                    {
                        contentData.CurrentPage,
                        content,
                        contentData.Parameters,
                        contentData.TotalPages
                    },
                    new Newtonsoft.Json.JsonSerializerSettings()
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    }
                );

                return jsonResult;
            }
            catch (Exception exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new
                {
                    ExceptionMessage = exception.Message,
                });
            }
        }
    }
}