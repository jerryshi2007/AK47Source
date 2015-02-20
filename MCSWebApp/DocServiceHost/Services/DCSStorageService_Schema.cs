using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DocServiceContract;
using Microsoft.SharePoint.Client;
using MCS.Library.SOA.DocServiceContract.Exceptions;
using MCS.Library.Services.Converters;
using MCS.Library.Core;

namespace MCS.Library.Services
{
    /// <summary>
    /// 架构管理实现
    /// </summary>
    public partial class DCSStorageService
    {
        public DCTFieldInfo DCMAddField(MCS.Library.SOA.DocServiceContract.DCTFieldInfo field, bool raiseErrorWhenExist)
        {
            ((field.Title).IsNullOrEmpty()).TrueThrow<ArgumentException>("请设置Field的Title.");

			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {

                if (FieldExists(field, context) && raiseErrorWhenExist)
                {
                    throw new FieldAlreadyExistException();
                }

                Field spField = context.BaseList.Fields.AddFieldAsXml("<Field Type=\"" + field.ValueType.ToString() + "\" DisplayName=\"" + field.Title + "\" Name=\"" + field.Title + "\"/>", true, AddFieldOptions.AddFieldToDefaultView);

                DCTConverterHelper.Convert(field, spField);

                context.Load(spField);
                context.ExecuteQuery();

                field.ID = spField.Id.ToString();
                field.InternalName = spField.InternalName;

                return field;
            }
        }

        private bool FieldExists(DCTFieldInfo field, DocLibContext context)
        {
            context.Load(context.BaseList.Fields);
            context.ExecuteQuery();
            bool found = false;

            foreach (Field spField in context.BaseList.Fields)
            {
                if (spField.Title == field.Title)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        public BaseCollection<DCTFieldInfo> DCMGetFields()
        {
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                context.Load(context.BaseList.Fields);
                context.ExecuteQuery();
                BaseCollection<DCTFieldInfo> result = new BaseCollection<DCTFieldInfo>();

                foreach (Field field in context.BaseList.Fields)
                {
                    DCTFieldType fieldType;
                    if (!Enum.TryParse<DCTFieldType>(field.TypeAsString, out fieldType))
                        continue;
                    DCTFieldInfo fieldInfo = new DCTFieldInfo();
                    DCTConverterHelper.Convert(field, fieldInfo);

                    result.Add(fieldInfo);
                }

                return result;
            }
        }

        public void DCMDeleteField(DCTFieldInfo field)
        {
            (string.IsNullOrEmpty(field.InternalName) && string.IsNullOrEmpty(field.Title) && string.IsNullOrEmpty(field.ID)).TrueThrow<ArgumentException>("请设置Field的Title、Internal Name或ID.");

            using (DocLibContext context = new DocLibContext())
            {
                Field spField = null;

                if (!string.IsNullOrEmpty(field.Title))
                    spField = context.BaseList.Fields.GetByTitle(field.Title);
                else
                    if (!string.IsNullOrEmpty(field.InternalName))
                        spField = context.BaseList.Fields.GetByInternalNameOrTitle(field.InternalName);
                    else
                        if (!string.IsNullOrEmpty(field.ID))
                            spField = context.BaseList.Fields.GetById(new Guid(field.ID));
                        else
                            throw new TargetNotFoundException();

                context.Load(spField);
                context.ExecuteQuery();

                spField.DeleteObject();
                context.BaseList.Update();

                context.Load(spField);
                context.ExecuteQuery();
                //context.BaseList.Fields.
            }
        }
    }
}