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
    public class WfActivityMatrixResourceDescriptor : WfResourceDescriptor, IWfCreateActivityParamsGenerator, IWfMatrixContainer
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

        /// <summary>
        /// 外部矩阵ID
        /// </summary>
        public string ExternalMatrixID
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是活动矩阵
        /// </summary>
        public bool UseCreateActivityParams
        {
            get
            {
                return this.PropertyDefinitions.MatrixType == WfMatrixType.ActivityMatrix;
            }
        }

        /// <summary>
        /// 矩阵的类型
        /// </summary>
        public WfMatrixType MatrixType
        {
            get
            {
                return this.PropertyDefinitions.MatrixType;
            }
        }

        protected internal override void FillUsers(OguDataCollection<IUser> users)
        {
            SOARoleContext.DoAction(this.PropertyDefinitions, this.ProcessInstance, (context) =>
            {
                SOARolePropertyRowCollection matchedRows = this.Rows.QueryWithoutCondition(context.QueryParams);

                matchedRows = matchedRows.ExtractMatrixRows();

                matchedRows = this.MergeExternalMatrix(matchedRows, context.QueryParams);

                matchedRows = matchedRows.FilterByConditionColumn();

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
                SOARolePropertyRowCollection matchedRows = this.Rows.QueryWithoutCondition(context.QueryParams);

                matchedRows = matchedRows.ExtractMatrixRows();

                matchedRows = this.MergeExternalMatrix(matchedRows, context.QueryParams);

                matchedRows = matchedRows.FilterByConditionColumn();

                matchedRows.FillCreateActivityParams(capc, this.PropertyDefinitions, definedProperties);
            });
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

        /// <summary>
        /// 得到外部矩阵
        /// </summary>
        /// <returns></returns>
        public IWfMatrixContainer GetExternalMatrix()
        {
            WfApprovalMatrix result = null;

            if (this.ExternalMatrixID.IsNotEmpty())
                result = WfApprovalMatrixAdapter.Instance.GetByID(this.ExternalMatrixID);
            else
                result = new WfApprovalMatrix();

            return result;
        }

        private SOARolePropertyRowCollection MergeExternalMatrix(SOARolePropertyRowCollection matrixRows, IEnumerable<SOARolePropertiesQueryParam> queryParams)
        {
            if (this.ExternalMatrixID.IsNotEmpty())
            {
                IWfMatrixContainer externalMatrix = this.GetExternalMatrix();

                SOARoleContext.DoNewContextAction(externalMatrix.PropertyDefinitions, this.ProcessInstance, (context) =>
                {
                    if (externalMatrix.PropertyDefinitions.MatrixType == WfMatrixType.ApprovalMatrix)
                    {
                        SOARolePropertyRowCollection approvalRows = externalMatrix.Rows.QueryWithoutCondition(context.QueryParams, true);

                        matrixRows.SortActivitySN();
                        matrixRows.MergeApprovalMatrix(this.PropertyDefinitions, approvalRows, externalMatrix.PropertyDefinitions);
                    }
                    else
                    {
                        SOARolePropertyRowCollection approvalRows = externalMatrix.Rows.QueryWithoutCondition(context.QueryParams, false);

                        matrixRows.SortActivitySN();
                        matrixRows.MergeActivityMatrix(this.PropertyDefinitions, approvalRows, externalMatrix.PropertyDefinitions);
                    }
                });

                matrixRows = matrixRows.FilterByConditionColumn();
            }

            return matrixRows;
        }
    }
}
