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
	public partial class DCSStorageService
	{

		public DCTFile DCMSave(DCTFolder folder, byte[] fileData, string filename, bool overwrite)
		{
			(null == folder).TrueThrow<ArgumentException>("文件夹对象为空.");
			(string.IsNullOrEmpty(folder.Uri) && (!folder.IsRootFolder) && folder.ID < 0).TrueThrow<ArgumentException>("文件夹无效.请提供ID或Uri");

			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				string uri = LoadUri(folder, context);

				using (System.IO.MemoryStream stream = new System.IO.MemoryStream(fileData))
				{
					stream.Seek(0, System.IO.SeekOrigin.Begin);
					string fileUri = uri;

					if (!fileUri.EndsWith("/"))
						fileUri += "/";

					fileUri += filename;

					context.ExecuteQuery();
					File.SaveBinaryDirect(context, fileUri, stream, overwrite);

					File file = context.Web.GetFileByServerRelativeUrl(fileUri);
					context.Load(file);
					context.ExecuteQuery();
					context.Load(file.Author);

					if (file.CheckOutType != CheckOutType.None)
						context.Load(file.CheckedOutByUser);

					context.Load(file.ListItemAllFields);
					context.Load(file.ModifiedBy);
					context.ExecuteQuery();

					DCTFile dctFile = new DCTFile();

					Uri target = new Uri(fileUri, UriKind.RelativeOrAbsolute);
					Uri refUri = new Uri(context.Url);


					dctFile.AbsoluteUri = target.MakeAbsolute(refUri).ToString(); UriHelper.CombinePath(context.Url, fileUri);
					DCTConverterHelper.Convert(file, dctFile);

					return dctFile;
				}
			}
		}

		private ListItem LoadItem(DCTStorageObject folder, DocLibContext context)
		{
			if (folder is DCTFolder)
			{
				if ((folder as DCTFolder).IsRootFolder)
				{
					context.Load(context.BaseList.RootFolder);
					context.ExecuteQuery();

					return null;
				}
			}

			if (folder.Uri.IsNotEmpty())
			{
				return GetListItemByUri(folder.Uri, context);
			}

			return GetListItemById(folder.ID, context);
		}

		private ListItem GetListItemByUri(string uri, DocLibContext context)
		{
			CamlQuery query = new CamlQuery();
			query.ViewXml = Caml.SimpleView(Caml.Field("FileRef").Eq(Caml.Value("Text", uri)), CamlBuilder.ViewType.RecursiveAll).ToCamlString();

			ListItemCollection itemCollection = context.BaseList.GetItems(query);
			context.Load(itemCollection);
			context.ExecuteQuery();

			if (itemCollection.Count == 0)
				return null;
			return itemCollection[0];
		}

		private string LoadUri(DCTStorageObject folder, DocLibContext context)
		{
			if (folder is DCTFolder)
			{
				if ((folder as DCTFolder).IsRootFolder)
				{
					context.Load(context.BaseList.RootFolder);
					context.ExecuteQuery();

					return context.BaseList.RootFolder.ServerRelativeUrl;
				}
			}

			if (folder.Uri.IsNotEmpty())
				return folder.Uri;

			ListItem listItem = GetListItemById(folder.ID, context);

			(listItem == null).TrueThrow<TargetNotFoundException>("文件或文件夹(ID={0})不存在。", folder.ID);


			return listItem["FileRef"].ToString();
		}

		public DCTFile DCMGetFileByUri(string uri)
		{
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				File file = context.Web.GetFileByServerRelativeUrl(uri);
				context.Load(file.ListItemAllFields);
				context.ExecuteQuery();
				DCTFile dctFile = new DCTFile();
				DCTConverterHelper.Convert(file.ListItemAllFields, dctFile);
				return dctFile;
			}
		}

		//TODO: GetxxxByID是否是个通用的逻辑？
		public DCTFile DCMGetFileById(int fileId)
		{
			(fileId <= 0).TrueThrow<ArgumentException>("ID({0})无效,请传入大于0的值.", fileId);

			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				string uri = LoadUri(new DCTFile() { ID = fileId }, context);
				File file = context.Web.GetFileByServerRelativeUrl(uri);

				context.Load(file);
				context.Load(file.Author);
				context.ExecuteQuery();

				if (file.CheckOutType != CheckOutType.None)
				{
					context.Load(file.CheckedOutByUser);
				}

				context.Load(file.ListItemAllFields);
				context.Load(file.ModifiedBy);
				context.ExecuteQuery();

				DCTFile dctFile = new DCTFile();
				dctFile.AbsoluteUri = UriHelper.CombinePath(context.Url, uri);
				DCTConverterHelper.Convert(file, dctFile);

				return dctFile;
			}
		}

		public DCTFile DCMGetFileInFolder(DCTFolder folder, string filename)
		{
			DCTFolder curFolder = null;
			if (!folder.Uri.IsNullOrEmpty())
			{
				curFolder = DCMGetFolderByUri(folder.Uri);
			}
			else
			{
				curFolder = DCMGetFolderById(folder.ID);
			}

			string uri = folder.Uri;
			string fileUri = UriHelper.CombinePath(uri, filename);
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				ListItem item = GetListItemByUri(fileUri, context);
				DCTFile dctFile = new DCTFile();
				DCTConverterHelper.Convert(item, dctFile);
				return dctFile;
			}

		}

		public byte[] DCMGetFileContent(int fileId)
		{
			(fileId <= 0).TrueThrow<ArgumentException>("ID({0})无效,请传入大于0的值.", fileId);

			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				string uri = LoadUri(new DCTFile() { ID = fileId }, context);

				return context.OpenBinary(uri);
			}
		}

		public void DCMSetFileFields(int fileId, BaseCollection<DCTFileField> fileFields)
		{
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				PreCheckFields(fileFields, context);
				string uri = LoadUri(new DCTFile() { ID = fileId }, context);
				File file = context.Web.GetFileByServerRelativeUrl(uri);
				context.Load(file.ListItemAllFields);

				foreach (DCTFileField fileField in fileFields)
				{
					file.ListItemAllFields[fileField.Field.InternalName] = fileField.FieldValue;
				}

				file.ListItemAllFields.Update();
				context.ExecuteQuery();
			}
		}

		private void PreCheckFields(BaseCollection<DCTFileField> fileFields, DocLibContext context)
		{
			(null == fileFields || fileFields.Count == 0).TrueThrow<ArgumentException>("Field集合为空或元素个数为0");

			foreach (DCTFileField fileField in fileFields)
			{
				(null == fileField.Field).TrueThrow<ArgumentException>("Field元素为空。");
				(string.IsNullOrEmpty(fileField.Field.Title) && string.IsNullOrEmpty(fileField.Field.InternalName) && string.IsNullOrEmpty(fileField.Field.ID)).TrueThrow<ArgumentException>("要设置文件夹的Field,请提供ID、Internal Name或Title.");

				if (string.IsNullOrEmpty(fileField.Field.InternalName))
				{
					Field field = null;

					if (!string.IsNullOrEmpty(fileField.Field.ID))
						field = context.BaseList.Fields.GetById(new Guid(fileField.Field.ID));
					else
						if ((!string.IsNullOrEmpty(fileField.Field.Title)))
							field = context.BaseList.Fields.GetByTitle(fileField.Field.Title);

					context.Load(field);
					context.ExecuteQuery();
					fileField.Field.InternalName = field.InternalName;
				}
			}
		}

		public BaseCollection<DCTFileField> DCMGetAllFileFields(int fileId)
		{
			(fileId <= 0).TrueThrow<ArgumentException>("ID({0})无效,请传入大于0的值.", fileId);

			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				string uri = LoadUri(new DCTFile() { ID = fileId }, context);
				File file = context.Web.GetFileByServerRelativeUrl(uri);
				BaseCollection<DCTFileField> results = BuildFileFields(context, context.BaseList.Fields, file);

				return results;
			}
		}

		public BaseCollection<DCTFileField> DCMGetFileFields(int fileId, BaseCollection<DCTFieldInfo> fields)
		{
			(fileId <= 0).TrueThrow<ArgumentException>("ID({0})无效,请传入大于0的值.", fileId);

			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				List<Field> spfields = GetSPFields(fields, context);

				string uri = LoadUri(new DCTFile() { ID = fileId }, context);
				File file = context.Web.GetFileByServerRelativeUrl(uri);

				return BuildFileFields(context, spfields, file);
			}
		}

		private static BaseCollection<DCTFileField> BuildFileFields(DocLibContext context, IEnumerable<Field> spfields, File file)
		{
			BaseCollection<DCTFileField> results = new BaseCollection<DCTFileField>();
			context.Load(file.ListItemAllFields);
			context.Load(context.BaseList.Fields);
			context.ExecuteQuery();

			foreach (Field field in spfields)
			{
				DCTFileField fileField = new DCTFileField();
				DCTFieldInfo fieldInfo = new DCTFieldInfo();

				DCTConverterHelper.Convert(field, fieldInfo);

				if (string.IsNullOrEmpty(fieldInfo.ID))
					continue;

				fileField.Field = fieldInfo;
				fileField.FieldValue = file.ListItemAllFields[field.InternalName] == null ? "" : file.ListItemAllFields[field.InternalName].ToString();

				results.Add(fileField);
			}

			return results;
		}

		private static List<Field> GetSPFields(BaseCollection<DCTFieldInfo> fields, DocLibContext context)
		{
			(null == fields || fields.Count == 0).TrueThrow<ArgumentException>("Field集合为空或元素个数为0");

			List<Field> results = new List<Field>();

			foreach (DCTFieldInfo field in fields)
			{
				Field spfield = null;

				if (!string.IsNullOrEmpty(field.Title))
					spfield = context.BaseList.Fields.GetByTitle(field.Title);
				else
					if ((!string.IsNullOrEmpty(field.InternalName)))
						spfield = context.BaseList.Fields.GetByInternalNameOrTitle(field.InternalName);
					else
						if ((!string.IsNullOrEmpty(field.ID)))
							spfield = context.BaseList.Fields.GetById(new Guid(field.ID));

				context.Load(spfield);
				results.Add(spfield);
			}

			context.ExecuteQuery();

			return results;
		}
	}
}