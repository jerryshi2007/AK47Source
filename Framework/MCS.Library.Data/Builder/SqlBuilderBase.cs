#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	SqlBuilderBase.cs
// Remark	��	ISqlBuilder��ʵ�ֻ���
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Data.Builder
{
	/// <summary>
	/// ISqlBuilder��ʵ�ֻ���
	/// </summary>
	public abstract class SqlBuilderBase : ISqlBuilder
	{
		#region ISqlBuilder ��Ա
		/// <summary>
		/// ��鲢�޸����ű��
		/// </summary>
		/// <param name="data">��Ҫ�����ַ���</param>
		/// <param name="addQuotation">�Ƿ��������</param>
		/// <returns>���ؼ�����ַ���</returns>
		public virtual string CheckQuotationMark(string data, bool addQuotation)
		{
			string result = data;

			if (data != null)
			{
				result = data.Replace("'", "''");

				if (addQuotation)
					result = "'" + result + "'";
			}

			return result;
		}

		/// <summary>
		/// ���е����ż�飬��������ַ������е����ţ���ô�滻�����������ţ���ֹע��ʽ������Ȼ����ͷβ�����һ�����š�Ȼ�����Unicode˵��
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public virtual string CheckUnicodeQuotationMark(string data)
		{
			return this.CheckQuotationMark(data, true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="addQuotation"></param>
		/// <returns></returns>
		public abstract string GetDBStringLengthFunction(string data, bool addQuotation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="addQuotation"></param>
		/// <returns></returns>
		public abstract string GetDBByteLengthFunction(string data, bool addQuotation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <param name="addQuotation"></param>
		/// <returns></returns>
		public abstract string GetDBSubStringStr(string data, int start, int length, bool addQuotation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public abstract string FormatDateTime(DateTime dt);

		/// <summary>
		/// ��ʽ��ȫ�ļ����ַ�����Ĭ�ϰ���SQL Server�Ĺ�������˫���š����ҽ��ո��滻���߼������
		/// </summary>
		/// <param name="logicOp">�߼��������Ĭ����AND</param>
		/// <param name="searchText"></param>
		/// <returns></returns>
		public string FormatFullTextString(LogicOperatorDefine logicOp, string searchText)
		{
			StringBuilder result = new StringBuilder();

			string[] parts = null;

			if (searchText.IsNotEmpty())
			{
				searchText = searchText.Replace("\"", "");
				parts = searchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			}
			else
				parts = new string[0];

			string op = EnumItemDescriptionAttribute.GetAttribute(logicOp).ShortName;

			foreach (string part in parts)
			{
				if (result.Length > 0)
					result.AppendFormat(" {0} ", op);

				result.AppendFormat("\"{0}\"", part);
			}

			return result.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="nullStr"></param>
		/// <param name="addQuotation"></param>
		/// <returns></returns>
		public abstract string DBNullToString(string data, string nullStr, bool addQuotation);

		/// <summary>
		/// 
		/// </summary>
		public abstract string DBCurrentTimeFunction
		{ get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract string DBStrConcatSymbol
		{ get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract string DBStatementBegin
		{ get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract string DBStatementEnd
		{ get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract string DBStatementSeperator
		{ get; }

		/// <summary>
		/// ��LIKE��Ӧ�Ĳ�ѯ�Ӿ�ת�塣������е�%��[��_ת��
		/// </summary>
		/// <param name="likeString"></param>
		/// <returns></returns>
		public string EscapeLikeString(string likeString)
		{
			string result = likeString;

			if (result.IsNotEmpty())
			{
				result = result.Replace("[", "[[]");
				result = result.Replace("%", "[%]");
				result = result.Replace("_", "[_]");
			}

			return result;
		}
		#endregion

		/// <summary>
		/// �������
		/// </summary>
		/// <param name="data">��Ҫ�������ַ���</param>
		/// <param name="addQuotation">�������</param>
		/// <returns></returns>
		protected virtual string AddQuotation(string data, bool addQuotation)
		{
			string result = data;

			if (addQuotation)
				result = CheckQuotationMark(data, addQuotation);

			return result;
		}
	}
}
