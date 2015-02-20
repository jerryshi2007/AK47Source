using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Reflection;

using MCS.Web.Library.Script;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// OguObject对象序列化反序列化
    /// </summary>
    public class OguObjectConverter : JavaScriptConverter
    {
        /// <summary>
        ///反序列化OguObject
        /// </summary>
        /// <param name="dictionary">对象类型</param>
        /// <param name="type">对象类型</param>
        /// <param name="serializer">JS序列化器</param>
        /// <returns>反序列化出的对象</returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            SchemaType oguType;
            OguBase result;
            ExtractOguObject(dictionary, out oguType, out result);

            switch (oguType)
            {
                case SchemaType.Organizations:
                    ((OguOrganization)result).CustomsCode = DictionaryHelper.GetValue(dictionary, "customsCode", string.Empty);
                    ((OguOrganization)result).DepartmentClass = DictionaryHelper.GetValue(dictionary, "departmentClass", DepartmentClassType.Unspecified);
                    ((OguOrganization)result).DepartmentType = DictionaryHelper.GetValue(dictionary, "departmentType", DepartmentTypeDefine.Unspecified);
                    ((OguOrganization)result).Rank = DictionaryHelper.GetValue(dictionary, "rank", DepartmentRankType.None);
                    ((OguOrganization)result).ExcludeVirtualDepartment = DictionaryHelper.GetValue(dictionary, "excludeVirtualDepartment", false);
                    break;
                case SchemaType.Users:
                    ((OguUser)result).Email = DictionaryHelper.GetValue(dictionary, "email", string.Empty);
                    ((OguUser)result).IsSideline = DictionaryHelper.GetValue(dictionary, "isSideline", false);
                    ((OguUser)result).LogOnName = DictionaryHelper.GetValue(dictionary, "logOnName", string.Empty);
                    ((OguUser)result).Occupation = DictionaryHelper.GetValue(dictionary, "occupation", string.Empty);
                    ((OguUser)result).Rank = DictionaryHelper.GetValue(dictionary, "rank", UserRankType.Unspecified);
                    break;
                case SchemaType.Groups:
                    break;
            }

            return result;
        }

        protected virtual void ExtractOguObject(IDictionary<string, object> dictionary, out SchemaType oguType, out OguBase result)
        {
            string id = (string)dictionary["id"];
            oguType = SchemaType.Users;

            if (dictionary.ContainsKey("objectType"))
                oguType = (SchemaType)dictionary["objectType"];

            result = CreateOguObject(oguType, id);

            result.Description = DictionaryHelper.GetValue(dictionary, "description", string.Empty);
            result.DisplayName = DictionaryHelper.GetValue(dictionary, "displayName", string.Empty);
            result.FullPath = DictionaryHelper.GetValue(dictionary, "fullPath", string.Empty);
            result.GlobalSortID = DictionaryHelper.GetValue(dictionary, "globalSortID", string.Empty);
            result.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
            result.SortID = DictionaryHelper.GetValue(dictionary, "sortID", string.Empty);
            result.Tag = DictionaryHelper.GetValue(dictionary, "tag", string.Empty);
            result.ClientContext = DictionaryHelper.GetValue(dictionary, "clientContext", (Dictionary<string, object>)null);

            if (dictionary.ContainsKey("status"))
                result.Properties["STATUS"] = DictionaryHelper.GetValue(dictionary, "status", 1);
        }

        protected virtual OguBase CreateOguObject(SchemaType oguType, string id)
        {
            return (OguBase)OguBase.CreateWrapperObject(id, oguType);
        }

        /// <summary>
        /// 序列化OguOrganization
        /// </summary>
        /// <param name="obj">material对象</param>
        /// <param name="serializer">序列化器</param>
        /// <returns>属性集合</returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            try
            {
                OguBase oguObj = (OguBase)obj;

                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "id", oguObj.ID);
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "name", oguObj.Name);
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "__type", obj.GetType().AssemblyQualifiedName);
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "displayName", oguObj.DisplayName);
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "objectType", GetObjectSchemaType(oguObj));
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "description", oguObj.Description);
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "fullPath", oguObj.FullPath);
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "globalSortID", oguObj.GlobalSortID);
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "levels", oguObj.Levels);
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "sortID", oguObj.SortID);
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "status", oguObj.Properties["STATUS"]);
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "tag", oguObj.Tag);

                dictionary.Add("clientContext", oguObj.ClientContext);

                switch (oguObj.ObjectType)
                {
                    case SchemaType.Organizations:
                        DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "customsCode", ((IOrganization)oguObj).CustomsCode);
                        DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "departmentClass", ((IOrganization)oguObj).DepartmentClass);
                        DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "departmentType", ((IOrganization)oguObj).DepartmentType);
                        DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "rank", ((IOrganization)oguObj).Rank);
                        DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "excludeVirtualDepartment", ((IVirtualOrganization)oguObj).ExcludeVirtualDepartment, false);
                        break;
                    case SchemaType.Users:
                        DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "email", ((IUser)oguObj).Email);
                        DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "isSideline", ((IUser)oguObj).IsSideline);
                        DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "logOnName", ((IUser)oguObj).LogOnName);
                        DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "occupation", ((IUser)oguObj).Occupation);
                        DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "rank", ((IUser)oguObj).Rank);
                        break;
                    case SchemaType.Groups:
                        break;
                }
            }
            catch (System.Exception)
            {
            }

            return dictionary;
        }

        private static SchemaType GetObjectSchemaType(IOguObject obj)
        {
            SchemaType type = obj.ObjectType;

            if (type == SchemaType.Unspecified)
            {
                if (obj is IUser)
                    type = SchemaType.Users;
                else
                    if (obj is IOrganization)
                        type = SchemaType.Organizations;
                    else
                        if (obj is IGroup)
                            type = SchemaType.Groups;
            }

            return type;
        }

        /// <summary>
        /// 获取此Converter支持的类别
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(OguBase));
                types.Add(typeof(OguGroup));
                types.Add(typeof(OguOrganization));
                types.Add(typeof(OguUser));

                return types;
            }
        }
    }
}
