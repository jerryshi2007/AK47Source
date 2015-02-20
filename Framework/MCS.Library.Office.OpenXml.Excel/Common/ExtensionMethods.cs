using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO.Packaging;
using System.IO;

namespace MCS.Library.Office.OpenXml.Excel
{
	public static class ExtensionMethods
	{
		public static bool ValidateSheetName(this string Name)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(Name, @":|\?|/|\\|\[|\]");
		}

		public static string SetAlignString(this Enum align)
		{
			string newName = Enum.GetName(align.GetType(), align);
			return newName.Substring(0, 1).ToLower() + newName.Substring(1, newName.Length - 1);
		}

		public static bool ConvertBool(this Nullable<bool> boolValue)
		{
			bool result = false;
			if (boolValue.HasValue)
			{
				return (bool)boolValue;
			}

			return result;
		}

	}

	public static class PackageExtensions
	{
		/// <summary>
		/// 根据url
		/// </summary>
		/// <param name="document"></param>
		/// <param name="uriPart"></param>
		/// <returns></returns>
		public static XmlDocument GetXmlFromUri(this Package document, Uri uriPart)
		{
			XmlDocument xlPart = new XmlDocument();
			PackagePart packPart = document.GetPart(uriPart);
			xlPart.Load(packPart.GetStream());

			return xlPart;
		}

		/// <summary>
		/// 根据Uri 和 Xml部件保存到Package
		/// </summary>
		/// <param name="document">保存到Package</param>
		/// <param name="uriPart"></param>
		/// <param name="xmlPart"></param>
		internal static void SavePart(this Package document, Uri uriPart, XmlDocument xmlPart)
		{
			PackagePart partPack = document.GetPart(uriPart);
			xmlPart.Save(partPack.GetStream(FileMode.Create, FileAccess.Write));
		}

		internal static Uri GetNewUri(this Package package, string sUri)
		{
			int id = 1;
			Uri uri;
			do
			{
				uri = new Uri(string.Format(sUri, id++), UriKind.Relative);
			}
			while (package.PartExists(uri));
			return uri;
		}
	}


}
