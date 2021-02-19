using System.Linq;
using Microsoft.AspNetCore.Http;

namespace JustCSharp.ReportViewer.ASPNetCore.Utilities
{
    public static class ReportViewerModelExtension
    {
        public static void BuildParameters(this ReportViewerModel model, HttpRequest request)
        {
            if (request.Query != null && request.Query.Keys != null)
            {
                foreach (var key in request.Query.Keys.Where(x => !ReportViewerModel.KEYS_TO_IGNORE.Contains(x)))
                {
                    model.AddParameter(key, request.Query[key].ToSafeString().ToStringList(ReportViewerModel.VALUE_SEPARATORS).ToArray());
                }
            }

            try
            {
                if (request.Form != null && request.Form.Keys != null)
                {
                    foreach (var key in request.Form?.Keys.Where(x => !ReportViewerModel.KEYS_TO_IGNORE.Contains(x)))
                    {
                        model.AddParameter(key, request.Form[key].ToSafeString().ToStringList(ReportViewerModel.VALUE_SEPARATORS).ToArray());
                    }
                }
            }
            catch
            {
                //No need to throw errors, just no Form was passed in and it's unhappy about that
            }
        }
    }
}