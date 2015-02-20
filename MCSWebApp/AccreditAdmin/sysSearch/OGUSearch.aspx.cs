#region using

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Data.Common;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.Configuration;
using MCS.Applications.AccreditAdmin.Properties;
using MCS.Applications.AccreditAdmin.classLib;
#endregion

namespace MCS.Applications.AccreditAdmin.sysSearch
{
	/// <summary>
	/// OGUSearch ��ժҪ˵����
	/// </summary>
	public partial class OGUSearch : XmlRequestWebClass
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			switch (this.CommandName)
			{
				case "getRoot":
				case "getOrganizationChildren":
                    GetOrganizationChildren();                    
					break;
				case "queryOGUByCondition":
					QueryOGUByCondition();                    
					break;
				case "getPinYin":
					getPinYin(ParamValue(0));
					break;
				case "getUsersInGroups":
					GetUsersInGroups();
					break;
				case "getSecretariesOfLeaders":
					GetSecretariesOfLeaders();
					break;
				case "queryObjForOGUInput":
					QueryObjForOGUInput();
					break;
				case "getObjectsDetail":
					GetObjectsDetail();
					break;
				case "getUsersInGroupsInPage":
					GetUsersInGroupsInPage();
					break;
				default:
					this.SetErrorResult(_XmlResult, "ϵͳ��û��������ݴ���\"" + this.CommandName + "\"�ĳ���", string.Empty);
					break;
			}
		}

		/// <summary>
		/// ��ȡϵͳ��ָ���û����ֶ�������
		/// </summary>
		private void GetOrganizationChildren()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strLot = root.GetAttribute("listObjectType");//Ҫ���ѯ�Ķ�������
			if (strLot.Trim() == string.Empty)
				strLot = ((int)ListObjectType.ALL_TYPE).ToString();

			string strLod = root.GetAttribute("listObjectDelete");
			if (strLod.Trim() == string.Empty)
				strLod = ((int)ListObjectDelete.COMMON).ToString();

			string strAttr = OGUCommonDefine.CombinateAttr(root.GetAttribute("extAttr"));//Ҫ���ѯ������

			string strOrgAccessRankCN = root.GetAttribute("orgAccessRankCN");//Ҫ���ѯ�����������������
			string strUserAccessRankCN = root.GetAttribute("userAccessRankCN");//Ҫ���ѯ��Ա�������������
			string strHideType = root.GetAttribute("hideType");//���β�ѯҪ����ԵĶ��󣨲���Xml�ļ����÷�ʽIgnoreObjs.xml��
			string strDepth = root.GetAttribute("oguDepth");

			string strOrgClass = root.GetAttribute("orgClass");	// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
			int iOrgClass = (strOrgClass.Trim() == string.Empty ? 0 : int.Parse(strOrgClass.Trim()));
			string strOrgType = root.GetAttribute("orgType");	// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
			int iOrgType = (strOrgType.Trim() == string.Empty ? 0 : int.Parse(strOrgType.Trim()));
			if (strDepth.Trim() == string.Empty)
				strDepth = "1";

			using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
			{
				string strOrgGuid = root.GetAttribute("rootOrgGuid");//Ҫ���ѯ�ĸ�
				if (strOrgGuid.Trim() == string.Empty)
				{
					string strOrg = root.GetAttribute("rootOrg");
					if (strOrg.Trim() == string.Empty)
					{
						DataTable table = OGUReader.GetRootDSE().Tables[0];
						ExceptionHelper.TrueThrow(table.Rows.Count == 0, "ϵͳ�в����ڻ����������ݣ�����ϵϵͳ����Ա����");

						strOrgGuid = OGUCommonDefine.DBValueToString(table.Rows[0]["GUID"]);
					}
					else
					{
						DataTable table = OGUReader.GetObjectsDetail("ORGANIZATIONS", strOrg,
							SearchObjectColumn.SEARCH_ALL_PATH_NAME, string.Empty, SearchObjectColumn.SEARCH_NULL).Tables[0];
						ExceptionHelper.TrueThrow(table.Rows.Count == 0, "ϵͳ�в����ڻ����������ݡ�" + strOrg + "����");
						strOrgGuid = OGUCommonDefine.DBValueToString(table.Rows[0]["GUID"]);
					}
				}

				DataSet ds = OGUReader.GetOrganizationChildren(strOrgGuid, SearchObjectColumn.SEARCH_GUID,
					int.Parse(strLot), int.Parse(strLod), int.Parse(strDepth), strOrgAccessRankCN, strUserAccessRankCN,
					strHideType, strAttr, iOrgClass, iOrgType);
				_XmlResult = OGUReader.GetLevelSortXmlDocAttr(ds.Tables[0], "GLOBAL_SORT", "OBJECTCLASS",
					AccreditResource.OriginalSortDefault.Length);

				XmlNodeList objList = _XmlResult.DocumentElement.SelectNodes(".//*[@OBJ_NAME != '']");

				foreach (XmlElement elem in objList)
				{
					if (string.IsNullOrEmpty(elem.GetAttribute("DISPLAY_NAME")))
						elem.SetAttribute("DISPLAY_NAME", elem.GetAttribute("OBJ_NAME"));
				}

				Debug.WriteLine(_XmlResult.OuterXml, "Result");
			}
		}

        /// <summary>
        /// ��ȡϵͳ��ָ���û����ֶ�������
        /// </summary>
        /// 
        //2009-05-07
        private void GetOrganizationChildren2()
        {
            XmlElement root = _XmlRequest.DocumentElement;
            string strLot = root.GetAttribute("listObjectType");//Ҫ���ѯ�Ķ�������
            if (strLot.Trim() == string.Empty)
                strLot = ((int)ListObjectType.ALL_TYPE).ToString();

            string strLod = root.GetAttribute("listObjectDelete");
            if (strLod.Trim() == string.Empty)
                strLod = ((int)ListObjectDelete.COMMON).ToString();

            string strAttr = OGUCommonDefine.CombinateAttr(root.GetAttribute("extAttr"));//Ҫ���ѯ������           
            string strHideType = root.GetAttribute("hideType");//���β�ѯҪ����ԵĶ��󣨲���Xml�ļ����÷�ʽIgnoreObjs.xml��
            string strDepth = root.GetAttribute("oguDepth");

            string strOrgClass = root.GetAttribute("orgClass");	// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
            int iOrgClass = (strOrgClass.Trim() == string.Empty ? 0 : int.Parse(strOrgClass.Trim()));
            string strOrgType = root.GetAttribute("orgType");	// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
            int iOrgType = (strOrgType.Trim() == string.Empty ? 0 : int.Parse(strOrgType.Trim()));
            if (strDepth.Trim() == string.Empty)
                strDepth = "1";

            using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
            {
                string strOrgGuid = root.GetAttribute("rootOrgGuid");//Ҫ���ѯ�ĸ�
                if (strOrgGuid.Trim() == string.Empty)
                {
                    string strOrg = root.GetAttribute("rootOrg");
                    if (strOrg.Trim() == string.Empty)
                    {
                        DataTable table = OGUReader.GetRootDSE().Tables[0];
                        ExceptionHelper.TrueThrow(table.Rows.Count == 0, "ϵͳ�в����ڻ����������ݣ�����ϵϵͳ����Ա����");

                        strOrgGuid = OGUCommonDefine.DBValueToString(table.Rows[0]["GUID"]);
                    }
                    else
                    {
                        DataTable table = OGUReader.GetObjectsDetail("ORGANIZATIONS", strOrg,
                            SearchObjectColumn.SEARCH_ALL_PATH_NAME, string.Empty, SearchObjectColumn.SEARCH_NULL).Tables[0];
                        ExceptionHelper.TrueThrow(table.Rows.Count == 0, "ϵͳ�в����ڻ����������ݡ�" + strOrg + "����");
                        strOrgGuid = OGUCommonDefine.DBValueToString(table.Rows[0]["GUID"]);
                    }
                }

                DataSet ds = OGUReader.GetOrganizationChildren2(strOrgGuid, SearchObjectColumn.SEARCH_GUID,
                    int.Parse(strLot), int.Parse(strLod), int.Parse(strDepth),
                    strHideType, strAttr, iOrgClass, iOrgType);
                _XmlResult = OGUReader.GetLevelSortXmlDocAttr(ds.Tables[0], "GLOBAL_SORT", "OBJECTCLASS",
                    AccreditResource.OriginalSortDefault.Length);
                Debug.WriteLine(_XmlResult.OuterXml, "Result");
            }
        }

		/// <summary>
		/// ���ݲ�ѯ������ȡϵͳ�з������������ݶ���
		/// </summary>
		private void QueryOGUByCondition()
		{
			XmlNode root = _XmlRequest.DocumentElement.FirstChild;
			string strRootOrg = XmlHelper.GetSingleNodeValue<string>(root, "ALL_PATH_NAME", string.Empty);
			string strLikeName = XmlHelper.GetSingleNodeValue<string>(root, "name", "*");
			bool bFirstPerson = (root.SelectSingleNode("firstPerson") != null);

			string strOrgAccessRankCN = string.Empty;
			string strUserAccessRankCN = string.Empty;
			int iQueryType = (int)ListObjectType.GROUPS;
			if (root.SelectSingleNode("USERS") != null)
			{
				iQueryType = (int)(ListObjectType.USERS | ListObjectType.SIDELINE);
				strUserAccessRankCN = XmlHelper.GetSingleNodeValue<string>(root, "RANK_CODE", string.Empty);
			}
			else
			{
				if (root.SelectSingleNode("ORGANIZATIONS") != null)
				{
					iQueryType = (int)ListObjectType.ORGANIZATIONS;
					strOrgAccessRankCN = XmlHelper.GetSingleNodeValue<string>(root, "RANK_CODE", string.Empty);
				}
			}

			string strAttr = OGUCommonDefine.CombinateAttr(string.Empty);

			using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
			{
				string strOrgGuid = string.Empty;
				string strSql = string.Empty;

				if (strRootOrg.Length > 0)
				{
					strSql = "SELECT GUID FROM ORGANIZATIONS WHERE ALL_PATH_NAME = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strRootOrg, true);
					Database database = DatabaseFactory.Create(context);

					object obj = database.ExecuteScalar(CommandType.Text, strSql);

					ExceptionHelper.TrueThrow(obj == null, "ϵͳ��û���ҵ�ָ���ĸ����󣨡�" + strRootOrg + "������");

					strOrgGuid = obj.ToString();
				}
				else
					strOrgGuid = OGUCommonDefine.DBValueToString(OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"]);

				DataSet ds = OGUReader.QueryOGUByCondition(strOrgGuid, SearchObjectColumn.SEARCH_GUID, strLikeName, true,
					bFirstPerson, strOrgAccessRankCN, strUserAccessRankCN, strAttr, iQueryType, 0, string.Empty);
				_XmlResult = InnerCommon.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");
				Debug.WriteLine(_XmlResult.OuterXml, "Result");
			}
		}

        /// <summary>
        /// ���ݲ�ѯ������ȡϵͳ�з������������ݶ���
        /// </summary>
        /// 
        //2009-05-08
        private void QueryOGUByCondition2()
        {
            XmlNode root = _XmlRequest.DocumentElement.FirstChild;
            string strRootOrg = XmlHelper.GetSingleNodeValue<string>(root, "ALL_PATH_NAME", string.Empty);
            string strLikeName = XmlHelper.GetSingleNodeValue<string>(root, "name", "*");
            bool bFirstPerson = (root.SelectSingleNode("firstPerson") != null);

            string strOrgAccessRankCN = string.Empty;
            string strUserAccessRankCN = string.Empty;
            int iQueryType = (int)ListObjectType.GROUPS;
            if (root.SelectSingleNode("USERS") != null)
            {
                iQueryType = (int)(ListObjectType.USERS | ListObjectType.SIDELINE);
                strUserAccessRankCN = XmlHelper.GetSingleNodeValue<string>(root, "RANK_CODE", string.Empty);
            }
            else
            {
                if (root.SelectSingleNode("ORGANIZATIONS") != null)
                {
                    iQueryType = (int)ListObjectType.ORGANIZATIONS;
                    strOrgAccessRankCN = XmlHelper.GetSingleNodeValue<string>(root, "RANK_CODE", string.Empty);
                }
            }

            string strAttr = OGUCommonDefine.CombinateAttr(string.Empty);

            using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
            {
                string strOrgGuid = string.Empty;
                string strSql = string.Empty;

                if (strRootOrg.Length > 0)
                {
                    strSql = "SELECT GUID FROM ORGANIZATIONS WHERE ALL_PATH_NAME = "
                        + TSqlBuilder.Instance.CheckQuotationMark(strRootOrg, true);
                    Database database = DatabaseFactory.Create(context);

                    object obj = database.ExecuteScalar(CommandType.Text, strSql);

                    ExceptionHelper.TrueThrow(obj == null, "ϵͳ��û���ҵ�ָ���ĸ����󣨡�" + strRootOrg + "������");

                    strOrgGuid = obj.ToString();
                }
                else
                    strOrgGuid = OGUCommonDefine.DBValueToString(OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"]);

                DataSet ds = OGUReader.QueryOGUByCondition2(strOrgGuid, SearchObjectColumn.SEARCH_GUID, strLikeName, true,
                     strAttr, iQueryType, ListObjectDelete.COMMON, 0, string.Empty, -1);
                _XmlResult = InnerCommon.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");
                Debug.WriteLine(_XmlResult.OuterXml, "Result");
            }
        }

		/// <summary>
		/// ���ݲ�ѯ������ѯϵͳ�з������������ж���
		/// </summary>
		private void QueryObjForOGUInput()
		{
			XmlElement root = (XmlElement)_XmlRequest.DocumentElement.FirstChild;
			string strLikeName = root.GetAttribute("likeName");
			ExceptionHelper.TrueThrow(strLikeName.Length == 0, "�Բ��𣬲�ѯ��������Ϊ�գ�");

			int iListType = 0;
			string strQueryObjMask = root.GetAttribute("queryObjMask");
			if (strQueryObjMask.Length == 0)
				iListType = (int)ListObjectType.USERS;
			else
				iListType = int.Parse(strQueryObjMask);

			string strRootOrg = root.GetAttribute("rootOrg");
			if (strRootOrg.Length == 0)
				strRootOrg = AccreditSection.GetConfig().AccreditSettings.OguRootName;

			string strOrgAccessLevel = root.GetAttribute("orgAccessLevel");
			string strUserAccesslevel = root.GetAttribute("userAccessLevel");
			string strAttr = root.GetAttribute("extAttr");

			DataSet ds = OGUReader.QueryOGUByCondition(strRootOrg,
				SearchObjectColumn.SEARCH_ALL_PATH_NAME,
				strLikeName,
				false,
				strOrgAccessLevel,
				strUserAccesslevel,
				strAttr,
				iListType);

			_XmlResult = InnerCommon.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");
			Debug.WriteLine(_XmlResult.OuterXml, "Result");
		}

		/// <summary>
		/// ���ݺ��ֻ�ȡ��Ӧ���ֵ�ƴ��
		/// </summary>
		/// <param name="strPinYin"></param>
		private void getPinYin(string strPinYin)
		{
			string strSelect = string.Empty;
			string strFrom = string.Empty;
			string strWhere = string.Empty;
			string strOrderBy = string.Empty;

			for (int i = 0; i < strPinYin.Length; i++)
			{
				char ch = strPinYin[i];

				if (ch < ' ' || ch > 'z')
				{
					string strAlias = "P" + i.ToString();

					if (strSelect != string.Empty)
						strSelect += ", ";
					strSelect += strAlias + ".PINYIN";

					if (strFrom != string.Empty)
						strFrom += ", ";
					strFrom += "PINYIN " + strAlias;

					if (strWhere != string.Empty)
						strWhere += " AND ";

					strWhere += strAlias + ".HZ = " + TSqlBuilder.Instance.CheckQuotationMark(ch.ToString(), true);

					if (strOrderBy != string.Empty)
						strOrderBy += ", ";

					strOrderBy += strAlias + ".WEIGHT"; 
				}
			}

			StringBuilder strB = new StringBuilder(1024);
			string strResult = strPinYin;

			if (strSelect != string.Empty)
			{
				string strSql = "SELECT " + strSelect + " \n FROM " + strFrom + " \n WHERE " + strWhere + "\n ORDER BY " + strOrderBy;

				using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
				{
					Database database = DatabaseFactory.Create(context);

					DbDataReader dr = database.ExecuteReader(CommandType.Text, strSql);

					_XmlResult = new XmlDocument();
					_XmlResult.LoadXml("<PINYIN />");

					XmlElement root = _XmlResult.DocumentElement;

					while (dr.Read())
					{
						int nIndex = 0;
						for (int i = 0; i < strPinYin.Length; i++)
						{
							char ch = strPinYin[i];

							if (ch < ' ' || ch > 'z')
							{
								object objValue = dr[nIndex++];

								if (objValue != null)
									strB.Append(objValue.ToString());
								else
									strB.Append(ch);
							}
							else
								strB.Append(ch);
						}

						XmlElement nodeCode = (XmlElement)XmlHelper.AppendNode(root, "Code");

						nodeCode.SetAttribute("pinyin", "", strB.ToString());
						strB.Remove(0, strB.Length);
					}
				}
			}
#if DEBUG
			Debug.WriteLine(strResult, "DBPINYIN");
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		private void GetUsersInGroups()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strGroupGuid = root.GetAttribute("GUID");
			string strAttrs = OGUCommonDefine.CombinateAttr(root.GetAttribute("extAttr"));

			DataSet ds = OGUReader.GetUsersInGroups(strGroupGuid,
				SearchObjectColumn.SEARCH_GUID,
				strAttrs,
				string.Empty,
				SearchObjectColumn.SEARCH_NULL,
				string.Empty,
				(int)ListObjectDelete.COMMON);

			_XmlResult = InnerCommon.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");
		}

		/// <summary>
		/// 
		/// </summary>
		private void GetUsersInGroupsInPage()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strGroupGuid = root.GetAttribute("GUID");
			string strAttrs = OGUCommonDefine.CombinateAttr(root.GetAttribute("extAttr"));
			int iPageNo = Convert.ToInt32(root.GetAttribute("PageNo"));
			int iPageSize = Convert.ToInt32(root.GetAttribute("PageSize"));
			string strSortColumn = root.GetAttribute("PageSort");
			string strSearchName = root.GetAttribute("SearchName");

			DataSet ds = OGUReader.GetUsersInGroups(strGroupGuid, SearchObjectColumn.SEARCH_GUID, strSearchName,
				strSortColumn, strAttrs, iPageNo, iPageSize);
			_XmlResult = InnerCommon.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");

			if (root.GetAttribute("GetCount") == "true")
				_XmlResult.DocumentElement.SetAttribute("GetCount", OGUReader.GetUsersInGroups(strGroupGuid,
					SearchObjectColumn.SEARCH_GUID, strSearchName, strSortColumn, 0, 0).Tables[0].Rows.Count.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		private void GetObjectsDetail()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strValueType = root.GetAttribute("valueType");
			SearchObjectColumn soc = OGUCommonDefine.GetSearchObjectColumn(strValueType);

			string strValue = root.GetAttribute("oValues");
			string strExtAttr = root.GetAttribute("extAttrs");

			DataSet ds = OGUReader.GetObjectsDetail(string.Empty, strValue, soc, string.Empty, SearchObjectColumn.SEARCH_NULL, strExtAttr);
			_XmlResult = InnerCommon.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");
#if DEBUG
			Debug.WriteLine(_XmlResult.OuterXml, "Result");
#endif
		}
		/// <summary>
		/// 
		/// </summary>
		private void GetSecretariesOfLeaders()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strLeaderGuid = root.GetAttribute("GUID");
			string strAttrs = OGUCommonDefine.CombinateAttr(root.GetAttribute("extAttr"));

			DataSet ds = OGUReader.GetSecretariesOfLeaders(strLeaderGuid,
				SearchObjectColumn.SEARCH_GUID,
				strAttrs,
				(int)ListObjectDelete.COMMON);

			_XmlResult = InnerCommon.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");
		}


		#region Web ������������ɵĴ���
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: �õ����� ASP.NET Web ���������������ġ�
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
