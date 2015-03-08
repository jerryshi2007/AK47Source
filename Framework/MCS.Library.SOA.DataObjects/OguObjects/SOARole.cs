using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
    [Serializable]
    [XElementSerializable]
    public class SOARole : OguRole, IWfMatrixContainer
    {
        [NonSerialized]
        private object _RowsSyncObject = new object();

        [NonSerialized]
        private SOARolePropertyRowCollection _Rows = null;

        [NonSerialized]
        private object _PropertyDefinitionsSyncObject = new object();

        [NonSerialized]
        private SOARolePropertyDefinitionCollection _PropertyDefinitions = null;

        public SOARole()
        {
        }

        public SOARole(IRole role)
            : base(role)
        {
        }

        public SOARole(string key)
            : base(key)
        {
        }

        /// <summary>
        /// 通过预先定义好的列定义来构造
        /// </summary>
        /// <param name="definitions"></param>
        public SOARole(SOARolePropertyDefinitionCollection definitions)
        {
            this._PropertyDefinitions = definitions;
        }

        public new static IRole CreateWrapperObject(IRole role)
        {
            IRole result = role;

            if (role is SOARole == false)
                result = new SOARole(role);

            return result;
        }

        public SOARolePropertyDefinitionCollection PropertyDefinitions
        {
            get
            {
                if (this._PropertyDefinitions == null)
                {
                    lock (this._PropertyDefinitionsSyncObject)
                    {
                        if (this._PropertyDefinitions == null)
                            this._PropertyDefinitions = SOARolePropertyDefinitionAdapter.Instance.GetByRole(this);
                    }
                }

                return this._PropertyDefinitions;
            }

            set { this._PropertyDefinitions = value; }
        }

        public SOARolePropertyRowCollection Rows
        {
            get
            {
                if (this._Rows == null)
                {
                    lock (this._RowsSyncObject)
                    {
                        if (this._Rows == null)
                            this._Rows = SOARolePropertiesAdapter.Instance.GetByRole(this, this.PropertyDefinitions);
                    }
                }

                return this._Rows;
            }
        }

        /// <summary>
        /// 是否是活动矩阵
        /// </summary>
        public WfMatrixType MatrixType
        {
            get
            {
                return this.PropertyDefinitions.MatrixType;
            }
        }

        /// <summary>
        /// 得到角色中的所有对象
        /// </summary>
        public override OguObjectCollection<IOguObject> ObjectsInRole
        {
            get
            {
                OguDataCollection<IOguObject> objects = new OguDataCollection<IOguObject>();

                objects.CopyFrom(base.ObjectsInRole);
                objects.Sort(OrderByPropertyType.GlobalSortID, SortDirectionType.Ascending);

                OguDataCollection<IOguObject> objectsInMatrix = GetObjectsFromMatrix();

                objects.CopyFrom(objectsInMatrix);

                return new OguObjectCollection<IOguObject>(objects.ToArray());
            }
        }

        /// <summary>
        /// 从矩阵中获得对象。这主要取决于上下文中的参数
        /// </summary>
        /// <returns></returns>
        public OguDataCollection<IOguObject> GetObjectsFromMatrix()
        {
            OguDataCollection<IOguObject> result = new OguDataCollection<IOguObject>();

            SOARoleContext context = SOARoleContext.Current;

            if (context != null && context.QueryParams.Count > 0)
            {
                SOARolePropertyRowCollection matchedRows = this.Rows.Query(context.QueryParams);

                matchedRows = matchedRows.ExtractMatrixRows();
                foreach (SOARolePropertyRowUsers rowUsers in matchedRows.GenerateRowsUsers())
                {
                    foreach (IUser user in rowUsers.Users)
                    {
                        if (result.Contains(user) == false)
                            result.Add(user);
                    }
                }
            }

            return result;
        }

        public void FillCreateActivityParams(WfCreateActivityParamCollection capc, PropertyDefineCollection definedProperties)
        {
            SOARoleContext context = SOARoleContext.Current;

            if (context != null && context.QueryParams.Count > 0)
            {
                SOARolePropertyRowCollection matchedRows = this.Rows.Query(context.QueryParams);

                matchedRows = matchedRows.ExtractMatrixRows();

                matchedRows.FillCreateActivityParams(capc, definedProperties);
            }
        }

        /// <summary>
        /// 从DataTable进行构造
        /// </summary>
        /// <param name="table"></param>
        public void FromDataTable(DataTable table)
        {
            this.PropertyDefinitions.FromDataColumns(table.Columns);
            this.Rows.FromDataTable(table.Rows, this.PropertyDefinitions);
        }
    }
}
