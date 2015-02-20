using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DocServiceContract;
using Microsoft.SharePoint.Client;
using MCS.Library.SOA.DocServiceContract.Exceptions;
using MCS.Library.Core;
using MCS.Library.Services.Converters;
using MCS.Library.CamlBuilder;

namespace MCS.Library.Services
{
	/// <summary>
	/// 版本管理实现
	/// </summary>
	public partial class DCSStorageService
	{
		public void DCMCheckIn(int fileId, string comment, MCS.Library.SOA.DocServiceContract.DCTCheckinType checkInType)
		{
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				File file = getFileById(fileId, context);
				file.CheckIn(comment, (CheckinType)checkInType);
				context.ExecuteQuery();

			}
		}

		//TODO:重复功能
		private static File getFileById(int fileId, DocLibContext context)
		{
			ListItem listItem = GetListItemById(fileId, context);
			(listItem == null).TrueThrow<TargetNotFoundException>("文件(ID为{0})未找到。", fileId);
			return listItem.File;
		}

		public void DCMCheckOut(int fileId)
		{
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				File file = getFileById(fileId, context);
				file.CheckOut();
				context.ExecuteQuery();
			}
		}

		public void DCMUndoCheckOut(int fileId)
		{
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				File file = getFileById(fileId, context);
				file.UndoCheckOut();
				context.ExecuteQuery();
			}
		}

		public BaseCollection<DCTFileVersion> DCMGetVersions(int fileId)
		{
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				File file = getFileById(fileId, context);
				context.Load(file.Versions);
				context.ExecuteQuery();

				BaseCollection<DCTFileVersion> results = new BaseCollection<DCTFileVersion>();

				foreach (FileVersion version in file.Versions)
				{
					context.Load(version.CreatedBy);
					context.ExecuteQuery();
					DCTFileVersion dctFileVersion = new DCTFileVersion();
					dctFileVersion.AbsoluteUri = UriHelper.CombinePath(context.Url, version.Url);
					DCTConverterHelper.Convert(version, dctFileVersion);

					results.Add(dctFileVersion);
				}

				return results;
			}
		}

		public byte[] DCMGetVersionFileContent(int fileId, int versionId)
		{
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				File file = getFileById(fileId, context);

				return getVersionContetById(file, versionId, context);
			}
		}

		private byte[] getVersionContetById(File file, int versionId, DocLibContext context)
		{
			context.Load(file.Versions);
			context.ExecuteQuery();
			FileVersion selectedVersion = null;
			foreach (FileVersion version in file.Versions)
			{
				if (version.ID == versionId)
				{
					selectedVersion = version;
					break;
				}

			}
			(null == selectedVersion).TrueThrow<TargetNotFoundException>("版本(ID={0})未找到。", versionId);

			return context.OpenBinary(selectedVersion.Url);
		}
	}
}