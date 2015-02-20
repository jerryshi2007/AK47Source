using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.PermissionBridge.Adapters;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.PermissionBridge
{
    public static class BridgeExtensions
    {
		public static QueryByIDsAdapterBase<OguObjectCollection<T>> ToQueryAdapter<T>(this SearchOUIDType idType, string[] schemaTypes, string[] ids) where T : IOguObject
        {
			QueryByIDsAdapterBase<OguObjectCollection<T>> result = null;

            switch (idType)
            {
                case SearchOUIDType.Guid:
                    result = new QueryByGuidsAdapter<T>(schemaTypes, ids);
                    break;
                case SearchOUIDType.FullPath:
                    result = new QueryByFullPathsAdapter<T>(schemaTypes, ids);
                    break;
                case SearchOUIDType.LogOnName:
                    result = new QueryByCodeNamesAdapter<T>(schemaTypes, ids);
                    break;
                default:
                    throw new ArgumentException(string.Format("{0}不是可以处理的ID类型", idType), "idType");
            }

            return result;
        }

        public static string[] ToSchemaTypes(this Type type)
        {
            List<string> result = new List<string>();

            if (type == typeof(IUser))
                result.Add("Users");
            else
                if (type == typeof(IOrganization))
                    result.Add("Organizations");
                else
                    if (type == typeof(IGroup))
                        result.Add("Groups");

            return result.ToArray();
        }

        public static List<T> ConvertToOguObjects<T>(this SCObjectAndRelationCollection relations) where T : IOguObject
        {
            relations.FillDetails();

            IOguObjectFactory factory = OguPermissionSettings.GetConfig().OguObjectFactory;

            //Dictionary<string, SCSimpleObjectCollection> allParentsInfo = SCSnapshotAdapter.Instance.LoadAllParentsInfo(true, relations.ToParentIDArray());
            List<T> objList = new List<T>(relations.Count);

            foreach (SCObjectAndRelation relation in relations)
            {
                SchemaObjectBase obj = relation.Detail;

                if (obj != null)
                {
                    SchemaType schemaType = SchemaType.Unspecified;

                    if (Enum.TryParse(obj.SchemaType, out schemaType))
                    {
                        T opObj = (T)factory.CreateObject(schemaType);

                        //SCSimpleObjectCollection parentsInfo = null;


                        //allParentsInfo.TryGetValue(relation.ParentID, out parentsInfo);

                        OguPropertyAdapterBase.GetConverter(opObj).Fill(opObj, obj, relation);
                        ((IOguPropertyAccessible)opObj).FullPath = relation.FullPath;

                        objList.Add(opObj);
                    }
                }
            }

            return objList;
        }
    }
}
