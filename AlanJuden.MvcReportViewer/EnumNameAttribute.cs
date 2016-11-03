using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlanJuden.MvcReportViewer
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
