using System;

namespace AlanJuden.MvcReportViewer.NetCore2
{
	public class EnumNameAttribute : Attribute
	{
		public string Name { get; set; }

		public EnumNameAttribute(string name)
		{
			this.Name = name;
		}
	}
}
