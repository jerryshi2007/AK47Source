using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using Microsoft.SharePoint.Client;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Services.Converters
{
	public class ListItemToStorageObjectConverter : DataTypeConverterGenericBase<ListItem, DCTStorageObject>
	{
		protected override void GenericConvert(ListItem srcObject, DCTStorageObject targetObject)
		{
			targetObject.AbsoluteUri = UriHelper.CombinePath(DocLibContext.BaseUri.ToString(), srcObject.FieldValues["FileRef"].ToString());
			targetObject.ID = srcObject.Id;
			targetObject.Name = srcObject.FieldValues["FileLeafRef"].ToString();
			targetObject.Uri = srcObject.FieldValues["FileRef"].ToString();
		}
	}

	public class ListItemToFolderConverter : DataTypeConverterGenericBase<ListItem, DCTFolder>
	{
		protected override void GenericConvert(ListItem srcObject, DCTFolder targetObject)
		{
			DCTConverterHelper.Convert<ListItem, DCTStorageObject>(srcObject, targetObject);

            targetObject.IsRootFolder = srcObject.Id == 0 ? true : false;
		}
	}

	public class ListItemToFileConverter : DataTypeConverterGenericBase<ListItem, DCTFile>
	{
        private DCTCheckOutType GetFileCheckOutType(ListItem listItem)
        {
            DCTCheckOutType result;
            if (listItem.FieldValues["CheckoutUser"] == null)
            {
                result = DCTCheckOutType.None;
            }
            else
            {
                result = (DCTCheckOutType)(int.Parse(listItem.FieldValues["IsCheckedoutToLocal"].ToString()));
            }
            return result;
        }

		protected override void GenericConvert(ListItem srcObject, DCTFile targetObject)
		{
			//DCTConverterHelper.Convert<ListItem, DCTStorageObject>(srcObject, targetObject);

            DCTUser user= null;

			//DCTConverterHelper.Convert(srcObject.File.Author, user);

            //作者
            if (srcObject.FieldValues["Author"] != null)
            {
                user = new DCTUser();
                DCTConverterHelper.Convert((FieldUserValue)srcObject.FieldValues["Author"], user);
            }

			DCTUser checkOutUser = null;

            //if (srcObject.File.CheckOutType == CheckOutType.Online)
            //{
				checkOutUser = new DCTUser();
                //签出者
				//DCTConverterHelper.Convert(srcObject.File.CheckedOutByUser, checkOutUser);
                if (srcObject.FieldValues["CheckoutUser"] != null)
                {
                    checkOutUser = new DCTUser();
                    DCTConverterHelper.Convert((FieldUserValue)srcObject.FieldValues["CheckoutUser"], checkOutUser);
                }
            //}

            DCTUser modifyUser = null;
			//DCTConverterHelper.Convert(srcObject.File.ModifiedBy, modifyUser);
            //修改者
            if (srcObject.FieldValues["Editor"] != null)
            {
                modifyUser = new DCTUser();
                DCTConverterHelper.Convert((FieldUserValue)srcObject.FieldValues["Editor"], modifyUser);
            }

			targetObject.ID = srcObject.Id;
            targetObject.AbsoluteUri = UriHelper.CombinePath(DocLibContext.BaseUri.ToString(), srcObject.FieldValues["FileRef"].ToString());
			targetObject.Author = user;
			targetObject.CheckedOutBy = checkOutUser;
			targetObject.CheckOutType = GetFileCheckOutType(srcObject);//(DCTCheckOutType)srcObject.File.CheckOutType;
            targetObject.Created = (DateTime)srcObject.FieldValues["Created"];//srcObject.File.TimeCreated;
			targetObject.ID = srcObject.Id;
            double version = double.Parse(srcObject.FieldValues["_UIVersionString"].ToString());
            targetObject.MajorVersion = (int)version;
            targetObject.MinorVersion = (int)version%1;
            targetObject.Modified = (DateTime)srcObject.FieldValues["Modified"];
			targetObject.ModifiedBy = modifyUser;
            targetObject.Name = srcObject.FieldValues["FileLeafRef"].ToString();
            targetObject.Uri = srcObject.FieldValues["FileRef"].ToString();
		}
	}
}