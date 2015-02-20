using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.OA.Portal.Common;
using MCS.Web.Library;
using System.Data;

namespace MCS.OA.Portal.Search
{
	public partial class SearchList : System.Web.UI.Page
	{
		private DataSet _DataSet;

		/// <summary>
		/// 查询的应用名
		/// </summary>
		private string ApplicationName
		{
			get
			{
				if (Request.QueryString["app"] != null)
					return Request.QueryString["app"].ToString();
				else
					return string.Empty;
			}
		}

		/// <summary>
		/// 查询的内容
		/// </summary>
		private string Content
		{
			get
			{
				if (Request.QueryString["content"] != null)
					return Request.QueryString["content"].ToString();
				else
					return string.Empty;
			}
		}

		private List<string> keywords = new List<string>();
		/// <summary>
		/// 各个关键词
		/// </summary>
		private List<string> Keywords
		{
			get
			{
				if (this.keywords.Count == 0 && this.Content != string.Empty)
				{
					this.keywords = GenerateKeywords(this.Content);
				}

				return this.keywords;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();

			ExceptionHelper.TrueThrow(this.Keywords.Count == 0, "请输入搜索条件");
			ExceptionHelper.TrueThrow(this.Content.Length > 128, "搜索条件字数过多");

			if (!IsPostBack)
			{
				UserSettings settings = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID);
				this.GridQuery.PageSize = settings.GetPropertyValue("CommonSettings", "SearchPageSize", 10);
				this.LiteralApplicationName.Text = "全部应用";
				//Note:应用名称显示注销
				//this.LiteralApplicationName.Text = string.Empty == ApplicationName ? "全部应用" : Server.HtmlEncode(ApplicationInfoConfig.GetConfig().Applications[ApplicationName].Description);
				this.LabelContent.Text = Server.HtmlEncode(this.Content);

				string where = "1 = 1";

				if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin") == false)
				{
					ConnectiveSqlClauseCollection cscc = WfAclAdapter.Instance.GetAclQueryConditionsByUser(DeluxeIdentity.CurrentUser.ID);

					string resourceIDList = string.Format("SELECT RESOURCE_ID FROM WF.ACL WHERE {0}", cscc.ToSqlString(TSqlBuilder.Instance));

					where = string.Format("{0} AND ACI.RESOURCE_ID IN ({1})", where, resourceIDList);
				}


				if (keywords.Count > 0)
				{
					StringBuilder queryString = new StringBuilder();

					for (int i = 0; i < keywords.Count; i++)
					{
						if (i > 0)
							queryString.Append(" AND ");

						queryString.Append('"');
						queryString.Append(keywords[i].Replace("\"", "\"\""));
						queryString.Append('"');
					}
					where = string.Format("{0} AND CONTAINS(ACI.*, {1})", where, TSqlBuilder.Instance.CheckQuotationMark(queryString.ToString(), true));
					queryValue.Value = TSqlBuilder.Instance.CheckQuotationMark(queryString.ToString(), true);
				}

				whereCondition.Value = where;


				//分页控件赋总条目数需要在这个时候给，只能显式的绑定一下来通过数据源Select，由于数据源绑定了分页控件的参数，分页控件初始化后还会再查一次，这样会导致多查一次。
				//或许今后在了解分页控件的技术支持下，能解决这个问题---by RexCheng
			}

			_DataSet = SearchFullData.Instance.SearchFullDataByQuery(queryValue.Value);
		}

		/// <summary>
		/// 为搜索结果中的关键字加亮
		/// </summary>
		/// <param name="origion"></param>
		/// <returns></returns>
		protected string RenderQueryWords(string origion)
		{
			for (int i = 0; i < this.Keywords.Count; i++)
			{
				//这里可能出现循环嵌套的问题比如第一个词嵌套了<span>，第二个词就是"<span>"
				//   origion = origion.Replace(this.Keywords[i], string.Format("<span style='color:Red'>{0}</span>", this.Keywords[i]));
				origion = GetExpress(keywords[i], origion);
			}

			return origion;
		}

		protected string RenderQueryWordsTitle(string origion)
		{
			for (int i = 0; i < this.Keywords.Count; i++)
			{
				//这里可能出现循环嵌套的问题比如第一个词嵌套了<span>，第二个词就是"<span>"
				origion = origion.Replace(this.Keywords[i], string.Format("<span style='color:Red'>{0}</span>", this.Keywords[i]));
			}
			return origion;
		}

		private static string GetExpress(string keyword, string origion)
		{
			//(\.|;|。|^|\r|\n).*?alaliu.*?(。|;|\.|$|\r|\n)
			string reg = "(\\.|。|;|；|^|\\r|\\n).*?" + keyword + ".*?(。|\\.|;|；|$|\\r|\\n)";
			Regex r = new Regex(reg); //定义一个Regex对象实例
			MatchCollection mc = r.Matches(origion);
			if (mc.Count == 0)
			{
				return origion;
			}
			string Express = string.Empty;
			for (int i = 0; i < mc.Count; i++) //在输入字符串中找到所有匹配
			{
				if (i != 0)
				{
					Express += "...";
				}
				Express += mc[i].Value.Replace(keyword, string.Format("<span style='color:Red'>{0}</span>", keyword));
			}
			return Express;
		}

		protected void ObjectDataSourceFiles_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			LastQueryRowCount = (int)e.OutputParameters["totalCount"];

			if (LastQueryRowCount == 0)
			{
				this.GridQuery.Visible = false;
				this.LabelContent2.Text = Server.HtmlEncode(this.Content);
				string script = "<script type=\"text/javascript\">document.getElementById('SearchIntroduction').style.display = 'none';document.getElementById('NoResultTips').style.display = 'block';</script>";
				Page.ClientScript.RegisterStartupScript(this.GetType(), "setScene", script);
			}
		}

		private int LastQueryRowCount
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value);
			}
		}

		protected void ObjectDataSourceFiles_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["totalCount"] = LastQueryRowCount;
		}

		/// <summary>
		/// 通过分割符区分多个关键词，同时过滤多个相同的关键词。
		/// </summary>
		/// <param name="origionContent"></param>
		/// <returns></returns>
		private List<string> GenerateKeywords(string origionContent)
		{
			char[] separators = new char[] { ' ', '　' };
			string[] wordsSplitted = origionContent.Split(separators, StringSplitOptions.RemoveEmptyEntries);

			List<string> wordsList = new List<string>();

			for (int i = 0; i < wordsSplitted.Length; i++)
			{
				if (wordsList.Contains(wordsSplitted[i]) == false)
				{
					wordsList.Add(wordsSplitted[i]);
				}
			}

			return wordsList;
		}

		protected string GetNormalizedUrl(string appCodeName, string progCodeName, string url)
		{
			if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin"))
			{
				NameValueCollection reqParams = UriHelper.GetUriParamsCollection(url);

				reqParams["mode"] = "Admin";

				url = UriHelper.CombineUrlParams(url, reqParams);
			}

			return UserTask.GetNormalizedUrl(appCodeName, progCodeName, url);
		}

		protected string GetProcessID(string resourceID, string url)
		{
			string processID = string.Empty;
			if (_DataSet != null)
			{
				DataTable tb = _DataSet.Tables[0];

				DataRow[] dr = tb.Select("RESOURCE_ID ='" + resourceID + "'");

				int i = dr.Count();

				if (i == 0)
				{
				}
				else if (i == 1)
				{
					processID = dr[0]["PROCESS_ID"].ToString() == "" ? dr[0]["INSTANCE_ID"].ToString() : dr[0]["PROCESS_ID"].ToString();

				}
				else if (i > 1)
				{
					NameValueCollection reqParams = UriHelper.GetUriParamsCollection(url);

					if (reqParams["porcessID"] != null)
					{
						processID = reqParams["porcessID"];
					}
					else
					{
						processID = tb.Select("RESOURCE_ID ='" + resourceID + "' AND OWNER_ACTIVITY_ID IS NULL")[0]["INSTANCE_ID"].ToString();
					}
				}

			}

			return processID;
		}
	}
}