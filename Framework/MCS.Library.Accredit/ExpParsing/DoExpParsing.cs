using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Diagnostics;

using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.OguAdmin;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// UserDefineFunction ��ժҪ˵����
	/// </summary>
	public class DoExpParsing : IExpParsing
	{
		private static int _Level = 0;

		//private const string STR_CONN = "AccreditAdmin";
		private static Hashtable RankDefineSortHT = new Hashtable();//��������
		private static Hashtable RankDefineNameHT = new Hashtable();//����������

		/// <summary>
		/// ���캯��
		/// </summary>
		public DoExpParsing()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		static DoExpParsing()
		{
			string sql = "SELECT CODE_NAME, SORT_ID, NAME FROM RANK_DEFINE";
			DataTable RankDefineDT = OGUCommonDefine.ExecuteDataset(sql).Tables[0];

			foreach (DataRow row in RankDefineDT.Rows)
			{
				RankDefineSortHT.Add(row["CODE_NAME"], row["SORT_ID"]);
				RankDefineNameHT.Add(row["CODE_NAME"], row["NAME"]);
			}
		}

		#region �Զ��庯������
		/// <summary>
		/// �Զ��庯������
		/// </summary>
		/// <param name="strFuncName">��������</param>
		/// <param name="arrParams">������ʹ�õĲ�����</param>
		/// <param name="parseObj">������ʽ</param>
		/// <returns>���ʽ�������</returns>
		public object CalculateUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj)
		{
			return DoUserFunction(strFuncName, arrParams, parseObj);
		}

		/// <summary>
		/// У���û��Զ�����ʽ
		/// </summary>
		/// <param name="strFuncName">��������</param>
		/// <param name="arrParams">������ʹ�õĲ�����</param>
		/// <param name="parseObj">������ʽ</param>
		/// <returns>�û��Զ�����ʽ�Ľ������</returns>
		public object CheckUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj)
		{
			return DoUserFunction(strFuncName, arrParams, parseObj);
		}

		private static object DoUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj)
		{
			object oValue = null;

			switch (strFuncName.ToLower())
			{
				case "organizations":
				case "users":
				case "groups":
					oValue = strFuncName.ToUpper();
					break;
				//��Ա�����ж�
				case "userrank":
					ExceptionHelper.TrueThrow<ApplicationException>(arrParams.Length != 2, "����" + strFuncName + "������������!");
					if (arrParams.Length == 2)
					{
						//string 
						oValue = UserRank(AddDoubleQuotationMark(arrParams[0].Value.ToString()), arrParams[1].Value.ToString());
					}
					break;
				//��ǰ��ԱGuid
				case "curuserid":
					oValue = "001416f3-f108-454e-9033-62b5a300e347";
					break;

				//��ǰ��Ա����
				case "curuserrank":
					oValue = "NAVVY_U";
					break;
				//������
				case "curcustomsscope":
					//oValue = ObjectID(arrParams[0].Value.ToString());
					break;
				//������
				case "curdepartscope":
					//oValue = CurObjetBelongTo(arrParams[0].Value.ToString());
					break;
				//�Զ���������Χ
				case "userdefinescope":
					//oValue = CurRankLevel();
					break;

				//��Ա��������
				case "userrankname":
					oValue = UserRankName(arrParams[0].Value.ToString());
					break;
				//�õ����ʽ�ж���Ա������޶���Ϣ
				case "getuserrank":
					oValue = GetUserRank(arrParams[0].Value.ToString());
					break;

				//����ĳ����
				case "belongto":
					oValue = true;
					break;
				//�õ����ʽ�ж���Ա�������޶���Ϣ
				case "getbelongto":
					oValue = GetBelongTo(arrParams[0].Value.ToString());
					break;
				default:
					ExceptionHelper.TrueThrow<ApplicationException>(true, "�����ں���:" + strFuncName);
					break;
			}

			return oValue;
		}

		//��Ա��������
		private static string UserRankName(string strRank)
		{
			string strResult = "δ������Ա����";
			object obj = RankDefineNameHT[strRank];
			if (obj != null)
				strResult = obj.ToString();
			return strResult;
		}

		//��ñ��ʽ�ж������Ա�����޶���Ϣ
		private static string GetUserRank(string strExp)
		{
			ParseExpression pe = new ParseExpression();


			pe.OutputIdentifiers = true;
			pe.UserFunctions = (IExpParsing)new DoExpParsing();
			pe.ChangeExpression(strExp);

			ParseIdentifier pi = GetIdentifiers(pe.Identifiers, "UserRank");

			if (pi == null)
				return "";

			string strValue = String.Empty;

			//�õ��޶��ļ���
			pi = pi.SubIdentifier.NextIdentifier;
			strValue = pi.Identifier;


			//�õ�������
			pi = pi.NextIdentifier.NextIdentifier;
			strValue = pi.Identifier + "," + strValue;

			return strValue.Replace("\"", "");
		}

		//�õ����ʽ�ж���Ա�������޶���Ϣ
		private static string GetBelongTo(string strExp)
		{
			ParseExpression pe = new ParseExpression();


			pe.OutputIdentifiers = true;
			pe.UserFunctions = (IExpParsing)new DoExpParsing();
			pe.ChangeExpression(strExp);

			ParseIdentifier pi = GetIdentifiers(pe.Identifiers, "belongto");

			if (pi == null)
				return "";

			string strValue = String.Empty;

			//�õ����
			pi = pi.SubIdentifier.NextIdentifier;
			strValue = pi.Identifier.ToUpper();

			//�õ�objId
			pi = pi.NextIdentifier.NextIdentifier;
			strValue += "," + pi.Identifier;

			//�õ�parentId
			pi = pi.NextIdentifier.NextIdentifier;
			strValue += "," + pi.Identifier;

			return strValue.Replace("\"", "");
		}

		private static ParseIdentifier GetIdentifiers(ParseIdentifier pi, string strID)
		{
			while (pi != null)
			{
				if (pi.Identifier.ToLower() == strID.ToLower())
					return pi;

				if (pi.SubIdentifier != null)
				{
					ParseIdentifier piTemp = GetIdentifiers(pi.SubIdentifier, strID);
					if (piTemp != null)
						return piTemp;
				}
				pi = pi.NextIdentifier;
			}
			return null;
		}

		//��ǰ����
		private static string CurRankRank()
		{
			return "COMMON_U";
		}

		//����
		private static bool UserRank(string strRank, string op)
		{
			ParseExpression pe = new ParseExpression();

			pe.UserFunctions = (IExpParsing)new DoExpParsing();
			pe.ChangeExpression("curuserrank" + op + strRank);

			object oValue = pe.Value();

			return (bool)oValue;
		}


		#endregion �Զ��庯������


		#region ���ʽ������
		/// <summary>
		/// ��ȡ���ʽ��������Ϣ
		/// </summary>
		/// <param name="node">���ʽ����ڵ�</param>
		/// <param name="strB">��������</param>
		public static void GetDescription(EXP_TreeNode node, StringBuilder strB)
		{
			if (node != null)
			{
				switch (node.OperationID)
				{
					case Operation_IDs.OI_NUMBER:
					case Operation_IDs.OI_STRING:
					case Operation_IDs.OI_NEG:
					case Operation_IDs.OI_BOOLEAN:
					case Operation_IDs.OI_DATETIME:
						strB.Append(node.Value.ToString());
						break;
					case Operation_IDs.OI_ADD:
						GetBinOPDesp(node, "��", strB);
						break;
					case Operation_IDs.OI_MINUS:
						GetBinOPDesp(node, "��", strB);
						break;
					case Operation_IDs.OI_MUL:
						GetBinOPDesp(node, "����", strB);
						break;
					case Operation_IDs.OI_DIV:
						GetBinOPDesp(node, "����", strB);
						break;
					case Operation_IDs.OI_LOGICAL_OR:
						GetBinOPDesp(node, "����", strB);
						break;
					case Operation_IDs.OI_LOGICAL_AND:
						GetBinOPDesp(node, "����", strB);
						break;
					case Operation_IDs.OI_NOT:
						GetOPDesp(node.Right, "������", strB);
						break;
					case Operation_IDs.OI_GREAT:
						GetBinOPDesp(node, "����", strB);
						break;
					case Operation_IDs.OI_GREATEQUAL:
						GetBinOPDesp(node, "���ڵ���", strB);
						break;
					case Operation_IDs.OI_LESS:
						GetBinOPDesp(node, "С��", strB);
						break;
					case Operation_IDs.OI_LESSEQUAL:
						GetBinOPDesp(node, "С�ڵ���", strB);
						break;
					case Operation_IDs.OI_NOT_EQUAL:
						GetBinOPDesp(node, "������", strB);
						break;
					case Operation_IDs.OI_USERDEFINE:
						GetUserDefineDescription(node, strB);
						break;
				}
			}

		}
		private static void GetUserDefineDescription(EXP_TreeNode node, StringBuilder strB)
		{

			switch (node.FunctionName.ToLower())
			{
				//��Ա�����ж�
				case "userrank":
					if (node.Params.Count == 2)
					{
						string strTemp;
						strTemp = UserRankName(node.Params[0].ToString());

						//strB.Append("( ");
						strB.Append("�û�����Ա����");
						strB.Append(string.Format("{0}", ((EXP_TreeNode)node.Params[1]).Value.ToString()));
						strB.Append("'" + UserRankDsp((EXP_TreeNode)node.Params[0]) + "'");
						//strB.Append(" )");
					}
					else
						strB.Append("��Ա�����ж�������������!");
					break;
				//��ǰ��ԱGuid
				case "curuserid":
					break;
				//��ǰ��Ա����
				case "curuserrank":
					strB.Append("�û�����");
					break;
				//������
				case "curcustomsscope":
					break;
				//������
				case "curdepartscope":
					break;
				//�Զ���������Χ
				case "userdefinescope":
					break;
				//��Ա��������
				case "userrankname":
					break;
				case "getuserrank":
					break;
				case "organizations":
				case "users":
				case "groups":
					strB.Append(node.FunctionName.ToUpper());
					break;
				//����ĳ����
				case "belongto":
					if (node.Params.Count != 3)
					{
						strB.Append("BelongTo()������������!");
					}
					else
					{
						string strType = ((EXP_TreeNode)node.Params[0]).FunctionName.ToUpper();
						string strObjId = ((EXP_TreeNode)node.Params[1]).Value.ToString();
						string strParentId = ((EXP_TreeNode)node.Params[2]).Value.ToString();
						string strDsp = string.Empty;
						strB.Append(GetObjInfo(strType, strObjId, strParentId));
					}
					break;
				default:
					strB.Append("δ���庯��");
					strB.Append("( ");

					strB.Append(node.FunctionName);
					if (node.Params != null)
					{
						strB.Append("(");

						for (int i = 0; i < node.Params.Count; i++)
						{
							if (i > 0)
								strB.Append(",");
							GetDescription((EXP_TreeNode)node.Params[i], strB);
						}
						strB.Append(")");
					}
					strB.Append(" )");
					break;
			}
		}

		private static void GetBinOPDesp(EXP_TreeNode node, string strOP, StringBuilder strB)
		{
			strB.Append("(");
			GetDescription(node.Left, strB);
			strB.Append(" " + strOP + " ");
			GetDescription(node.Right, strB);
			strB.Append(")");
		}

		private static void GetOPDesp(EXP_TreeNode node, string strOP, StringBuilder strB)
		{
			strB.Append(" " + strOP + " ");
			GetDescription(node, strB);
		}

		private static string UserRankDsp(EXP_TreeNode node)
		{
			if (node.OperationID == Operation_IDs.OI_USERDEFINE)
			{
				if (node.FunctionName.ToLower() == "curuserrank")
					return "�û�����Ա����";
				else
					return "δ������Ա����";
			}
			else if (node.OperationID == Operation_IDs.OI_STRING)
			{
				return UserRankName(node.Value.ToString());
			}
			else return "δ������Ա����";
		}

		private static string GetObjInfo(string strType, string strObjId, string strParentId)
		{

			string strResult = string.Empty;

			string strSql = string.Empty;
			switch (strType.ToUpper())
			{
				case "USERS":
					strSql = "SELECT ALL_PATH_NAME FROM OU_USERS WHERE USER_GUID = {0} AND PARENT_GUID = {1}";
					break;
				case "ORGANIZATIONS":
					strSql = "SELECT ALL_PATH_NAME FROM ORGANIZATIONS WHERE GUID = {0} AND PARENT_GUID = {1}";
					break;
				case "GROUPS":
					strSql = "SELECT ALL_PATH_NAME FROM GROUPS WHERE GUID = {0} AND PARENT_GUID = {1}";
					break;
			}

			if (strSql != string.Empty)
			{
				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strObjId, true),
					TSqlBuilder.Instance.CheckQuotationMark(strParentId, true));
				object obj = OGUCommonDefine.ExecuteScalar(strSql);
				if (obj != null)
					strResult = obj.ToString();
			}

			if (strResult == string.Empty)
				strResult = string.Format("û���ҵ�{0}��Ϣ:\nobjId:{1}\nparentId:{2}\n", strType, strObjId, strParentId);

			return strResult;
		}


		#endregion ���ʽ������

		/// <summary>
		/// Ϊ�ַ���strValue�����˼���˫����,����strValue��ԭ�е�˫����,�������˫����
		/// </summary>
		/// <param name="strValue">�ַ���</param>
		/// <returns>�������˶���˫���ŵ��ַ���</returns>
		/// <remarks>
		/// Ϊ�ַ���strValue�����˼���˫����,����strValue��ԭ�е�˫����,�������˫����
		/// </remarks>
		public static string AddDoubleQuotationMark(string strValue)
		{
			return "\"" + strValue.Replace("\"", "\"\"") + "\"";
		}



		/// <summary>
		/// ��Debug�����������ֵ
		/// </summary>
		/// <param name="pi"></param>
		public static void OutputIdentifiers(ParseIdentifier pi)
		{
			while (pi != null)
			{
				Trace.WriteLine(new string('\t', _Level) + string.Format("ID:{0}\tPosition: {1}", pi.Identifier, pi.Position));

				if (pi.SubIdentifier != null)
				{
					_Level++;
					OutputIdentifiers(pi.SubIdentifier);
					_Level--;
				}
				pi = pi.NextIdentifier;
			}
		}


		/// <summary>
		/// ��������Ȩ����ı��ʽ
		/// </summary>
		/// <param name="strExpression">����Ȩ����ı��ʽ</param>
		/// <param name="objType">�������������Ȩ���������</param>
		/// <param name="objID">�������������Ȩ����ID</param>
		/// <param name="objParentID">�������������Ȩ����ĸ������ID</param>
		/// <param name="userAccessLevel">�������������Ȩ�������޶�����Ա����</param>
		/// <param name="pe">���ʽ������</param>
		public static void getObjInfo(string strExpression, out string objType, out string objID, out string objParentID, out string userAccessLevel, ParseExpression pe)
		{
			objType = string.Empty;
			objID = string.Empty;
			objParentID = string.Empty;
			userAccessLevel = string.Empty;

			string strTemp;
			string[] strList;
			strTemp = string.Format("GetBelongTo({0})", DoExpParsing.AddDoubleQuotationMark(strExpression));
			pe.ChangeExpression(strTemp);

			strList = pe.Value().ToString().Split(new char[] { ',', ';' });

			if (strList.Length > 0 && strList[0] != string.Empty)
				objType = strList[0];
			if (strList.Length > 1)
				objID = strList[1];
			if (strList.Length > 2)
				objParentID = strList[2];

			if (objType != "USERS")
			{
				strTemp = string.Format("GetUserRank({0})", DoExpParsing.AddDoubleQuotationMark(strExpression));
				pe.ChangeExpression(strTemp);
				strList = pe.Value().ToString().Split(new char[] { ',', ';' });
				if (strList.Length > 1)
				{
					userAccessLevel = strList[1];
				}
			}

		}

		/// <summary>
		/// ��������Ȩ����ı��ʽ
		/// </summary>
		/// <param name="strExpression">����Ȩ����ı��ʽ</param>
		/// <param name="objType">�������������Ȩ���������</param>
		/// <param name="objID">�������������Ȩ����ID</param>
		/// <param name="objParentID">�������������Ȩ����ĸ������ID</param>
		/// <param name="userAccessLevel">�������������Ȩ�������޶�����Ա����</param>
		public static void getObjInfo(string strExpression, out string objType, out string objID, out string objParentID, out string userAccessLevel)
		{
			ParseExpression pe = new ParseExpression();
			pe.UserFunctions = new DoExpParsing();
			getObjInfo(strExpression, out objType, out objID, out objParentID, out userAccessLevel, pe);
		}

	}
}
