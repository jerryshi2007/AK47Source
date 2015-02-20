using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace MCS.Library.Services
{
	internal static class ServiceHelper
	{
		internal static string GetDocumentLibraryName()
		{
			return GetIncomingMessageTag("libraryName", GetConfiguration().DocumentLibraryName);
		}

		internal static MossServerInfoConfigurationElement GetConfiguration()
		{
			MossServerInfoConfigurationSettings section = MossServerInfoConfigurationSettings.GetConfig();

			string elementName = GetIncomingMessageTag("documentServer", "documentServer");

			return section.Servers[elementName];
		}

		private static string GetIncomingMessageTag(string headerName, string defaultValue)
		{
			string result = defaultValue;

			int index = OperationContext.Current.IncomingMessageHeaders.FindHeader(headerName, string.Empty);

			if (index >= 0)
			{
				result = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(index);
			}

			return result;
		}
	}
}