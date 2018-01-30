namespace AlanJuden.MvcReportViewer.NetCore2
{
	public enum ReportFormats
	{
		[EnumName("")]
		NotSet,

		/// <summary>
		/// XML file with report data
		/// </summary>
		[EnumName("XML")]
		Xml,

		/// <summary>
		/// CSV (comma delimited)
		/// </summary>
		[EnumName("CSV")]
		Csv,

		/// <summary>
		/// Data Feed (rss)
		/// </summary>
		[EnumName("ATOM")]
		Atom,

		/// <summary>
		/// PDF
		/// </summary>
		[EnumName("PDF")]
		Pdf,

		/// <summary>
		/// Remote GDI+ file
		/// </summary>
		[EnumName("RGDI")]
		Rgdi,

		/// <summary>
		/// HTML 4.0
		/// </summary>
		[EnumName("HTML4.0")]
		Html4_0,

		/// <summary>
		/// MHTML (web archive)
		/// </summary>
		[EnumName("MHTML")]
		Mhtml,

		/// <summary>
		/// Excel 2003 (.xls)
		/// </summary>
		[EnumName("EXCEL")]
		Excel,

		/// <summary>
		/// Excel (.xlsx)
		/// </summary>
		[EnumName("EXCELOPENXML")]
		ExcelOpenXml,

		/// <summary>
		/// RPL Renderer
		/// </summary>
		[EnumName("RPL")]
		Rpl,

		/// <summary>
		/// TIFF file
		/// </summary>
		[EnumName("IMAGE")]
		Image,

		/// <summary>
		/// Word 2003 (.doc)
		/// </summary>
		[EnumName("WORD")]
		Word,

		/// <summary>
		/// Word (.docx)
		/// </summary>
		[EnumName("WORDOPENXML")]
		WordOpenXml,
	}

	public enum ReportViewModes
	{
		Export,
		Print,
		View
	}
}
