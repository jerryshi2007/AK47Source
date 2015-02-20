using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;
using System.Collections.Generic;

using MCS.Library.Data.Properties;

namespace MCS.Library.Data.SqlServer
{
	/// <summary>
	/// SQL Server���ݿ����
	/// </summary>
	public class SqlDatabase : Database
	{
		/// <summary>
		/// SQL Server �洢���̲����ı�ʶ����
		/// </summary>
		protected char parameterToken = '@';

		/// <summary>
		/// ͨ���߼����ƹ������ݿ����ʵ��
		/// </summary>
		/// <param name="name">�߼�����</param>
		public SqlDatabase(string name)
			: base(name)
		{
			this.factory = SqlClientFactory.Instance;
		}

		/// <summary>
		/// ����Command����ָ��洢���̻�ȡ������Ĳ�����
		/// </summary>
		/// <param name="discoveryCommand"></param>
		protected override void DeriveParameters(DbCommand discoveryCommand)
		{
			SqlCommandBuilder.DeriveParameters((SqlCommand)discoveryCommand);
		}

		/// <summary>
		/// ����ض����ݿ������£�Parameter��Command�е���ʼλ�á�
		/// </summary>
		/// <returns>��ʼ�±�</returns>
		protected override int UserParametersStartIndex()
		{
			return 1;
		}

		/// <summary>
		/// �������ݿ������ṩָ���Ĳ�������
		/// </summary>
		/// <remarks>Ϊ�������������ݿ��޹أ��������в������ƾ�ͨ���÷������в�������ƥ��</remarks>
		/// <param name="name">Ӧ�ö���Ĳ�������</param>
		/// <returns>����SQL Server�����������Ĳ�������</returns>
		public override string BuildParameterName(string name)
		{
			return (name[0] != parameterToken) ? Convert.ToString(parameterToken) + name : name;
		}

		/// <summary>
		/// �ж�Command��������Ĳ��������Ƿ������ֵ�������Ա����ƥ��
		/// </summary>
		/// <remarks>SQL Server��N+1:Nƥ��</remarks>
		/// <param name="command">Command����</param>
		/// <param name="values">����ֵ������</param>
		/// <returns>�Ƿ�ƥ��</returns>
		protected override bool SameNumberOfParametersAndValues(DbCommand command, object[] values)
		{
			return command.Parameters.Count - 1 == values.Length;   // SQL ServerĬ����һ�����ز���
		}

		/// <summary>
		/// ���ڴ洢���̣�������Function�����ؽ���Ĳ�������
		/// </summary>
		protected override string DefaultReturnValueParameterName
		{
			get
			{
				return "@RETURN_VALUE";
			}
		}

		/// <summary>
		/// ����һ��Parameter����ͬʱΪ�ڸ�ֵ
		/// </summary>
		protected override void ConfigureParameter(DbParameter parameter, 
			string parameterName, 
			DbType dbType,
			int size,
			ParameterDirection direction,
			bool nullable, 
			byte precision, 
			byte scale, 
			string sourceColumn, 
			DataRowVersion sourceVersion, 
			object value)
		{
			SqlParameter param = parameter as SqlParameter;
			param.ParameterName = parameterName;

			if ((dbType.Equals(DbType.Object)) && (value is byte[]))
			{
				param.SqlDbType = SqlDbType.Image;
			}
			else
			{
				param.DbType = dbType;
			}

			param.Size = size;
			param.Direction = direction;
			param.IsNullable = nullable;
			param.Precision = precision;
			param.Scale = scale;
			param.SourceColumn = sourceColumn;
			param.SourceVersion = sourceVersion;
			param.Value = (value == null) ? DBNull.Value : value;
		}

		/// <summary>
		/// ����һ��Parameter����
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="parameterName"></param>
		/// <param name="dbType"></param>
		/// <param name="size"></param>
		/// <param name="direction"></param>
		/// <param name="nullable"></param>
		/// <param name="sourceColumn"></param>
		/// <remarks>
		///     ���������������ӵķ���
		///     added by wangxiang . May 21, 2008
		/// </remarks>
		protected override void ConfigureParameter(DbParameter parameter, 
			string parameterName, 
			DbType dbType, 
			int size,
			ParameterDirection direction,
			bool nullable, 
			string sourceColumn)
		{
			SqlParameter param = parameter as SqlParameter;
			param.ParameterName = parameterName;

			if (dbType.Equals(DbType.Object))
			{
				param.SqlDbType = SqlDbType.Image;
			}
			else
			{
				param.DbType = dbType;
			}

			param.Size = size;
			param.Direction = direction;
			param.IsNullable = nullable;
			param.SourceColumn = sourceColumn;
		}

		/// <summary>
		/// ����һ��Parameter����
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="parameterName"></param>
		/// <param name="dbType"></param>
		/// <param name="size"></param>
		/// <param name="direction"></param>
		/// <param name="sourceColumn"></param>
		/// <remarks>
		///     ���������������ӵķ���
		///     added by wangxiang . May 21, 2008
		/// </remarks>
		protected override void ConfigureParameter(DbParameter parameter, 
			string parameterName, 
			DbType dbType,
			int size, 
			ParameterDirection direction, 
			string sourceColumn)
		{
			this.ConfigureParameter(parameter, parameterName, dbType, size, direction, true, sourceColumn);
		}


		#region region Batch Event Mechanism added by wangxiang . May 21, 2008

		/// <summary>
		/// ΪDataAdapter���¹��������¼�ί��
		/// </summary>
		/// <param name="adapter">Data Adapter</param>
		protected override void SetUpRowUpdatedEvent(DbDataAdapter adapter)
		{
			((SqlDataAdapter)adapter).RowUpdated +=
				new SqlRowUpdatedEventHandler(this.OnSqlRowUpdated);
		}

		/// <summary>
		/// �Լ�¼���¹��̵���Ӧ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="rowThatCouldNotBeWritten"></param>
		private void OnSqlRowUpdated(object sender, SqlRowUpdatedEventArgs rowThatCouldNotBeWritten)
		{
			if (rowThatCouldNotBeWritten.RecordsAffected == 0)
			{
				if (rowThatCouldNotBeWritten.Errors != null)
				{
					rowThatCouldNotBeWritten.Row.RowError = Resource.ExceptionMessageUpdateDataSetRowFailure;
					rowThatCouldNotBeWritten.Status = UpdateStatus.SkipCurrentRow;
				}
			}
		}

		#endregion
	}
}
