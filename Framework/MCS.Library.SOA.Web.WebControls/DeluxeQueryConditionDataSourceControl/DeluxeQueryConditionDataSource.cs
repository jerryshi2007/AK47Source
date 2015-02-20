using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Web.WebControls
{
    [PersistChildren(false), ParseChildren(true), Designer("System.Web.UI.Design.WebControls.ObjectDataSourceDesigner, System.Design"), ToolboxBitmap(typeof(ObjectDataSource)), DefaultEvent("Selecting"), DefaultProperty("Connection")]
    public class DeluxeQueryConditionDataSource : DeluxeObjectDataSource
    {
        public DeluxeQueryConditionDataSource()
        {
            this.TypeName = "MCS.Web.WebControls.DeluxeQueryInnerDataSourceAdapter";
            this.SortParameterName = "";
        }

        #region 隐藏属性

        private new string TypeName
        {
            get
            {
                return base.TypeName;
            }
            set
            {
                base.TypeName = value;
            }
        }

        private new string SelectMethod
        {
            get
            {
                return base.SelectMethod;
            }
        }

        private new string SelectCountMethod
        {
            get
            {
                return base.SelectCountMethod;
            }
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 数据库链接名称
        /// </summary>
        [DefaultValue(""), Browsable(true)]
        public string Connection
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "Connection", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "Connection", value);
            }
        }

        /// <summary>
        /// sql语句返回的字段
        /// </summary>
        [DefaultValue(""), Browsable(true)]
        public string SelectFields
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "SelectFields", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "SelectFields", value);
            }
        }

        /// <summary>
        /// sql语句的orderBy部分
        /// </summary>
        [DefaultValue(""), Browsable(true)]
        public string OrderByClause
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "OrderByClause", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "OrderByClause", value);
            }
        }

        /// <summary>
        /// sql语句的where部分
        /// </summary>
        public string WhereClause
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "WhereClause", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "WhereClause", value);
            }
        }

        /// <summary>
        /// 表名
        /// </summary>
        [DefaultValue(""), Browsable(true)]
        public string TableName
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "TableName", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "TableName", value);
            }
        }

        #endregion

        #region 方法

        protected override void OnInit(EventArgs e)
        {
            this.Selecting += new ObjectDataSourceSelectingEventHandler(DeluxeQueryConditionDataSource_Selecting);
            base.OnInit(e);
        }

        private void DeluxeQueryConditionDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            this.Connection.IsNullOrEmpty().TrueThrow("Connection不能为空！");
            this.TableName.IsNullOrEmpty().TrueThrow("TableName不能为空！");

            e.InputParameters["connection"] = this.Connection;
            e.InputParameters["tableName"] = this.TableName;
            e.InputParameters["selectFields"] = this.SelectFields;
            e.InputParameters["orderBy"] = this.OrderByClause;

            this.Condition = this.WhereClause;
        }

        #endregion
    }

    internal class DeluxeQueryInnerDataSourceAdapter
    {
        public DataSet Query(int startRowIndex, int maximumRows, string connection, string tableName, string selectFields, ref int totalCount)
        {
            return Query(startRowIndex, maximumRows, connection, tableName, selectFields, string.Empty, string.Empty, ref totalCount);
        }

        public DataSet Query(int startRowIndex, int maximumRows, string connection, string tableName, string selectFields, string orderBy, ref int totalCount)
        {
            return Query(startRowIndex, maximumRows, connection, tableName, selectFields, string.Empty, orderBy, ref totalCount);
        }

        public DataSet Query(int startRowIndex, int maximumRows, string connection, string tableName, string selectFields, string where, string orderBy, ref int totalCount)
        {
            QueryCondition qc = new QueryCondition(startRowIndex, maximumRows, selectFields, tableName, orderBy, where);

            //OnBuildQueryCondition(qc);

            CommonAdapter adapter = new CommonAdapter(connection);

            DataSet result = adapter.SplitPageQuery(qc, totalCount <= 0);

            if (result.Tables.Count > 1)
                totalCount = (int)result.Tables[1].Rows[0][0];

            var contextCacheKey = connection + "." + tableName + ".Query";
            ObjectContextCache.Instance[contextCacheKey] = totalCount;

            //当页码超出索引的，返回最大页
            if (result.Tables[0].Rows.Count == 0 && totalCount > 0)
            {
                int newStartRowIndex = (totalCount - 1) / maximumRows * maximumRows;

                totalCount = -1;

                result = Query(newStartRowIndex, maximumRows, connection, tableName, selectFields, where, orderBy, ref totalCount);
            }

            return result;
        }

        public DataSet Query(string connection, string tableName, string selectFields, string where, string orderBy, ref int totalCount)
        {
            return Query(0, 0, connection, tableName, selectFields, where, orderBy, ref totalCount);
        }

        public int GetQueryCount(string connection, string tableName, string selectFields, string where, string orderBy, ref int totalCount)
        {
            var contextCacheKey = connection + "." + tableName + ".Query";
            totalCount = (int)ObjectContextCache.Instance[contextCacheKey];
            return totalCount;
        }
    }
}
