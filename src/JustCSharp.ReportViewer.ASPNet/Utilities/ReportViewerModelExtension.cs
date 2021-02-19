using System.Linq;
using System.Web;

namespace JustCSharp.ReportViewer.ASPNet.Utilities
{
    public static class ReportViewerModelExtension
    {
		public static void BuildParameters(this ReportViewerModel model, HttpRequestBase request)
		{
			foreach (var key in request.QueryString.AllKeys.Where(x => !ReportViewerModel.KEYS_TO_IGNORE.Contains(x)))
			{
				model.AddParameter(key, request.QueryString[key].ToSafeString().ToStringList(ReportViewerModel.VALUE_SEPARATORS).ToArray());
			}

			foreach (var key in request.Form.AllKeys.Where(x => !ReportViewerModel.KEYS_TO_IGNORE.Contains(x)))
			{
				model.AddParameter(key, request.Form[key].ToSafeString().ToStringList(ReportViewerModel.VALUE_SEPARATORS).ToArray());
			}
		}
	}
}
