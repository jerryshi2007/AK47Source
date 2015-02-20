using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security
{
    /// <summary>
    /// 表示对象及其关系的相关信息
    /// </summary>
    [Serializable]
    public class SCObjectAndRelation
    {
        [NonSerialized]
        private SchemaObjectBase _Detail = null;
        private string _FullPath;

        /// <summary>
        /// 获取或设置ID
        /// </summary>
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置显示名称
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置代码名称
        /// </summary>
        public string CodeName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置父级ID
        /// </summary>
        public string ParentID
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置内部排序号
        /// </summary>
        public int InnerSort
        {
            get;
            set;
        }

        /// <summary>
        /// 全局排序号
        /// </summary>
        public string GlobalSort
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置一个值，表示关系是否默认关系
        /// </summary>
        [ORFieldMapping("IsDefault")]
        public bool Default
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置<see cref="SchemaObjectStatus"/>值之一，表示关系对象的状态
        /// </summary>
        public SchemaObjectStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置一个字符串，表示模式类型
        /// </summary>
        public string SchemaType
        {
            get;
            set;
        }

        [NoMapping]
        public SchemaObjectBase Detail
        {
            get
            {
                return this._Detail;
            }
            internal set
            {
                this._Detail = value;
            }
        }

        /// <summary>
        /// 获取或设置全路径
        /// </summary>
        public string FullPath
        {
            get { return this._FullPath; }
            set { this._FullPath = value; }
        }

        public void FillDetail()
        {
            this.Detail = SchemaObjectAdapter.Instance.Load(this.ID);
        }

        /// <summary>
        /// 得到根对象的实例
        /// </summary>
        /// <returns></returns>
        public static SCObjectAndRelation GetRoot()
        {
            SCOrganization root = SCOrganization.GetRoot();
            SCRelationObject rootRelation = SCOrganization.GetRootRelationObject();

            SCObjectAndRelation result = new SCObjectAndRelation();

            result.ID = root.ID;
            result.Name = root.Name;
            result.CodeName = root.CodeName;
            result.DisplayName = root.DisplayName;
            result.SchemaType = root.SchemaType;
            result.Status = root.Status;

            result.Default = rootRelation.Default;
            result.InnerSort = rootRelation.InnerSort;
            result.GlobalSort = rootRelation.GlobalSort;
            result.ParentID = rootRelation.ParentID;
            result.FullPath = rootRelation.FullPath;

            return result;
        }
    }

    /// <summary>
    /// 表示<see cref="SCObjectAndRelation"/>对象的集合。
    /// </summary>
    [Serializable]
    public class SCObjectAndRelationCollection : EditableDataObjectCollectionBase<SCObjectAndRelation>
    {
        /// <summary>
        /// 从DataView加载
        /// </summary>
        /// <param name="view"></param>
        public void LoadFromDataView(DataView view)
        {
            LoadFromDataView(view, false);
        }

        /// <summary>
        /// 从DataView加载，删除相同的对象。主要针对于人员兼职情况，有主职保留主职，没有仅保留一个兼职
        /// </summary>
        /// <param name="view"></param>
        /// <param name="removeDuplicateData"></param>
        public void LoadFromDataView(DataView view, bool removeDuplicateData)
        {
            view.NullCheck("view");

            this.Clear();

            if (removeDuplicateData)
                LoadDataViewRemoveDuplicateData(view);
            else
                ORMapping.DataViewToCollection(this, view);
        }

        private void LoadDataViewRemoveDuplicateData(DataView view)
        {
            Dictionary<string, int> indexDict = new Dictionary<string, int>(view.Count);

            int index = 0;

            foreach (DataRowView drv in view)
            {
                SCObjectAndRelation data = new SCObjectAndRelation();

                ORMapping.DataRowToObject(drv.Row, data);

                int existsIndex = -1;
                bool replacedOrIgnored = false;

                if (indexDict.TryGetValue(data.ID, out existsIndex))
                {
                    //如果当前数据是主职，已存在的是兼职，则替换掉已存在的数据，否则忽略掉
                    if (data.Default && this[existsIndex].Default == false)
                        this[existsIndex] = data;

                    replacedOrIgnored = true;
                }

                if (replacedOrIgnored == false)
                {
                    this.Add(data);
                    indexDict.Add(data.ID, index);
                    index++;
                }
            }
        }

        /// <summary>
        /// 根据集合中的ID查询出完整的对象信息
        /// </summary>
        /// <returns></returns>
        public SchemaObjectCollection ToSchemaObjects()
        {
            InSqlClauseBuilder builder = new InSqlClauseBuilder("ID");

            this.ForEach(obj => builder.AppendItem(obj.ID));

            return SchemaObjectAdapter.Instance.Load(builder);
        }

        public void FillDetails()
        {
            SchemaObjectCollection allObjs = ToSchemaObjects();

            this.ForEach(sor =>
            {
                sor.Detail = allObjs[sor.ID];

                if (sor.Detail == null)
                {
                    if (string.Compare(SCOrganization.RootOrganizationID, sor.ID, true) == 0)
                        sor.Detail = SCOrganization.GetRoot();
                    else
                        throw new ObjectNotFoundException("Can not find matching detail of object (ID:" + sor.ID + "), there may have data errors");
                }
            });
        }

        public string[] ToParentIDArray()
        {
            List<string> result = new List<string>(this.Count);

            this.ForEach(s => result.Add(s.ParentID));

            return result.ToArray();
        }

        public string[] ToIDArray()
        {
            List<string> result = new List<string>(this.Count);

            this.ForEach(s => result.Add(s.ID));

            return result.ToArray();
        }
    }
}
