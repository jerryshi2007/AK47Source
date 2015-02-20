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
using System.IO;
using System.Data.OleDb;
using System.Diagnostics;
using System.Xml;
using System.Text;
using System.Transactions;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.Accredit.AppAdmin;
using MCS.Library.Accredit.OguAdmin;
using MCS.Applications.AccreditAdmin.classLib;
using MCS.Applications.AccreditAdmin.Properties;
#endregion

namespace MCS.Applications.AccreditAdmin.dialogs
{
	/// <summary>
	/// PrepareForImport ��ժҪ˵����
	/// </summary>
	public partial class PrepareForImport : WebUserBaseClass
	{

		private XmlElement _XmlParamElem = null;
		private Hashtable _RootHash = new Hashtable();
		private string _RootAllPathName = string.Empty;
		private string[][] _DataColumns;//Value, Text, ColumnName
		private DataSet _RankDefineDS;
		private StringBuilder _StrBuild = new StringBuilder(1024);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (IsPostBack)
			{
				try
				{
					DataSet ds = null;
					ExceptionHelper.TrueThrow(importFile.PostedFile.FileName.Trim() == string.Empty, "�Բ���ϵͳû���ϴ��ļ���");

					string strFileName, strFileExtension, strNewFileName;
					strFileName = Path.GetFileName(importFile.PostedFile.FileName);

					strFileExtension = System.IO.Path.GetExtension(strFileName);
					ExceptionHelper.TrueThrow(strFileExtension.ToUpper() != ".XLS", "�Բ������ϴ����ļ���ʽ����ȷ�����ϴ���׼Excel�ļ���");

					string strTempPath = Path.GetTempPath();
					strNewFileName = Guid.NewGuid().ToString().Replace("-", "") + strFileExtension;
					try
					{
						importFile.PostedFile.SaveAs(strTempPath + strNewFileName);

						string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strTempPath + strNewFileName + ";Extended Properties=Excel 8.0;";
						OleDbConnection oConn = new OleDbConnection(strConn);
						try
						{
							oConn.Open();
							OleDbCommand oCmdSelect = new OleDbCommand("SELECT * FROM [Sheet1$]", oConn);
							OleDbDataAdapter oAdapter = new OleDbDataAdapter();
							oAdapter.SelectCommand = oCmdSelect;
							ds = new DataSet();
							oAdapter.Fill(ds, "XLData");
#if DEBUG
							Debug.WriteLine(ds.GetXmlSchema(), "");
							Debug.WriteLine(ds.GetXml(), "")  ;
#endif
						}
						finally
						{
							oConn.Close();
							oConn.Dispose();
						}
						using (TransactionScope scope = TransactionScopeFactory.Create())
						{
							DataCheckAndPutInDB(ds);
							scope.Complete();
						}
						middleTD.InnerHtml = "�����ļ�����ɹ���";
					}
					finally
					{
						if (File.Exists(strTempPath + strNewFileName))
							File.Delete(strTempPath + strNewFileName);
					}
				}
				catch (Exception ex)
				{
					middleTD.InnerHtml = ex.Message;
				}
				finally
				{
					btnOK.Visible = false;
				}
			}
			else
			{
				CheckImportUserRole();
			}
		}

		private void DataCheckAndPutInDB(DataSet ds)
		{
			DataTable oTable = ds.Tables[0];

			GetRootOrgAllPathName(oTable);
			if (false == _RootHash.ContainsKey(_RootAllPathName))
			{
				DataTable table = OGUReader.GetObjectsDetail("ORGANIZATIONS", _RootAllPathName, SearchObjectColumn.SEARCH_ALL_PATH_NAME).Tables[0];

				while (table.Rows.Count <= 0)
				{
					_RootAllPathName = _RootAllPathName.Substring(0, _RootAllPathName.LastIndexOf("\\"));
					ExceptionHelper.TrueThrow(_RootAllPathName.Length == 0, "�Բ����Ҳ���ָ���ĸ�������");
					table = OGUReader.GetObjectsDetail("ORGANIZATIONS", _RootAllPathName, SearchObjectColumn.SEARCH_ALL_PATH_NAME).Tables[0];
				}

				_RootHash.Add(_RootAllPathName, table.Rows[0]["GUID"]);
			}

			CheckImportPermission();

			GetColumnArray(oTable);

			try
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
					{
						for (int iRow = 1; iRow < oTable.Rows.Count; iRow++)
						{
							if (ContainsValue(1, "ϵͳλ��") >= 0)
							{
								string strAllPathName = (string)oTable.Rows[iRow][_DataColumns[ContainsValue(1, "ϵͳλ��")][2]];
								if (strAllPathName == _RootAllPathName)
									continue;
								ExceptionHelper.FalseThrow(strAllPathName.Length > _RootAllPathName.Length,
									"�Բ������õġ�ϵͳλ�á�--��" + strAllPathName + "��������Ҫ��");
								ExceptionHelper.FalseThrow(strAllPathName.Substring(0, _RootAllPathName.Length) == _RootAllPathName,
									"�Բ������õġ�ϵͳλ�á�--��" + strAllPathName + "��������Ҫ��");
							}

							string strObjClass = string.Empty;
							switch (oTable.Rows[iRow][1].ToString())
							{
								case "����":
									strObjClass = "ORGANIZATIONS";
									InsertOrgOrGroups(oTable.Rows[iRow], strObjClass);
									break;
								case "��Ա��":
									strObjClass = "GROUPS";
									InsertOrgOrGroups(oTable.Rows[iRow], strObjClass);
									break;
								case "��Ա":
									strObjClass = "USERS";
									InsertUsers(oTable.Rows[iRow]);
									break;
								default:
									ExceptionHelper.TrueThrow(true, "�������͡�" + oTable.Rows[iRow][1].ToString() + "��������ϵͳ�淶(��"
										+ (iRow + 2).ToString() + "��)��\n\nϵͳֻ�ܳ���3�������������Ա�顱������Ա����������ٴ��ύ��");
									break;
							}
						}
#if DEBUG
						Debug.WriteLine(_StrBuild.ToString(), "ALLSQL");
#endif
					}
					scope.Complete();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				switch (ex.Number)
				{
					//						case 2627://�ؼ��ֳ�ͻ
					//							throw new Exception("�Բ���,ϵͳ�з������ݿ�ؼ��ֳ�ͻ�������ԣ�");
					case 2601://������ͻ
						string strMsg = ex.Message;
						ExceptionHelper.TrueThrow(strMsg.IndexOf("LOGON_NAME") >= 0, "�Բ��𣬸õ�¼����ϵͳ���Ѵ��ڣ��뻻һ����¼����");

						ExceptionHelper.TrueThrow(strMsg.IndexOf("ALL_PATH_NAME") >= 0, "�Բ���ϵͳ���Ѿ������������������ݶ���\n\n���޸ġ��������ơ����ٱ��棡");

						ExceptionHelper.TrueThrow(strMsg.IndexOf("PERSON_ID") >= 0, "�Բ���ϵͳ���Ѿ�������ָ���ġ���Ա���롱��\n\n���޸ĺ��ٱ��棡");

						ExceptionHelper.TrueThrow(strMsg.IndexOf("IC_CARD") >= 0, "�Բ���ϵͳ���Ѿ�������ָ���ġ�IC�����롱��\n\n���޸ĺ��ٱ��棡");

						ExceptionHelper.TrueThrow(strMsg.IndexOf("CUSTOMS_CODE") >= 0, "�Բ���ϵͳ���Ѿ�������ָ���ġ��������롱��\n\n���޸ĺ��ٱ��棡");

						break;
				}
				throw ex;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void InsertUsers(DataRow oRow)
		{
			ExceptionHelper.TrueThrow(ContainsValue(0, "LOGON_NAME") < 0, "�Բ��𣬵�����Ա���ݱ����������Ա�ġ���¼���ơ���");
			string strParent, strSelfAllPathName, strRootGuid, strInnerSort;
			DataRow row = PrepareForInsert(oRow, out strParent, out strSelfAllPathName, out strRootGuid, out strInnerSort);

			string strGuid = Guid.NewGuid().ToString();
			InsertSqlClauseBuilder uIC = new InsertSqlClauseBuilder();
			InsertSqlClauseBuilder ouIC = new InsertSqlClauseBuilder();

			uIC.AppendItem("GUID", strGuid);
			//uIC.AppendItem("PERSON_ID", string.Empty);// strGuid.Substring(0, 8));
			ouIC.AppendItem("USER_GUID", strGuid);
			ouIC.AppendItem("PARENT_GUID", strRootGuid);

			for (int i = 2; i < oRow.Table.Columns.Count; i++)
			{
				if (string.IsNullOrEmpty(oRow[i].ToString()) == false)
				{
					string strRealColumnName = _DataColumns[ContainsValue(2, oRow.Table.Columns[i].ColumnName)][0];

					if (strRealColumnName == "NAME")
						uIC.AppendItem("RANK_CODE", GetRankCode("USERS", (string)oRow[i]));
					else
					{
						if (strRealColumnName == "GUID" || strRealColumnName.IndexOf("GUID") >= 0)
							continue;
						else
						{
							if (CheckXsdExist(strRealColumnName, "USERS"))
								uIC.AppendItem(strRealColumnName, oRow[i]);
							if (CheckXsdExist(strRealColumnName, "OU_USERS"))
								ouIC.AppendItem(strRealColumnName, oRow[i]);
						}
					}
				}
			}

			if (ContainsValue(0, "NAME") < 0)
				uIC.AppendItem("RANK_CODE", GetRankCode("USERS", string.Empty));

			GetAndSetUserName(uIC, oRow);

			if (ContainsValue(0, "ALL_PATH_NAME") < 0)
				ouIC.AppendItem("ALL_PATH_NAME", strSelfAllPathName);
			ouIC.AppendItem("INNER_SORT", strInnerSort);
			ouIC.AppendItem("GLOBAL_SORT", OGUCommonDefine.DBValueToString(row["GLOBAL_SORT"]) + strInnerSort);
			ouIC.AppendItem("ORIGINAL_SORT", OGUCommonDefine.DBValueToString(row["ORIGINAL_SORT"]) + strInnerSort);
			ouIC.AppendItem("STATUS", "1");

			string strSql = "INSERT INTO USERS " + uIC.ToSqlString(TSqlBuilder.Instance) + ";\n"
				+ "INSERT INTO OU_USERS " + ouIC.ToSqlString(TSqlBuilder.Instance) + ";\n";

			InnerCommon.ExecuteDataset(strSql);
			_StrBuild.Append(strSql);
		}

		private void InsertOrgOrGroups(DataRow oRow, string strObjClass)
		{
			string strParent, strSelfAllPathName, strRootGuid, strInnerSort;
			DataRow row = PrepareForInsert(oRow, out strParent, out strSelfAllPathName, out strRootGuid, out strInnerSort);

			string strGuid = Guid.NewGuid().ToString();
			InsertSqlClauseBuilder ic = new InsertSqlClauseBuilder();
			ic.AppendItem("GUID", strGuid);
			ic.AppendItem("PARENT_GUID", strRootGuid);

			for (int i = 2; i < oRow.Table.Columns.Count; i++)
			{
				string strRealColumnName = _DataColumns[ContainsValue(2, oRow.Table.Columns[i].ColumnName)][0];
				if (strRealColumnName == "NAME" && strObjClass == "ORGANIZATIONS")
					ic.AppendItem("RANK_CODE", GetRankCode(strObjClass, (string)oRow[i]));
				else
				{
					if (strRealColumnName == "GUID" || strRealColumnName.IndexOf("GUID") >= 0)
						continue;
					else
					{
						if (CheckXsdExist(strRealColumnName, strObjClass))
							ic.AppendItem(strRealColumnName, oRow[i]);
					}
				}
			}

			if (ContainsValue(0, "NAME") < 0 && strObjClass == "ORGANIZATIONS")
				ic.AppendItem("RANK_CODE", GetRankCode(strObjClass, string.Empty));
			if (ContainsValue(0, "ALL_PATH_NAME") < 0)
				ic.AppendItem("ALL_PATH_NAME", strSelfAllPathName);

			ic.AppendItem("INNER_SORT", strInnerSort);
			ic.AppendItem("GLOBAL_SORT", OGUCommonDefine.DBValueToString(row["GLOBAL_SORT"]) + strInnerSort);
			ic.AppendItem("ORIGINAL_SORT", OGUCommonDefine.DBValueToString(row["ORIGINAL_SORT"]) + strInnerSort);
			ic.AppendItem("STATUS", "1");

			string strSql = "INSERT INTO " + strObjClass + ic.ToSqlString(TSqlBuilder.Instance) + ";\n";

			_StrBuild.Append(strSql);

			InnerCommon.ExecuteDataset(strSql);

			if (strObjClass == "ORGANIZATIONS")
				_RootHash.Add(strSelfAllPathName, strGuid);
		}

		private void GetAndSetUserName(InsertSqlClauseBuilder uIC, DataRow oRow)
		{
			string strObjName = (string)oRow[_DataColumns[ContainsValue(0, "OBJ_NAME")][2]];
			string strFirstName, strLastName;

			if (ContainsValue(0, "FIRST_NAME") >= 0)
				strFirstName = (string)oRow[_DataColumns[ContainsValue(0, "FIRST_NAME")][2]];
			else
			{
				if (strObjName.Length == 2)
					strFirstName = strObjName.Substring(1, 1);
				else
					strFirstName = strObjName.Substring(strObjName.Length - 2, 2);
			}
			uIC.AppendItem("FIRST_NAME", strFirstName);

			if (ContainsValue(0, "LAST_NAME") >= 0)
				strLastName = (string)oRow[_DataColumns[ContainsValue(0, "LAST_NAME")][2]];
			else
				strLastName = strObjName.Substring(0, strObjName.LastIndexOf(strFirstName));
			uIC.AppendItem("LAST_NAME", strLastName);
		}

		private DataRow PrepareForInsert(DataRow oRow, out string strParent, out string strSelfAllPathName, out string strRootGuid, out string strInnerSort)
		{
			strParent = _RootAllPathName;
			if (ContainsValue(0, "ALL_PATH_NAME") >= 0)
			{
				strSelfAllPathName = (string)oRow[_DataColumns[ContainsValue(0, "ALL_PATH_NAME")][2]];
				strParent = strSelfAllPathName.Substring(0, strSelfAllPathName.LastIndexOf("\\"));
			}
			else
				strSelfAllPathName = strParent + "\\" + (string)oRow[_DataColumns[ContainsValue(0, "OBJ_NAME")][2]];

			//strRootGuid = (string)_RootHash[strParent];
			//			string strSql = @"
			//				UPDATE ORGANIZATIONS 
			//					SET CHILDREN_COUNTER = CHILDREN_COUNTER + 1, MODIFY_TIME = GETDATE() 
			//				WHERE GUID = {0} ; 
			//
			//				SELECT * 
			//				FROM ORGANIZATIONS 
			//				WHERE GUID = {0}";
			//
			//			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strRootGuid));

			string strSql = @"
				UPDATE ORGANIZATIONS 
					SET CHILDREN_COUNTER = CHILDREN_COUNTER + 1, MODIFY_TIME = GETDATE() 
				WHERE ALL_PATH_NAME = {0} ; 

				SELECT * 
				FROM ORGANIZATIONS 
				WHERE ALL_PATH_NAME = {0}";

			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strParent, true));

			DataSet ds = InnerCommon.ExecuteDataset(strSql);

			ExceptionHelper.TrueThrow(((ds == null) || (ds.Tables.Count == 0) || (ds.Tables[0].Rows.Count == 0)), "�Բ���ϵͳ��û���ҵ����Ŷ���" + strParent + "��");
			DataRow row = ds.Tables[0].Rows[0];
			string strChildCounter = OGUCommonDefine.DBValueToString(row["CHILDREN_COUNTER"]);
			strInnerSort = AccreditResource.OriginalSortDefault.Substring(0, AccreditResource.OriginalSortDefault.Length - strChildCounter.Length) + strChildCounter;
			strRootGuid = (string)_RootHash[strParent];
			if (string.IsNullOrEmpty(strRootGuid))
			{
				_RootHash.Add(strParent, row["GUID"].ToString());
				strRootGuid = (string)_RootHash[strParent];
			}
			return row;
		}

		private bool CheckXsdExist(string strColumnName, string strObjClass)
		{
			XmlDocument xsdDoc;
			bool bResult = false;

			switch (strObjClass)
			{
				case "ORGANIZATIONS":
				case "GROUPS":
				case "USERS":
				case "OU_USERS":
					xsdDoc = GetXSDDocument(strObjClass);
					bResult = InnerCommon.GetXSDColumnNode(xsdDoc, strColumnName) != null;
					break;
			}
			return bResult;
		}

		private void CheckAllPathNameInSystem(string strAllPathName)
		{
			string strSql = @"
				SELECT * FROM ORGANIZATIONS WHERE ALL_PATH_NAME = {0};
				SELECT * FROM GROUPS WHERE ALL_PATH_NAME = {0};
				SELECT * FROM OU_USERS WHERE ALL_PATH_NAME = {0};";
			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strAllPathName, true));
			DataSet ds = InnerCommon.ExecuteDataset(strSql);
			foreach (DataTable table in ds.Tables)
			{
				ExceptionHelper.TrueThrow(table.Rows.Count > 0,
					"�Բ���ϵͳ���Ѿ������������Ķ���" + strAllPathName + "����\n\n���޸ģ��������ƣ����ٱ��棡");
			}
		}

		private void CheckImportPermission()
		{
			string strFuncCode = AccreditResource.Func_CreateOrg + "," + AccreditResource.Func_CreateGroup + "," + AccreditResource.Func_CreateUser;
			ExceptionHelper.FalseThrow(
				SecurityCheck.DoesUserHasPermissions(this.LogOnUserInfo.UserLogOnName,
													AccreditResource.AppCodeName,
													strFuncCode,
													UserValueType.LogonName,
													DelegationMaskType.All),
				"�Բ��������ڻ������������" + _RootAllPathName + "���е������ݣ�");

			DataSet dsScopes = SecurityCheck.GetUserFunctionsScopes(this.LogOnUserInfo.UserLogOnName,
													AccreditResource.AppCodeName,
													strFuncCode,
													UserValueType.LogonName,
													DelegationMaskType.All,
													ScopeMaskType.All);

			ExceptionHelper.FalseThrow(OGUWriter.IsObjectIsIncludeInObjects("ORGANIZATIONS",
				_RootAllPathName,
				SearchObjectColumn.SEARCH_ALL_PATH_NAME,
				string.Empty,
				dsScopes),
				"�Բ�����û��Ȩ���ڸû�����" + _RootAllPathName + "���д����µ��Ӷ���");
		}

		private void CheckImportUserRole()
		{
			string strRoles = OGUUserPermission.GetOGUPemission();
			if (false == (strRoles == "setNoPermission" ||
				strRoles.IndexOf(AccreditResource.Func_CreateOrg) > 0 ||
				strRoles.IndexOf(AccreditResource.Func_CreateGroup) > 0 ||
				strRoles.IndexOf(AccreditResource.Func_CreateUser) > 0))
			{
				btnOK.Visible = false;
				middleTD.InnerHtml = "�Բ�����������Ȩִ�иò�����";
			}
		}

		private void GetRootOrgAllPathName(DataTable oTable)
		{
			_RootAllPathName = oTable.Columns[0].ColumnName.Replace("��", "\\").Replace(".", "\\");

			Debug.WriteLine(_RootAllPathName, "Root");
		}

		private void GetColumnArray(DataTable oTable)
		{
			_DataColumns = new string[oTable.Columns.Count - 2][];

			DataRow row = oTable.Rows[0];
			for (int iCol = 2; iCol < oTable.Columns.Count; iCol++)
			{
				string strColumnName = oTable.Columns[iCol].ColumnName;
				string strText = row[iCol].ToString();
				string strValue = TransColumnName(strText);

				_DataColumns[iCol - 2] = new string[] { strValue, strText, strColumnName };
			}

			ExceptionHelper.TrueThrow(ContainsValue(0, "OBJ_NAME") < 0 || ContainsValue(0, "DISPLAY_NAME") < 0,
				"�Բ��𣬵��������б���������������ơ��͡���ʾ���ơ���");

			string strSql = @"SELECT * FROM RANK_DEFINE WHERE RANK_CLASS = 1 ORDER BY SORT_ID;
				SELECT * FROM RANK_DEFINE WHERE RANK_CLASS = 2 ORDER BY SORT_ID";

			_RankDefineDS = InnerCommon.ExecuteDataset(strSql, "ORGANIZATIONS", "USERS");
		}

		private string GetRankCode(string strObjClass, string strName)
		{
			string strResult = string.Empty;

			DataTable oTable = _RankDefineDS.Tables[strObjClass];
			DataRow[] oRows = oTable.Select("NAME=" + TSqlBuilder.Instance.CheckQuotationMark(strName, true));

			if (oRows.Length > 0)
				strResult = (string)oRows[0]["CODE_NAME"];
			else
				strResult = (string)oTable.Rows[oTable.Rows.Count - 2]["CODE_NAME"];

			return strResult;
		}

		private int ContainsValue(int iColumn, string strValue)
		{
			int iResult = -1;
			for (int iRow = 0; iRow < _DataColumns.Length; iRow++)
			{
				if (_DataColumns[iRow][iColumn] == strValue)
				{
					iResult = iRow;
					break;
				}
			}

			return iResult;
		}

		private string TransColumnName(string strOriginal)
		{
			if (_XmlParamElem == null)
				_XmlParamElem = GetXMLDocument("ExportOrImport").DocumentElement;

			XmlNode pNode = _XmlParamElem.SelectSingleNode("Option[Text=\"" + strOriginal + "\"]");

			ExceptionHelper.TrueThrow(pNode == null, "�Բ��𣬵����ļ��е������ơ�" + strOriginal + "������ȷ��");
			return pNode.SelectSingleNode("Value").InnerText;
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
