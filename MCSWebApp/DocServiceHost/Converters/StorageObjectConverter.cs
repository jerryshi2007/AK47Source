using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DocServiceContract;
using Microsoft.SharePoint.Client;
using MCS.Library.Core;

namespace MCS.Library.Services.Converters
{
	public class StorageObjectConverter : DataTypeConverterGenericBase<ClientObject, DCTStorageObject>
	{
		protected override void GenericConvert(ClientObject srcObject, DCTStorageObject targetObject)
		{
			if (srcObject is Folder)
			{
				Folder folder = (Folder)srcObject;

				targetObject.Name = folder.Name;
				targetObject.Uri = folder.ServerRelativeUrl;
				targetObject.AbsoluteUri = UriHelper.CombinePath(DocLibContext.BaseUri.ToString(), folder.ServerRelativeUrl);
			}
			else
				if (srcObject is File)
				{
					File file = (File)srcObject;

					targetObject.Name = file.Name;
					targetObject.Uri = file.ServerRelativeUrl;
					targetObject.AbsoluteUri = UriHelper.CombinePath(DocLibContext.BaseUri.ToString(), file.ServerRelativeUrl);
				}
				else
					throw new SystemSupportException("参数srcObject的类型必须是File或者Folder");
		}
	}
}