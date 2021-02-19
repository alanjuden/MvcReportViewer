using System;

namespace JustCSharp.ReportViewer
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
