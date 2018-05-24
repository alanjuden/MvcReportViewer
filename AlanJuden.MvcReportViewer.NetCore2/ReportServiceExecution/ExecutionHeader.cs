using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace AlanJuden.MvcReportViewer.NetCore2.ReportServiceExecution
{
	/// <summary> 
	/// Functionality added to the Microsoft Sql Server Reporting Services 
	/// service reference to force the ExecutionHeader class to correctly 
	/// serialize the execution id into a soap header via wcf. 
	/// </summary> 
	[DataContract(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")] 
	public partial class ExecutionHeader
	{
		/// <summary>
		/// The local name of the header xml element.
		/// </summary>
		public static readonly string HeaderName = "ExecutionHeader";

		/// <summary>
		/// The namespace uri of the header xml element.
		/// </summary>
		public static readonly string HeaderNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices";

		/// <summary>
		/// Gets or sets the execution id for the ExecutionHeader class. 
		/// This dummy property forces the correct serialization of the ExecutionID 
		/// field into the soap header. 
		/// </summary> 
		[DataMember(Order = 0, Name = "ExecutionID")]
		public string ExecutionIDForWcfSoapHeader
		{
			get
			{
				return this.executionIDField;
			}
			set
			{
				this.executionIDField = value;
			}
		}

		/// <summary> 
		/// Creates a message header containing the ExecutionHeader suitable for 
		/// inclusion in a soap header. 
		/// </summary> 
		/// <returns>A MessageHeader representing the execution context.</returns> 
		public MessageHeader CreateMessageHeader()
		{
			MessageHeader executionHeader = MessageHeader.CreateHeader(HeaderName, HeaderNamespace, this);
			return executionHeader;
		}
	}
}