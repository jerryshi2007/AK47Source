#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	SqlCaluseBuilderBase.cs
// Remark	��	Sql�Ӿ乹�����ĳ������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070824		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using MCS.Library.Core;

namespace MCS.Library.Data.Builder
{
	/// <summary>
	/// ����Sql��乹����Ļ���
	/// </summary>
	[Serializable]
	public abstract class SqlClauseBuilderItemBase
	{
		/// <summary>
		/// �õ�Data��Sql�ַ�������
		/// </summary>
		/// <param name="builder">������</param>
		/// <returns>���ؽ�data�����sql���Ľ��</returns>
		public abstract string GetDataDesp(ISqlBuilder builder);
	}

	/// <summary>
	/// �����ݵ�Sql��乹����Ļ���
	/// </summary>
	[Serializable]
	public class SqlCaluseBuilderItemWithData : SqlClauseBuilderItemBase
	{
		private static DataDescriptionGeneratorBase[] _DataDescriptors = new DataDescriptionGeneratorBase[]{
			NullDescriptionGenerator.Instance,
			ExpressionDescriptionGenerator.Instance,
			DateTimeDescriptionGenerator.Instance,
			DBNullDescriptionGenerator.Instance,
			BooleanDescriptionGenerator.Instance,
			StringDescriptionGenerator.Instance,
			EnumDescriptionGenerator.Instance,
			GuidDescriptionGenerator.Instance,
			BytesDescriptionGenerator.Instance,
			StreamDescriptionGenerator.Instance
		};

		private object data = null;
		private bool isExpression = false;

		/// <summary>
		/// ����
		/// </summary>
		public object Data
		{
			get { return this.data; }
			set { this.data = value; }
		}

		/// <summary>
		/// �������е�Data�Ƿ���sql���ʽ
		/// </summary>
		public bool IsExpression
		{
			get { return this.isExpression; }
			set { this.isExpression = value; }
		}

		/// <summary>
		/// �õ�Data��Sql�ַ�������
		/// </summary>
		/// <param name="builder">������</param>
		/// <returns>���ؽ�data�����sql���Ľ��</returns>
		public override string GetDataDesp(ISqlBuilder builder)
		{
			string result = string.Empty;

			DataDescriptionGeneratorBase generator = GetDataDescriptor(this);

			if (generator != null)
				result = generator.ToDescription(this, builder);
			else
				result = this.data.ToString();

			return result;
		}
		//public override string GetDataDesp(ISqlBuilder builder)
		//{
		//    string result = string.Empty;

		//    if (this.data == null || this.data is DBNull)
		//        result = "NULL";
		//    else
		//    {
		//        if (this.data is DateTime)
		//        {
		//            DateTime minDate = new DateTime(1753, 1, 1);

		//            if ((DateTime)this.data < minDate)
		//                result = "NULL";
		//            else
		//                result = builder.FormatDateTime((DateTime)this.data);
		//        }
		//        else if (this.data is System.Guid)
		//        {
		//            if ((Guid)this.data == Guid.Empty)
		//                result = "NULL";
		//            else
		//                result = builder.CheckUnicodeQuotationMark(this.data.ToString());
		//        }
		//        else
		//        {
		//            if (this.isExpression == false && (this.data is string || this.data.GetType().IsEnum))
		//                result = builder.CheckUnicodeQuotationMark(this.data.ToString());
		//            else
		//                if (this.data is bool)
		//                    result = ((int)Convert.ChangeType(this.data, typeof(int))).ToString();
		//                else
		//                {
		//                    if (this.data is byte[])
		//                        result = BytesToHexString((byte[])data);
		//                    else
		//                        if (this.data is Stream)
		//                            result = StreamToHexString((Stream)data);
		//                        else
		//                            result = this.data.ToString();
		//                }
		//        }
		//    }

		//    return result;
		//}

		private static DataDescriptionGeneratorBase GetDataDescriptor(SqlCaluseBuilderItemWithData buiderItem)
		{
			DataDescriptionGeneratorBase result = null;

			foreach (DataDescriptionGeneratorBase generator in _DataDescriptors)
			{
				if (generator.IsMatched(buiderItem))
				{
					result = generator;
					break;
				}
			}

			return result;
		}

		//private static string StreamToHexString(Stream stream)
		//{
		//    byte[] buffer = new byte[4096];

		//    StringBuilder strB = new StringBuilder(4096);

		//    using (BinaryReader br = new BinaryReader(stream))
		//    {
		//        int byteRead = br.Read(buffer, 0, buffer.Length);

		//        while (byteRead > 0)
		//        {
		//            for (int i = 0; i < byteRead; i++)
		//            {
		//                if (strB.Length == 0)
		//                    strB.Append("0X");

		//                strB.AppendFormat("{0:X2}", buffer[i]);
		//            }

		//            byteRead = br.Read(buffer, 0, buffer.Length);
		//        }
		//    }

		//    if (strB.Length == 0)
		//        strB.Append("NULL");

		//    return strB.ToString();
		//}

		//private static string BytesToHexString(byte[] data)
		//{
		//    StringBuilder strB = new StringBuilder(4096);

		//    if (data.Length > 0)
		//    {
		//        for (int i = 0; i < data.Length; i++)
		//        {
		//            if (strB.Length == 0)
		//                strB.Append("0X");

		//            strB.AppendFormat("{0:X2}", data[i]);
		//        }
		//    }
		//    else
		//        strB.Append("NULL");

		//    return strB.ToString();
		//}
	}

	/// <summary>
	/// In�����������
	/// </summary>
	[Serializable]
	public class SqlCaluseBuilderItemInOperator : SqlCaluseBuilderItemWithData
	{
	}

	/// <summary>
	/// �ʺ�INSERT��UPDATE��WHERE��ÿһ������������ֶ����ƺ��ֶε�ֵ������
	/// </summary>
	[Serializable]
	public class SqlClauseBuilderItemIUW : SqlCaluseBuilderItemWithData
	{
		private string operation = SqlClauseBuilderBase.EqualTo;

		/// <summary>
		/// ���췽��
		/// </summary>
		public SqlClauseBuilderItemIUW()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		private string dataField = string.Empty;

		/// <summary>
		/// Sql����е��ֶ���
		/// </summary>
		public string DataField
		{
			get { return this.dataField; }
			set
			{
				ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(value), "DataField���Բ���Ϊ�ջ���ַ���");
				this.dataField = value;
			}
		}

		/// <summary>
		/// �ֶκ�����֮��Ĳ�����
		/// </summary>
		public string Operation
		{
			get { return this.operation; }
			set { this.operation = value; }
		}
	}

	/// <summary>
	/// �ʺ�UPDATE��WHERE��ÿһ������������ֶ����ƺ��ֶε�ֵ������
	/// </summary>
	[Serializable]
	public class SqlClauseBuilderItemUW : SqlClauseBuilderItemIUW
	{
		private string template = string.Empty;

		/// <summary>
		/// ���ʽģ�塣���û���ṩ����ʹ��Ĭ��ģ��(${DataField}$ ${Operation}$ ${Data}$)��
		/// </summary>
		public string Template
		{
			get { return this.template; }
			set { this.template = value; }
		}

		/// <summary>
		/// Ĭ�ϵı��ʽģ��
		/// </summary>
		private const string DefaultTemplate = "${DataField}$ ${Operation}$ ${Data}$";

		/// <summary>
		/// ����ģ������SQL�Ӿ�
		/// </summary>
		/// <param name="strB"></param>
		/// <param name="builder"></param>
		internal void ToSqlString(StringBuilder strB, ISqlBuilder builder)
		{
			Regex reg = new Regex(@"\$\{[0-9 a-z A-Z]*?\}\$");

			string t = DefaultTemplate;

			if (this.template.IsNotEmpty())
				t = this.template;

			string replacedExp = reg.Replace(t, m =>
			{
				string paramName = m.Value.Substring(2, m.Length - 4);

				return GetParameterValue(paramName, builder);
			});

			strB.Append(replacedExp);
		}

		private string GetParameterValue(string paramName, ISqlBuilder builder)
		{
			string result = string.Empty;

			switch (paramName.ToLower())
			{
				case "datafield":
					result = this.DataField;
					break;
				case "operation":
					result = this.Operation;
					break;
				case "data":
					result = this.GetDataDesp(builder);
					break;
			}

			return result;
		}
	}

	/// <summary>
	/// ����������ʽ�Ĺ�����
	/// </summary>
	[Serializable]
	public class SqlClauseBuilderItemOrd : SqlClauseBuilderItemBase
	{
		/// <summary>
		/// 
		/// </summary>
		private FieldSortDirection sortDirection = FieldSortDirection.Ascending;
		/// <summary>
		/// 
		/// </summary>
		private string dataField = string.Empty;

		/// <summary>
		/// ���췽��
		/// </summary>
		public SqlClauseBuilderItemOrd()
		{
		}

		/// <summary>
		/// Sql����е��ֶ���
		/// </summary>
		public string DataField
		{
			get { return this.dataField; }
			set
			{
				ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(value), "DataField���Բ���Ϊ�ջ���ַ���");
				this.dataField = value;
			}
		}

		/// <summary>
		/// ������
		/// </summary>
		public FieldSortDirection SortDirection
		{
			get
			{
				return this.sortDirection;
			}
			set
			{
				this.sortDirection = value;
			}
		}

		/// <summary>
		/// �õ�Data��Sql�ַ�������
		/// </summary>
		/// <param name="builder">������</param>
		/// <returns>���ؽ�data�����sql���Ľ��</returns>
		public override string GetDataDesp(ISqlBuilder builder)
		{
			string result = string.Empty;

			if (this.sortDirection == FieldSortDirection.Descending)
				result = "DESC";

			return result;
		}

		/// <summary>
		/// ����SQL�Ӿ䣨���ֶ� ASC|DESC����
		/// </summary>
		/// <param name="strB"></param>
		/// <param name="builder"></param>
		internal void ToSqlString(StringBuilder strB, ISqlBuilder builder)
		{
			strB.Append(this.DataField);

			string desp = this.GetDataDesp(builder);

			if (false == string.IsNullOrEmpty(desp))
				strB.Append(" " + desp);
		}
	}

	/// <summary>
	/// �ֶε���������
	/// </summary>
	public enum FieldSortDirection
	{
		/// <summary>
		/// ����
		/// </summary>
		Ascending,

		/// <summary>
		/// ����
		/// </summary>
		Descending
	}

	/// <summary>
	/// �߼������
	/// </summary>
	public enum LogicOperatorDefine
	{
		/// <summary>
		/// ���롱����
		/// </summary>
		[EnumItemDescription(Description = "���롱����", ShortName = "AND")]
		And,

		/// <summary>
		/// ���򡱲���
		/// </summary>
		[EnumItemDescription(Description = "���򡱲���", ShortName = "OR")]
		Or
	}
}
