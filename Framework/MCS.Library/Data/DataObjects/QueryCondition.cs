using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Data.DataObjects
{
	/// <summary>
	/// ��װ��ҳ��ѯ�Ĳ�ѯ����
	/// </summary>
	[Serializable]
	public class QueryCondition
	{
		private string selectFields = string.Empty;
		private string fromClause = string.Empty;
		private string whereClause = string.Empty;
		private string orderByClause = string.Empty;
		private string groupByClause = string.Empty;
		private int rowIndex = 0;
		private int pageSize = 10;

		private string primaryKey = string.Empty;

		/// <summary>
		/// ���췽��
		/// </summary>
		public QueryCondition()
		{
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="rowIndex">�ӵڼ��п�ʼ</param>
		/// <param name="pageSize">ÿҳ������</param>
		/// <param name="select">sql��䷵�ص��ֶ�</param>
		/// <param name="from">sql����from����</param>
		/// <param name="orderBy">sql����orderBy����</param>
		public QueryCondition(int rowIndex, int pageSize, string select, string from, string orderBy)
		{
			this.rowIndex = rowIndex;
			this.pageSize = pageSize;
			this.selectFields = select;
			this.fromClause = from;
			this.orderByClause = orderBy;
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="rowIndex">�ӵڼ��п�ʼ</param>
		/// <param name="pageSize">ÿҳ������</param>
		/// <param name="select">sql��䷵�ص��ֶ�</param>
		/// <param name="from">sql����from����</param>
		/// <param name="orderBy">sql����orderBy����</param>
		/// <param name="where">sql����where����</param>
		public QueryCondition(int rowIndex, int pageSize, string select, string from, string orderBy, string where)
			: this(rowIndex, pageSize, select, from, orderBy)
		{
			this.whereClause = where;
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="rowIndex">�ӵڼ��п�ʼ</param>
		/// <param name="pageSize">ÿҳ������</param>
		/// <param name="select">sql��䷵�ص��ֶ�</param>
		/// <param name="from">sql����from����</param>
		/// <param name="orderBy">sql����orderBy����</param>
		/// <param name="where">sql����where����</param>
		/// <param name="key">�������</param>
		public QueryCondition(int rowIndex, int pageSize, string select, string from, string orderBy, string where,string key)
			: this(rowIndex, pageSize, select, from, orderBy,where)
		{
			this.PrimaryKey = key;
		}

		/// <summary>
		/// ÿҳ������
		/// </summary>
		public int PageSize
		{
			get { return this.pageSize; }
			set { this.pageSize = value; }
		}

		/// <summary>
		/// �ӵڼ��п�ʼ
		/// </summary>
		public int RowIndex
		{
			get { return this.rowIndex; }
			set { this.rowIndex = value; }
		}
		
		/// <summary>
		/// sql����orderBy����
		/// </summary>
		public string OrderByClause
		{
			get { return this.orderByClause; }
			set { this.orderByClause = value; }
		}

		/// <summary>
		/// sql����where����
		/// </summary>
		public string WhereClause
		{
			get { return this.whereClause; }
			set { this.whereClause = value; }
		}

		/// <summary>
		/// sql����from����
		/// </summary>
		public string FromClause
		{
			get { return this.fromClause; }
			set { this.fromClause = value; }
		}

		/// <summary>
		/// sql��䷵�ص��ֶ�
		/// </summary>
		public string SelectFields
		{
			get { return this.selectFields; }
			set { this.selectFields = value; }
		}

		/// <summary>
		/// �������
		/// </summary>
		public string PrimaryKey
		{
			get { return this.primaryKey; }
			set { this.primaryKey = value; }
		}

		/// <summary>
		/// GroupBy���֣�����Having����
		/// </summary>
		public string GroupBy
		{
			get { return this.groupByClause; }
			set { this.groupByClause = value; }
		}
	}
}
