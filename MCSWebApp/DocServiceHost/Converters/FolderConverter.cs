using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using Microsoft.SharePoint.Client;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Services.Converters
{
    public class FolderConverter : DataTypeConverterGenericBase<Folder, DCTFolder>
    {
        protected override void GenericConvert(Folder srcObject, DCTFolder targetObject)
        {
            DCTConverterHelper.Convert<ClientObject, DCTStorageObject>(srcObject, targetObject);
        }
    }

    public class FileConverter : DataTypeConverterGenericBase<File, DCTFile>
    {
        protected override void GenericConvert(File srcObject, DCTFile targetObject)
        {
            if (null == targetObject.Author)
                targetObject.Author = new DCTUser();
            if (srcObject.CheckOutType != CheckOutType.None && null == targetObject.CheckedOutBy)
                targetObject.CheckedOutBy = new DCTUser();
            targetObject.Created = srcObject.TimeCreated;
            targetObject.ID = srcObject.ListItemAllFields.Id;
            targetObject.MajorVersion = srcObject.MajorVersion;
            targetObject.MinorVersion = srcObject.MinorVersion;
            targetObject.Modified = srcObject.TimeLastModified;
            targetObject.Name = srcObject.Name;
            targetObject.Uri = srcObject.ServerRelativeUrl;
            targetObject.CheckOutType = (DCTCheckOutType)((int)srcObject.CheckOutType);

            DCTConverterHelper.Convert(srcObject.Author, targetObject.Author);
            if (null == targetObject.ModifiedBy)
                targetObject.ModifiedBy = new DCTUser();
            DCTConverterHelper.Convert(srcObject.ModifiedBy, targetObject.ModifiedBy);
            if (srcObject.CheckOutType != CheckOutType.None)
            {
                DCTConverterHelper.Convert(srcObject.CheckedOutByUser, targetObject.CheckedOutBy);
            }

        }
    }

    public class FieldConverter : DataTypeConverterGenericBase<Field, DCTFieldInfo>
    {
        protected override void GenericConvert(Field srcObject, DCTFieldInfo targetObject)
        {
            DCTFieldType fieldType;
            if (!Enum.TryParse<DCTFieldType>(srcObject.TypeAsString, out fieldType))
                return;

            targetObject.DefaultValue = srcObject.DefaultValue;
            targetObject.ID = srcObject.Id.ToString();
            targetObject.InternalName = srcObject.InternalName;
            targetObject.Title = srcObject.Title;
            targetObject.ValueType = fieldType;
            targetObject.Required = srcObject.Required;
            targetObject.ValidationFormula = srcObject.ValidationFormula;
            targetObject.ValidationMessage = srcObject.ValidationMessage;

        }
    }

    public class ReverseFieldConverter : DataTypeConverterGenericBase<DCTFieldInfo, Field>
    {
        protected override void GenericConvert(DCTFieldInfo srcObject, Field targetObject)
        {
            targetObject.DefaultValue = srcObject.DefaultValue;
            targetObject.Required = srcObject.Required;
            targetObject.ValidationFormula = srcObject.ValidationFormula;
            targetObject.ValidationMessage = srcObject.ValidationMessage;
        }
    }

    public class FileVersionConverter : DataTypeConverterGenericBase<FileVersion, DCTFileVersion>
    {
        protected override void GenericConvert(FileVersion srcObject, DCTFileVersion targetObject)
        {
            targetObject.CheckInComment = srcObject.CheckInComment;
            targetObject.Created = srcObject.Created;
            targetObject.ID = srcObject.ID;
            targetObject.IsCurrentVersion = srcObject.IsCurrentVersion;
            targetObject.Uri = srcObject.Url;
            if (null == targetObject.CreatedBy)
                targetObject.CreatedBy = new DCTUser();
            DCTConverterHelper.Convert(srcObject.CreatedBy, targetObject.CreatedBy);


        }
    }
}