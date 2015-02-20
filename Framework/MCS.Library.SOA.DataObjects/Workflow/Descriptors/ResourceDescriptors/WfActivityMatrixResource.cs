using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 表示活动矩阵的资源
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class WfActivityMatrixResourceDescriptor : WfResourceDescriptor, IWfCreateActivityParamsGenerator
    {
        public static readonly WfActivityMatrixResourceDescriptor EmptyInstance = new WfActivityMatrixResourceDescriptor();

        private SOARolePropertyDefinitionCollection _PropertyDefinitions = null;
        private SOARolePropertyRowCollection _Rows = null;

        /// <summary>
        /// 
        /// </summary>
        public WfActivityMatrixResourceDescriptor()
        {
        }

        /// <summary>
        /// 从DataTable构造
        /// </summary>
        /// <param name="table"></param>
        public WfActivityMatrixResourceDescriptor(DataTable table)
        {
            this.FromDataTable(table);
        }

        /// <summary>
        /// 矩阵的列定义
        /// </summary>
        public SOARolePropertyDefinitionCollection PropertyDefinitions
        {
            get
            {
                if (this._PropertyDefinitions == null)
                    this._PropertyDefinitions = new SOARolePropertyDefinitionCollection();

                return this._PropertyDefinitions;
            }
        }

        /// <summary>
        /// 矩阵的行
        /// </summary>
        public SOARolePropertyRowCollection Rows
        {
            get
            {
                if (this._Rows == null)
                    this._Rows = new SOARolePropertyRowCollection();

                return this._Rows;
            }
        }

        protected internal override void FillUsers(OguDataCollection<IUser> users)
        {
            SOARoleContext.DoAction(this.PropertyDefinitions, this.ProcessInstance, (context) =>
            {
                SOARolePropertyRowCollection matchedRows = this.Rows.Query(context.QueryParams);

                matchedRows = matchedRows.ExtractMatrixRows();
                foreach (SOARolePropertyRowUsers rowUsers in matchedRows.GenerateRowsUsers())
                {
                    foreach (IUser user in rowUsers.Users)
                    {
                        if (users.Contains(user) == false)
                            users.Add(user);
                    }
                }
            });
        }

        protected override void ToXElement(XElement element)
        {
        }

        public void Fill(WfCreateActivityParamCollection capc, PropertyDefineCollection definedProperties)
        {
            SOARoleContext.DoAction(this.PropertyDefinitions, this.ProcessInstance, (context) =>
            {
                SOARolePropertyRowCollection matchedRows = this.Rows.Query(context.QueryParams);

                matchedRows = matchedRows.ExtractMatrixRows();

                matchedRows.FillCreateActivityParams(capc, this.PropertyDefinitions, definedProperties);
            });
        }

        public bool UseCreateActivityParams
        {
            get
            {
                return this.PropertyDefinitions.IsActivityMatrix;
            }
        }

        /// <summary>
        /// 从DataTable构造
        /// </summary>
        /// <param name="table"></param>
        public void FromDataTable(DataTable table)
        {
            table.NullCheck("table");

            this.PropertyDefinitions.FromDataColumns(table.Columns);
            this.Rows.FromDataTable(table.Rows, this.PropertyDefinitions);
        }
    }
}
