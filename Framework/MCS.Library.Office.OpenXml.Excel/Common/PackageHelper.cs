using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.Xml.Linq;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal static class PackageHelper
	{
		public static XDocument GetXDocumentFromUri(this Package document, Uri uriPart)
		{
			PackagePart packPart = document.GetPart(uriPart);
			return XDocument.Load(packPart.GetStream());
		}

		public static XElement GetXElementFromUri(this Package document, Uri uriPart)
		{
			XDocument result = GetXDocumentFromUri(document, uriPart);
			return result.Root;
		}
	}
}
