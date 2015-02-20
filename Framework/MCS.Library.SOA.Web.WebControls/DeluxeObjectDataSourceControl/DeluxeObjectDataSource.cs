using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCS.Web.WebControls
{
	[PersistChildren(false), ParseChildren(true), Designer("System.Web.UI.Design.WebControls.ObjectDataSourceDesigner, System.Design"), ToolboxBitmap(typeof(ObjectDataSource)), DefaultEvent("Selecting"), DefaultProperty("TypeName")]
	public class DeluxeObjectDataSource : ObjectDataSource
	{
		private int _LastQueryRowCount = -1;

		public DeluxeObjectDataSource()
		{
			this.SelectMethod = "Query";
			this.SelectCountMethod = "GetQueryCount";
			this.SortParameterName = "orderBy";
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.Selecting += new ObjectDataSourceSelectingEventHandler(DeluxeObjectDataSource_Selecting);
			this.Selected += new ObjectDataSourceStatusEventHandler(DeluxeObjectDataSource_Selected);

			if (this.SelectParameters["where"] == null)
			{
				Parameter whereParameter = new Parameter("where", DbType.String, string.Empty);

				whereParameter.Direction = ParameterDirection.Input;

				this.SelectParameters.Add(new Parameter("where", DbType.String, string.Empty));
			}
		}

		protected override void LoadControlState(object savedState)
		{
			this._LastQueryRowCount = (int)(savedState ?? -1);
		}

		protected override object SaveControlState()
		{
			return this._LastQueryRowCount;
		}

		/// <summary>
		/// 最后查询返回的总行数
		/// </summary>
		public int LastQueryRowCount
		{
			get
			{
				return this._LastQueryRowCount;
			}
			set
			{
				this._LastQueryRowCount = value;
			}
		}

		/// <summary>
		/// SQL语句中的条件部分。可以是字符串、WhereSqlClauseBuilder或一个可序列化的对象(通过ConditionMapping来生成Where部分)
		/// </summary>
		[Browsable(false)]
		public object Condition
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "Condition", (object)null);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "Condition", value);
			}
		}

		private static string ConditionToSqlString(object condition)
		{
			string result = string.Empty;

			if (condition != null)
			{
				if (condition is IConnectiveSqlClause)
				{
					result = ((IConnectiveSqlClause)condition).ToSqlString(TSqlBuilder.Instance);
				}
				else
					if (condition is string)
					{
						result = condition.ToString();
					}
					else
					{
						result = ConditionMapping.GetWhereSqlClauseBuilder(condition).ToSqlString(TSqlBuilder.Instance);
					}
			}

			return result;
		}

		private void DeluxeObjectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["totalCount"] = this.LastQueryRowCount;
			e.InputParameters["where"] = ConditionToSqlString(this.Condition);
		}

		private void DeluxeObjectDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			LastQueryRowCount = (int)e.OutputParameters["totalCount"];
		}
	}
}
