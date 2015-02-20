using System;
using System.Web;
using System.Text;
using System.Data;
using System.Web.UI;
using System.Collections.Generic;
using MCS.Library.Data;
using MCS.Library.Core;
using MCS.Library.Data.Builder;

namespace MCS.Web.Library
{
	/// <summary>
	/// ��ViewState���浽SqlServer�еĳ־û���
	/// </summary>
	public class SqlPageStatePersister : PageStatePersister
	{
		private const string ConnectionName = "PageViewState";

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="page">ҳ��</param>
		public SqlPageStatePersister(Page page)
			: base(page)
		{
		}

		/// <summary>
		/// ����ViewState�����ViewState�ĳߴ�С��������Ϣ�е�ֵ��ȱʡΪ10K�������������м��أ���������ݿ��м���
		/// </summary>
		public override void Load()
		{
			string data = this.Page.Request.Form["__VIEWSTATE"];

			if (string.IsNullOrEmpty(data) == false)
			{
				Pair statePair = (Pair)StateFormatter.Deserialize(data);

				ControlState = statePair.Second;
				ViewState = statePair.First;

				long id;

				if (long.TryParse(ViewState.ToString(), out id))
				{
					string stateStr = LoadStateFromDB(id);

					ViewState = StateFormatter.Deserialize(stateStr);
				}
			}
		}

		/// <summary>
		/// ����ViewState�����ViewState�ĳߴ�С��������Ϣ�е�ֵ��ȱʡΪ10K�������浽�������У����򱣴浽���ݿ���
		/// </summary>
		public override void Save()
		{
			if (ViewState != null || ControlState != null)
			{
				string hiddenFieldData = string.Empty;

				string serializedState = StateFormatter.Serialize(ViewState);

				if (serializedState.Length >= ViewStatePersistSettings.GetConfig().Threshold)
					ViewState = SaveStateToDB(serializedState);
			}
		}

		private static string SaveStateToDB(string state)
		{
			InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

			builder.AppendItem("DATA", state);

			StringBuilder strB = new StringBuilder(5120);

			strB.Append("INSERT INTO PAGE_VIEW_STATE ");
			strB.Append(builder.ToSqlString(TSqlBuilder.Instance));
			strB.Append("\nSELECT SCOPE_IDENTITY()");

			using (DbContext context = DbContext.GetContext(ConnectionName))
			{
				Database db = DatabaseFactory.Create(context);

				Decimal result = (Decimal)db.ExecuteScalar(CommandType.Text, strB.ToString());

				return result.ToString();
			}
		}

		private static string LoadStateFromDB(long id)
		{
			string sql = string.Format("SELECT DATA FROM PAGE_VIEW_STATE WHERE ID = {0}", id);

			using (DbContext context = DbContext.GetContext(ConnectionName))
			{
				Database db = DatabaseFactory.Create(context);

				string result = (string)db.ExecuteScalar(CommandType.Text, sql);

				ExceptionHelper.FalseThrow(result != null, "����IDΪ{0}����ͼ����", id);

				return result;
			}
		}
	}
}
