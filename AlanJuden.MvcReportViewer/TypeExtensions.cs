using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlanJuden.MvcReportViewer
{
	public static class TypeExtensions
	{
		public static string[] COMMON_DELIMETERS = new string[] { ",", " ", "\r\n", "^", ";", "+", "\r", "\n", "\t" };

		public static string ToSafeString(this object value)
		{
			return ToSafeString(value, string.Empty);
		}

		public static string ToSafeString(object value, string defaultValue)
		{
			try
			{
				if (value == null)
				{
					return defaultValue;
				}
				else
				{
					return value.ToString();
				}
			}
			catch
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Takes ANY value whatsoever and makes it a usable string that has no leading or trailing whitespace.
		/// </summary>
		public static string SafeTrim(this object value)
		{
			return ToSafeString(value, string.Empty).Trim();
		}

		public static Boolean ToBoolean(this object value)
		{
			return ToBoolean(value, default(bool));
		}

		public static Boolean ToBoolean(object value, Boolean defaultValue)
		{
			try
			{
				if (value == null)
				{
					return defaultValue;
				}
				else
				{
					string val = value.ToString().ToUpper();
					//VB6 code set the True value to -1, check for this
					switch (val)
					{
						case "1":
						case "-1":
						case "T":
						case "TRUE":
						case "Y":
						case "CHECKED":
						case "ON":
							return true;
						case "0":
						case "F":
						case "FALSE":
						case "N":
						case "UNCHECKED":
						case "OFF":
							return false;
						default:
							return Convert.ToBoolean(value);
					}
				}
			}
			catch
			{
				return defaultValue;
			}
		}

		public static Int32 ToInt32(this object value)
		{
			return ToInt32(value, default(Int32));
		}

		public static Int32 ToInt32(object value, Int32 defaultValue)
		{
			try
			{
				if (value == null)
				{
					return defaultValue;
				}
				else
				{
					return Convert.ToInt32(value);
				}
			}
			catch
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Turns a string into a List<string>
		/// </summary>
		/// <param name="data">Data to be turned into a list</param>
		/// <returns></returns>
		public static List<string> ToStringList(this string data)
		{
			return ToStringList(data, COMMON_DELIMETERS);
		}

		/// <summary>
		/// Turns a string into a List<string>
		/// </summary>
		/// <param name="data">Data to be turned into a list</param>
		/// <param name="separators">List of separators you want to split your string on</param>
		/// <returns></returns>
		public static List<string> ToStringList(this string data, string[] separators)
		{
			var list = new List<string>();
			if (!string.IsNullOrEmpty(data))
			{
				var array = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);

				foreach (string token in array)
				{
					string asString = token.SafeTrim();
					if (!list.Contains(asString))
					{
						list.Add(asString);
					}
				}
			}
			return list;
		}

		public static string HtmlEncode(this string text)
		{
			return System.Web.HttpUtility.HtmlEncode(text.ToSafeString());
		}

		public static string UrlEncode(this string text)
		{
			return System.Web.HttpUtility.UrlEncode(text.ToSafeString());
		}

		public static string HtmlDecode(this string text)
		{
			return System.Web.HttpUtility.HtmlDecode(text.ToSafeString());
		}

		public static string UrlDecode(this string text)
		{
			return System.Web.HttpUtility.UrlDecode(text.ToSafeString());
		}

		public static bool HasValue(this string Value)
		{
			List<string> emptyList = new List<string>() { "", "N/A", "NA", "TBD" };

			return !string.IsNullOrEmpty(Value.SafeTrim())
				&& !emptyList.Contains(Value.SafeTrim().ToUpper());
		}

		public static string GetName(this Enum value)
		{
			var info = value.GetType().GetField(value.ToString());

			try
			{
				var attribs = (EnumNameAttribute[])info.GetCustomAttributes(typeof(EnumNameAttribute), false);

				if (attribs.Length > 0)
				{
					return attribs[0].Name;
				}
				else
				{
					return value.ToString();
				}
			}
			catch
			{
				return value.ToString();
			}
		}

		public static T ToEnum<T>(this String name)
		//where T : Enum
		{
			return (T)Enum.Parse(typeof(T), name);
		}


		public static T NameToEnum<T>(this String name)
		// where T : Enum
		{
			return (
				from val in Enum.GetNames(typeof(T)).AsEnumerable()
				let enumItem = Enum.Parse(typeof(T), val)
				where GetName((Enum)enumItem).Equals(name, StringComparison.InvariantCultureIgnoreCase)
				select (T)enumItem
				).Single();
		}
	}
}
