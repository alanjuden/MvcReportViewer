using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace JustCSharp.ReportViewer.ReportServiceExecution
{
    public partial class ReportExecutionServiceSoapClient
    {
		public Task<int> FindString(string executionID, int startPage, int endPage, string findValue)
		{
			using (OperationContextScope context = SetMessageHeaders(executionID))
			{
				return this.FindStringAsync(startPage, endPage, findValue);
			}
		}

		public Task<ExecutionInfo> GetExecutionInfo(string executionID)
		{
			using (OperationContextScope context = SetMessageHeaders(executionID))
			{
				return this.GetExecutionInfoAsync();
			}
		}
		public Task<RenderResponse> Render(string executionID, RenderRequest request)
		{
			using (OperationContextScope context = SetMessageHeaders(executionID))
			{
				return this.RenderAsync(request);
			}
		}

		public Task<Render2Response> Render2(string executionID, Render2Request request)
		{
			using (OperationContextScope context = SetMessageHeaders(executionID))
			{
				return this.Render2Async(request);
			}
		}

		public Task<ExecutionInfo> SetReportParameters(string executionID, IEnumerable<ParameterValue> parameterValues, string parameterLanguage)
		{
			using (OperationContextScope context = SetMessageHeaders(executionID))
			{
				ParameterValue[] parameterValuesArray = parameterValues.ToArray();
				if (parameterLanguage == null || parameterLanguage == "")
				{
					parameterLanguage = System.Globalization.CultureInfo.CurrentUICulture.Name;
				}

				return this.SetExecutionParametersAsync(parameterValuesArray, parameterLanguage);
			}
		}

		private OperationContextScope SetMessageHeaders(string executionID)
		{
			OperationContextScope context = new OperationContextScope(this.InnerChannel);

			JustCSharp.ReportViewer.ReportServiceExecution.ExecutionHeader executionHeaderData = new JustCSharp.ReportViewer.ReportServiceExecution.ExecutionHeader()
			{
				ExecutionID = executionID,
				//ExecutionIDForWcfSoapHeader = executionID
			};

#if true
			// add the ExecutionHeader entry to the soap headers
			OperationContext.Current.OutgoingMessageHeaders.Add(executionHeaderData.CreateMessageHeader());
#else
				// this does not appear to affect the soap headers
				OperationContext.Current.OutgoingMessageProperties.Add(ExecutionHeader.HeaderName, executionHeaderData);
#endif

			return context;
		}
	}
}
