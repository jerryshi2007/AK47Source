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
	/// UserDefineFunction 的摘要说明。
	/// </summary>
	public class DoExpParsing : IExpParsing
	{
		private static int _Level = 0;

		//private const string STR_CONN = "AccreditAdmin";
		private static Hashtable RankDefineSortHT = new Hashtable();//级别排序
		private static Hashtable RankDefineNameHT = new Hashtable();//级别中文名

		/// <summary>
		/// 构造函数
		/// </summary>
		public DoExpParsing()
		{
			//
			// TODO: 在此处添加构造函数逻辑
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

		#region 自定义函数计算
		/// <summary>
		/// 自定义函数计算
		/// </summary>
		/// <param name="strFuncName">函数名称</param>
		/// <param name="arrParams">函数中使用的参数组</param>
		/// <param name="parseObj">表达解析式</param>
		/// <returns>表达式解析结果</returns>
		public object CalculateUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj)
		{
			return DoUserFunction(strFuncName, arrParams, parseObj);
		}

		/// <summary>
		/// 校验用户自定义表达式
		/// </summary>
		/// <param name="strFuncName">函数名称</param>
		/// <param name="arrParams">函数中使用的参数组</param>
		/// <param name="parseObj">表达解析式</param>
		/// <returns>用户自定义表达式的解析结果</returns>
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
				//人员级别判定
				case "userrank":
					ExceptionHelper.TrueThrow<ApplicationException>(arrParams.Length != 2, "函数" + strFuncName + "参数个数有误!");
					if (arrParams.Length == 2)
					{
						//string 
						oValue = UserRank(AddDoubleQuotationMark(arrParams[0].Value.ToString()), arrParams[1].Value.ToString());
					}
					break;
				//当前人员Guid
				case "curuserid":
					oValue = "001416f3-f108-454e-9033-62b5a300e347";
					break;

				//当前人员级别
				case "curuserrank":
					oValue = "NAVVY_U";
					break;
				//本关区
				case "curcustomsscope":
					//oValue = ObjectID(arrParams[0].Value.ToString());
					break;
				//本部门
				case "curdepartscope":
					//oValue = CurObjetBelongTo(arrParams[0].Value.ToString());
					break;
				//自定机构服务范围
				case "userdefinescope":
					//oValue = CurRankLevel();
					break;

				//人员级别名称
				case "userrankname":
					oValue = UserRankName(arrParams[0].Value.ToString());
					break;
				//得到表达式中对人员级别的限定信息
				case "getuserrank":
					oValue = GetUserRank(arrParams[0].Value.ToString());
					break;

				//属于某对象
				case "belongto":
					oValue = true;
					break;
				//得到表达式中对人员所属的限定信息
				case "getbelongto":
					oValue = GetBelongTo(arrParams[0].Value.ToString());
					break;
				default:
					ExceptionHelper.TrueThrow<ApplicationException>(true, "不存在函数:" + strFuncName);
					break;
			}

			return oValue;
		}

		//人员级别名称
		private static string UserRankName(string strRank)
		{
			string strResult = "未定义人员级别";
			object obj = RankDefineNameHT[strRank];
			if (obj != null)
				strResult = obj.ToString();
			return strResult;
		}

		//获得表达式中定义的人员级别限定信息
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

			//得到限定的级别
			pi = pi.SubIdentifier.NextIdentifier;
			strValue = pi.Identifier;


			//得到操作符
			pi = pi.NextIdentifier.NextIdentifier;
			strValue = pi.Identifier + "," + strValue;

			return strValue.Replace("\"", "");
		}

		//得到表达式中对人员所属的限定信息
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

			//得到类别
			pi = pi.SubIdentifier.NextIdentifier;
			strValue = pi.Identifier.ToUpper();

			//得到objId
			pi = pi.NextIdentifier.NextIdentifier;
			strValue += "," + pi.Identifier;

			//得到parentId
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

		//当前级别
		private static string CurRankRank()
		{
			return "COMMON_U";
		}

		//级别
		private static bool UserRank(string strRank, string op)
		{
			ParseExpression pe = new ParseExpression();

			pe.UserFunctions = (IExpParsing)new DoExpParsing();
			pe.ChangeExpression("curuserrank" + op + strRank);

			object oValue = pe.Value();

			return (bool)oValue;
		}


		#endregion 自定义函数计算


		#region 表达式的描述
		/// <summary>
		/// 获取表达式的描述信息
		/// </summary>
		/// <param name="node">表达式二叉节点</param>
		/// <param name="strB">描述内容</param>
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
						GetBinOPDesp(node, "加", strB);
						break;
					case Operation_IDs.OI_MINUS:
						GetBinOPDesp(node, "减", strB);
						break;
					case Operation_IDs.OI_MUL:
						GetBinOPDesp(node, "乘以", strB);
						break;
					case Operation_IDs.OI_DIV:
						GetBinOPDesp(node, "除以", strB);
						break;
					case Operation_IDs.OI_LOGICAL_OR:
						GetBinOPDesp(node, "或者", strB);
						break;
					case Operation_IDs.OI_LOGICAL_AND:
						GetBinOPDesp(node, "并且", strB);
						break;
					case Operation_IDs.OI_NOT:
						GetOPDesp(node.Right, "不存在", strB);
						break;
					case Operation_IDs.OI_GREAT:
						GetBinOPDesp(node, "大于", strB);
						break;
					case Operation_IDs.OI_GREATEQUAL:
						GetBinOPDesp(node, "大于等于", strB);
						break;
					case Operation_IDs.OI_LESS:
						GetBinOPDesp(node, "小于", strB);
						break;
					case Operation_IDs.OI_LESSEQUAL:
						GetBinOPDesp(node, "小于等于", strB);
						break;
					case Operation_IDs.OI_NOT_EQUAL:
						GetBinOPDesp(node, "不等于", strB);
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
				//人员级别判定
				case "userrank":
					if (node.Params.Count == 2)
					{
						string strTemp;
						strTemp = UserRankName(node.Params[0].ToString());

						//strB.Append("( ");
						strB.Append("用户的人员级别");
						strB.Append(string.Format("{0}", ((EXP_TreeNode)node.Params[1]).Value.ToString()));
						strB.Append("'" + UserRankDsp((EXP_TreeNode)node.Params[0]) + "'");
						//strB.Append(" )");
					}
					else
						strB.Append("人员级别判定函数参数有误!");
					break;
				//当前人员Guid
				case "curuserid":
					break;
				//当前人员级别
				case "curuserrank":
					strB.Append("用户级别");
					break;
				//本关区
				case "curcustomsscope":
					break;
				//本部门
				case "curdepartscope":
					break;
				//自定机构服务范围
				case "userdefinescope":
					break;
				//人员级别名称
				case "userrankname":
					break;
				case "getuserrank":
					break;
				case "organizations":
				case "users":
				case "groups":
					strB.Append(node.FunctionName.ToUpper());
					break;
				//属于某对象
				case "belongto":
					if (node.Params.Count != 3)
					{
						strB.Append("BelongTo()参数个数有误!");
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
					strB.Append("未定义函数");
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
					return "用户的人员级别";
				else
					return "未定义人员级别";
			}
			else if (node.OperationID == Operation_IDs.OI_STRING)
			{
				return UserRankName(node.Value.ToString());
			}
			else return "未定义人员级别";
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
				strResult = string.Format("没有找到{0}信息:\nobjId:{1}\nparentId:{2}\n", strType, strObjId, strParentId);

			return strResult;
		}


		#endregion 表达式的描述

		/// <summary>
		/// 为字符串strValue的两端加上双引号,并且strValue中原有的双引号,变成两个双引号
		/// </summary>
		/// <param name="strValue">字符串</param>
		/// <returns>返回两端都有双引号的字符串</returns>
		/// <remarks>
		/// 为字符串strValue的两端加上双引号,并且strValue中原有的双引号,变成两个双引号
		/// </remarks>
		public static string AddDoubleQuotationMark(string strValue)
		{
			return "\"" + strValue.Replace("\"", "\"\"") + "\"";
		}



		/// <summary>
		/// 在Debug窗口输出调试值
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
		/// 解析被授权对象的表达式
		/// </summary>
		/// <param name="strExpression">被授权对象的表达式</param>
		/// <param name="objType">输出参数，被授权对象的类型</param>
		/// <param name="objID">输出参数，被授权对象ID</param>
		/// <param name="objParentID">输出参数，被授权对象的父对象的ID</param>
		/// <param name="userAccessLevel">输出参数，被授权对象中限定的人员级别</param>
		/// <param name="pe">表达式解析类</param>
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
		/// 解析被授权对象的表达式
		/// </summary>
		/// <param name="strExpression">被授权对象的表达式</param>
		/// <param name="objType">输出参数，被授权对象的类型</param>
		/// <param name="objID">输出参数，被授权对象ID</param>
		/// <param name="objParentID">输出参数，被授权对象的父对象的ID</param>
		/// <param name="userAccessLevel">输出参数，被授权对象中限定的人员级别</param>
		public static void getObjInfo(string strExpression, out string objType, out string objID, out string objParentID, out string userAccessLevel)
		{
			ParseExpression pe = new ParseExpression();
			pe.UserFunctions = new DoExpParsing();
			getObjInfo(strExpression, out objType, out objID, out objParentID, out userAccessLevel, pe);
		}

	}
}
