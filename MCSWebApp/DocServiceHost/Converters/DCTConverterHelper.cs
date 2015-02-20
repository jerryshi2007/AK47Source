using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using Microsoft.SharePoint.Client;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Services.Converters
{
	public static class DCTConverterHelper
	{
		static DCTConverterHelper()
		{
			RegisterAllConverters();
		}

		private static void RegisterAllConverters()
		{
			RegisterConverter(typeof(ClientObject), typeof(DCTStorageObject), new StorageObjectConverter());
			RegisterConverter(typeof(Folder), typeof(DCTFolder), new FolderConverter());

			RegisterConverter(typeof(Principal), typeof(DCTPrincipal), new PrincipalConverter());
			RegisterConverter(typeof(User), typeof(DCTUser), new UserConverter());
            RegisterConverter(typeof(Group), typeof(DCTGroup), new GroupConverter());

			RegisterConverter(typeof(ListItem), typeof(DCTStorageObject), new ListItemToStorageObjectConverter());
			RegisterConverter(typeof(ListItem), typeof(DCTFolder), new ListItemToFolderConverter());
			RegisterConverter(typeof(ListItem), typeof(DCTFile), new ListItemToFileConverter());

            RegisterConverter(typeof(FieldUserValue), typeof(DCTUser), new FieldUserValueConverter());

            RegisterConverter(typeof(RoleDefinition), typeof(DCTRoleDefinition),new RoleConverter());
            RegisterConverter(typeof(File),typeof(DCTFile),new FileConverter());
            RegisterConverter(typeof(Field),typeof(DCTFieldInfo),new FieldConverter());
            RegisterConverter(typeof(DCTFieldInfo),typeof(Field),new ReverseFieldConverter());
            RegisterConverter(typeof(FileVersion), typeof(DCTFileVersion), new FileVersionConverter());
            

		}

		public static void Convert<TSource, TTarget>(TSource srcObject, TTarget targetObject)
		{
			GetConverter(typeof(TSource), typeof(TTarget)).Convert(srcObject, targetObject);
		}

		public static void RegisterConverter(Type sourceType, Type destType, DataTypeConverterBase converter)
		{
			DataTypeConverterHelper.RegisterConverter(sourceType, destType, converter);
		}

		public static DataTypeConverterBase GetConverter(Type sourceType, Type destType)
		{
			return DataTypeConverterHelper.GetConverter(sourceType, destType);
		}
	}
}