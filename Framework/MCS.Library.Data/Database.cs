using MCS.Library.Core;
using MCS.Library.Data.Properties;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MCS.Library.Data
{
    /// <summary>
    /// �������ݿ�ʵ����
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCode]
    public abstract partial class Database
    {
        #region Protected Fields
        /// <summary>
        /// ��ǰ���ݿ������߼�����
        /// </summary>
        private string name;
        #endregion

        #region Private Fields
        /// <summary>
        /// ���ݿ����湤��
        /// </summary>
        protected DbProviderFactory factory;//Modify By Yuanyong 20080320

        //private bool hasCustomizedDbEventArgs;      // �Ƿ�Ϊ����߼����ݿ��������¼�����
        private static DbParameterCache cache = new DbParameterCache(); // �̰߳�ȫ��
        #endregion

        #region Private Consts
        /// <summary>
        /// ϵͳĬ�����DataSet�е����ݱ�����
        /// </summary>
        /// <remarks>
        /// ϵͳĬ�����DataSet�е����ݱ�����(����)�����ڴ���ExecuteDataSet��LoadDataSet�е�TableName��������
        /// </remarks>
        protected const string SystemCreatedTableNameRoot = "Table";
        #endregion

        #region Public Fields
        /// <summary>
        /// ���ݿ��߼�����
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// ���ݿ����ִ��֮ǰ���¼�
        /// </summary>
        public event DbEventHandler BeforeExecution;
        /// <summary>
        /// ���ݿ����ִ��֮����¼�
        /// </summary>
        public event DbEventHandler AfterExecution;

        // ���ݿ���ó����쳣���¼���û���г������ǵ�ִ��Ч�ʣ�����Database���ƶԴ�����Ĳ����ǡ�ֱ���׳���
        #endregion

        #region Constructor
        /// <summary>
        /// ͨ���߼����ƹ������ݿ����ʵ��
        /// </summary>
        /// <param name="name"></param>
        protected Database(string name)
        {
            this.name = name;
            //this.factory = DbConnectionManager.GetDbProviderFactory(this.name); Del By Yuanyong 20080320
            //this.hasCustomizedDbEventArgs = DbConnectionManager.GetEventArgsType(this.name) == null ? false : true;
        }
        #endregion

        #region LoadDataSet
        /// <summary>
        /// ��DataSet�����SQL���صĽ��
        /// </summary>
        /// <remarks>�÷���Oracle��֧�֣� �����Ҫ��ѯ���ض��DataTable�뽫��д�ɴ洢����</remarks>
        /// <param name="commandType">Command����</param>
        /// <param name="commandText">Command��������</param>
        /// <param name="dataSet">������DataSet</param>
        /// <param name="pageNo">Ҫ�󷵻��������ڵ�ҳ�롾��0��ʼ��</param>
        /// <param name="pageSize">Ҫ�󷵻�����ÿһҳ�����������Ϊ�����ʾ�������ݡ�</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        public virtual void LoadDataSet(CommandType commandType,
            string commandText,
            DataSet dataSet,
            int pageNo,
            int pageSize,
            params string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                LoadDataSet(command, dataSet, pageNo, pageSize, tableNames);
            }
        }

        /// <summary>
        /// ��DataSet�����SQL���صĽ��
        /// </summary>
        /// <remarks>�÷���Oracle��֧�֣� �����Ҫ��ѯ���ض��DataTable�뽫��д�ɴ洢����</remarks>
        /// <param name="commandType">Command����</param>
        /// <param name="commandText">Command��������</param>
        /// <param name="dataSet">������DataSet</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        public virtual void LoadDataSet(CommandType commandType,
            string commandText,
            DataSet dataSet,
            params string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                LoadDataSet(command, dataSet, tableNames);
            }
        }

        /// <summary>
        /// ��DataSet�����洢���̷��صĽ��
        /// </summary>
        /// <remarks>�����Oracle��ѯ����Ҫ�ڶ���洢���̵�ʱ���REF CURSOR�������������ĺ���</remarks>
        /// <param name="storedProcedureName">�洢��������</param>
        /// <param name="dataSet">������DataSet</param>
        /// <param name="tableName">��ѯ�����DataTable����</param>
        /// <param name="parameterValues">�洢���̲�����ֵ</param>
        public virtual void LoadDataSet(string storedProcedureName,
            DataSet dataSet,
            string tableName,
            params object[] parameterValues)
        {
            LoadDataSet(storedProcedureName, dataSet, new string[] { tableName }, parameterValues);
        }

        /// <summary>
        /// ��DataSet�����Command���صĽ��
        /// </summary>
        /// <remarks>�����Oracle��ѯ����Ҫ�ڶ���洢���̵�ʱ���REF CURSOR�������������ĺ���</remarks>
        /// <param name="storedProcedureName">�洢��������</param>
        /// <param name="dataSet">������DataSet</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        /// <param name="parameterValues">�洢���̲�����ֵ</param>
        public virtual void LoadDataSet(string storedProcedureName,
            DataSet dataSet,
            string[] tableNames,
            params object[] parameterValues)
        {
            using (DbCommand command = InitStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                LoadDataSet(command, dataSet, tableNames);
            }
        }

        /// <summary>
        /// ��DataSet�����Command���صĽ��
        /// </summary>
        /// <param name="command">Commandʵ��(Ҫ���ʱCommand�����Connection�Ѿ����ò���ʼ��)</param>
        /// <param name="dataSet">������DataSet</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        public virtual void LoadDataSet(DbCommand command,
            DataSet dataSet,
            params string[] tableNames)
        {
            this.LoadDataSet(command, dataSet, 0, 0, tableNames);
        }

        /// <summary>
        /// ��DataSet�����Command���صĽ��
        /// </summary>
        /// <param name="command">Commandʵ��(Ҫ���ʱCommand�����Connection�Ѿ����ò���ʼ��)</param>
        /// <param name="dataSet">������DataSet</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        /// <param name="pageNo">Ҫ�󷵻��������ڵ�ҳ�롾��0��ʼ��</param>
        /// <param name="pageSize">Ҫ�󷵻�����ÿһҳ�����������Ϊ�����ʾ�������ݡ�</param>
        public virtual void LoadDataSet(DbCommand command,
            DataSet dataSet,
            int pageNo,
            int pageSize,
            params string[] tableNames)
        {
            this.DoLoadDataSet(command, dataSet, pageNo, pageSize, tableNames);
        }
        #endregion

        #region ExecuteDataSet
        /// <summary>
        /// ���ش洢���̲�ѯ���
        /// </summary>
        /// <param name="storedProcedureName">�洢��������</param>
        /// <param name="parameterValues">�洢���̲�����ֵ</param>
        /// <returns>��ѯ���</returns>
        public virtual DataSet ExecuteDataSet(string storedProcedureName, params object[] parameterValues)
        {
            return this.ExecuteDataSet(storedProcedureName, new string[] { SystemCreatedTableNameRoot }, parameterValues);
        }

        /// <summary>
        /// ���ش洢���̲�ѯ���
        /// </summary>
        /// <param name="storedProcedureName">�洢��������</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        /// <param name="parameterValues">�洢���̲�����ֵ</param>
        /// <returns>��ѯ���</returns>
        public virtual DataSet ExecuteDataSet(string storedProcedureName, string[] tableNames, params object[] parameterValues)
        {
            using (DbCommand command = InitStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return this.ExecuteDataSet(command, tableNames);
            }
        }

        /// <summary>
        /// ���ز�ѯ���
        /// </summary>
        /// <remarks>�÷���Oracle��֧�֣� �����Ҫ��ѯ���ض��DataTable�뽫��д�ɴ洢����</remarks>
        /// <param name="commandType">Command����</param>
        /// <param name="commandText">Command��������</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        /// <returns>��ѯ���</returns>
        public virtual DataSet ExecuteDataSet(CommandType commandType, string commandText, params string[] tableNames)
        {
            return this.ExecuteDataSet(commandType, commandText, 0, 0, tableNames);
        }

        /// <summary>
        /// ���ز�ѯ���
        /// </summary>
        /// <param name="command">Commandʵ��</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        /// <returns>��ѯ���</returns>
        public virtual DataSet ExecuteDataSet(DbCommand command, params string[] tableNames)
        {
            return this.ExecuteDataSet(command, 0, 0, tableNames);
        }

        /// <summary>
        /// ���ز�ѯ���
        /// </summary>
        /// <param name="commandType">Command����</param>
        /// <param name="commandText">Command��������</param>
        /// <param name="pageNo">Ҫ�󷵻��������ڵ�ҳ�롾��0��ʼ��</param>
        /// <param name="pageSize">Ҫ�󷵻�����ÿһҳ�����������Ϊ�����ʾ�������ݡ�</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        /// <returns>��ѯ���</returns>
        /// <remarks>�÷���Oracle��֧�֣� �����Ҫ��ѯ���ض��DataTable�뽫��д�ɴ洢����</remarks>
        public virtual DataSet ExecuteDataSet(CommandType commandType,
            string commandText,
            int pageNo,
            int pageSize,
            params string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return this.ExecuteDataSet(command, pageNo, pageSize, tableNames);
            }
        }

        /// <summary>
        /// ���ز�ѯ���
        /// </summary>
        /// <param name="command">Commandʵ��</param>
        /// <param name="pageNo">Ҫ�󷵻��������ڵ�ҳ�롾��0��ʼ��</param>
        /// <param name="pageSize">Ҫ�󷵻�����ÿһҳ�����������Ϊ�����ʾ�������ݡ�</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        /// <returns>��ѯ���</returns>
        public virtual DataSet ExecuteDataSet(DbCommand command, int pageNo, int pageSize, params string[] tableNames)
        {
            DataSet dataSet = new DataSet();

            dataSet.Locale = System.Globalization.CultureInfo.InvariantCulture;

            this.LoadDataSet(command, dataSet, pageNo, pageSize, tableNames);

            return dataSet;
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// ִ��Command���ص�ֵ
        /// </summary>
        /// <param name="command">Commandʵ��(Ҫ���ʱCommand�����Connection�Ѿ����ò���ʼ��)</param>
        /// <returns>��ֵ</returns>
        public virtual object ExecuteScalar(DbCommand command)
        {
            return DoExecuteScalar(command);
        }

        /// <summary>
        /// ִ�д洢���̷��ص�ֵ
        /// </summary>
        /// <param name="storedProcedureName">�洢��������</param>
        /// <param name="parameterValues">�洢���̲�����ֵ</param>
        /// <returns>��ֵ</returns>
        public virtual object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = InitStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return ExecuteScalar(command);
            }
        }

        /// <summary>
        /// ִ��ָ����ѯ���ص�ֵ
        /// </summary>
        /// <param name="commandType">Command����</param>
        /// <param name="commandText">SQL������SPName����commandTypeƥ��ʹ��</param>
        /// <returns>��ֵ</returns>
        public virtual object ExecuteScalar(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command);
            }
        }

        #endregion

        #region ExecuteReader
        /// <summary>
        /// ����һ��DataReader����
        /// </summary>
        /// <remarks>
        /// <list>
        ///     <item>���ڴ洢���̷�ʽ����DataReader������з��ز�ѯ��������</item>
        ///     <item>��Ҫ�ⲿӦ����ʾ�ر�Reader</item>
        /// </list>
        /// </remarks>
        /// <param name="command">����ʵ��</param>
        /// <returns>DataReader����</returns>
        public virtual DbDataReader ExecuteReader(DbCommand command)
        {
            return DoExecuteReader(command);
        }

        /// <summary>
        /// ����һ��DataReader����
        /// </summary>
        /// <remarks>
        /// <list>
        ///     <item>���ڴ洢���̷�ʽ����DataReader������з��ز�ѯ��������</item>
        ///     <item>��Ҫ�ⲿӦ����ʾ�ر�Reader</item>
        /// </list>
        /// </remarks>
        /// <param name="storedProcedureName">�洢��������</param>
        /// <param name="parameterValues">�洢���̲�����ֵ</param>
        /// <returns>DataReader����</returns>
        public DbDataReader ExecuteReader(string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = InitStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return ExecuteReader(command);
            }
        }

        /// <summary>
        /// ����һ��DataReader����
        /// </summary>
        /// <remarks>
        /// <list>
        ///     <item>���ڴ洢���̷�ʽ����DataReader������з��ز�ѯ��������</item>
        ///     <item>��Ҫ�ⲿӦ����ʾ�ر�Reader</item>
        /// </list>
        /// </remarks>
        /// <param name="commandType">Command����</param>
        /// <param name="commandText">SQL���</param>
        /// <returns>DataReader����</returns>
        public DbDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command);
            }
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// ִ�����ݸ��²���(DML)
        /// </summary>
        /// <param name="command">Commandʵ��(Ҫ���ʱCommand�����Connection�Ѿ����ò���ʼ��)</param>
        /// <returns>��Ӱ�������</returns>
        public virtual int ExecuteNonQuery(DbCommand command)
        {
            return DoExecuteNonQuery(command);
        }

        /// <summary>
        /// ִ�����ݸ��²���(DML)
        /// </summary>
        /// <param name="storedProcedureName">�洢��������</param>
        /// <param name="parameterValues">�洢���̲�����ֵ</param>
        /// <returns>��Ӱ�������</returns>
        public virtual int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = InitStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// ִ�����ݸ��²���(DML)
        /// </summary>
        /// <param name="commandType">Command����</param>
        /// <param name="commandText">SQL���</param>
        /// <returns>��Ӱ�������</returns>
        public virtual int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command);
            }
        }
        #endregion

        #region UpdateDataSet ���������������ӵķ��� added by wangxiang . May 21, 2008

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="dataSet">�����µ�����</param>
        /// <param name="tableName">��Ҫ���µ����ݱ�����</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="deleteCommand">ɾ������DbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns></returns>
        public int UpdateDataSet(UpdateBehavior behavior,
            DataSet dataSet, string tableName,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            return DoUpdateDataSet(behavior, dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="dataSet">�����µ�����</param>
        /// <param name="tableName">��Ҫ���µ����ݱ�����</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="deleteCommand">ɾ������DbCommand</param>
        /// <returns></returns>
        public int UpdateDataSet(UpdateBehavior behavior,
            DataSet dataSet,
            string tableName,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand)
        {
            return DoUpdateDataSet(behavior, dataSet, tableName, insertCommand, updateCommand, deleteCommand, null);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="dataSet">�����µ�����</param>
        /// <param name="tableName">��Ҫ���µ����ݱ�����</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="deleteCommand">ɾ������DbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns></returns>
        public int UpdateDataSet(DataSet dataSet,
            string tableName,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            return UpdateDataSet(UpdateBehavior.Transactional, dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="dataSet">�����µ�����</param>
        /// <param name="tableName">��Ҫ���µ����ݱ�����</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="deleteCommand">ɾ������DbCommand</param>
        /// <returns></returns>
        public int UpdateDataSet(DataSet dataSet,
            string tableName,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand)
        {
            return UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, null);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="deleteCommand">ɾ������DbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns></returns>
        public int UpdateDataSet(UpdateBehavior behavior,
            DataTable table,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            return DoUpdateDataSet(behavior, table, insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="deleteCommand">ɾ������DbCommand</param>
        /// <returns></returns>
        public int UpdateDataSet(UpdateBehavior behavior,
            DataTable table,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand)
        {
            return DoUpdateDataSet(behavior, table, insertCommand, updateCommand, deleteCommand, null);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="deleteCommand">ɾ������DbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns></returns>
        public int UpdateDataSet(DataTable table,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            return UpdateDataSet(UpdateBehavior.Transactional, table, insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="deleteCommand">ɾ������DbCommand</param>
        /// <returns></returns>
        public int UpdateDataSet(DataTable table, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            return UpdateDataSet(table, insertCommand, updateCommand, deleteCommand, null);
        }

        #region �����ķ���

        #region BatchInsert

        /// <summary>
        /// ����Insert
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns>Ӱ������</returns>
        public int BatchInsert(UpdateBehavior behavior, DataTable table, DbCommand insertCommand, int? updateBatchSize)
        {
            return UpdateDataSet(behavior, table, insertCommand, null, null, updateBatchSize);
        }

        /// <summary>
        /// ����Insert
        /// </summary>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns>Ӱ������</returns>
        public int BatchInsert(DataTable table, DbCommand insertCommand, int? updateBatchSize)
        {
            return UpdateDataSet(table, insertCommand, null, null, updateBatchSize);
        }

        /// <summary>
        /// ����Insert
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <returns>Ӱ������</returns>
        public int BatchInsert(UpdateBehavior behavior, DataTable table, DbCommand insertCommand)
        {
            return UpdateDataSet(behavior, table, insertCommand, null, null, null);
        }

        /// <summary>
        /// ����Insert
        /// </summary>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <returns>Ӱ������</returns>
        public int BatchInsert(DataTable table, DbCommand insertCommand)
        {
            return UpdateDataSet(table, insertCommand, null, null, null);
        }

        #endregion

        #region BatchUpdate

        /// <summary>
        /// ����Update
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns>Ӱ������</returns>
        public int BatchUpdate(UpdateBehavior behavior, DataTable table, DbCommand updateCommand, int? updateBatchSize)
        {
            return UpdateDataSet(behavior, table, null, updateCommand, null, updateBatchSize);
        }

        /// <summary>
        /// ����Update
        /// </summary>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns>Ӱ������</returns>
        public int BatchUpdate(DataTable table, DbCommand updateCommand, int? updateBatchSize)
        {
            return UpdateDataSet(table, null, updateCommand, null, updateBatchSize);
        }

        /// <summary>
        /// ����Update
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <returns>Ӱ������</returns>
        public int BatchUpdate(UpdateBehavior behavior, DataTable table, DbCommand updateCommand)
        {
            return UpdateDataSet(behavior, table, null, updateCommand, null, null);
        }

        /// <summary>
        /// ����Update
        /// </summary>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <returns>Ӱ������</returns>
        public int BatchUpdate(DataTable table, DbCommand updateCommand)
        {
            return UpdateDataSet(table, null, updateCommand, null, null);
        }

        #endregion

        #region BatchDelete

        /// <summary>
        /// ����Delete
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="BatchDelete">BatchDeleteDbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns>Ӱ������</returns>
        public int BatchDelete(UpdateBehavior behavior, DataTable table, DbCommand BatchDelete, int? updateBatchSize)
        {
            return UpdateDataSet(behavior, table, null, null, BatchDelete, updateBatchSize);
        }

        /// <summary>
        /// ����Delete
        /// </summary>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="BatchDelete">BatchDeleteDbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns>Ӱ������</returns>
        public int BatchDelete(DataTable table, DbCommand BatchDelete, int? updateBatchSize)
        {
            return UpdateDataSet(table, null, null, BatchDelete, updateBatchSize);
        }

        /// <summary>
        /// ����Delete
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="BatchDelete">BatchDeleteDbCommand</param>
        /// <returns>Ӱ������</returns>
        public int BatchDelete(UpdateBehavior behavior, DataTable table, DbCommand BatchDelete)
        {
            return UpdateDataSet(behavior, table, null, null, BatchDelete, null);
        }

        /// <summary>
        /// ����Delete
        /// </summary>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="BatchDelete">BatchDeleteDbCommand</param>
        /// <returns>Ӱ������</returns>
        public int BatchDelete(DataTable table, DbCommand BatchDelete)
        {
            return UpdateDataSet(table, null, null, BatchDelete, null);
        }

        #endregion

        #endregion

        #endregion

        #region Command and Stored Procedure Mechanism
        /// <summary>
        /// ����Command����ָ��洢���̻�ȡ������Ĳ�����
        /// <remarks>
        ///     ���������ֻ�����IOC��ʽ����ʵ��Database�����
        /// </remarks>
        /// </summary>
        protected abstract void DeriveParameters(DbCommand discoveryCommand);

        /// <summary>
        /// ���ڴ洢���̣�������Function����һ��ExecuteScalar�ķ��ؽ��������RETURN_VALUE�У�
        /// ����ͬ���ݿ�ò���������ͬ�������Ҫ�ɸ����ݿ�ʵ���Լ�ʵ�ָ�����
        /// </summary>
        protected abstract string DefaultReturnValueParameterName { get; }

        /// <summary>
        /// ����Command����ָ��洢���̻�ȡ������Ĳ�����
        /// </summary>
        internal void DiscoverParameters(DbCommand command)
        {
            if (command != null && command.CommandType == CommandType.StoredProcedure)
            {
                using (DbContext context = DbContext.GetContext(this.name))
                {
                    command.Connection = context.Connection;

                    using (DbCommand discoveryCommand = CreateCommandByCommandType(command))
                    {
                        discoveryCommand.Connection = command.Connection;

                        this.DeriveParameters(discoveryCommand);

                        foreach (IDataParameter parameter in discoveryCommand.Parameters)
                        {
                            IDataParameter cloneParameter = (IDataParameter)((ICloneable)parameter).Clone();
                            command.Parameters.Add(cloneParameter);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ��ȡִ��ָ���洢������Ҫ��Commandʵ��
        /// </summary>
        /// <param name="storedProcedureName">�洢��������</param>
        /// <param name="parameterValues">��Ӧ��һ�������ֵ</param>
        /// <returns>Commandʵ��</returns>
        /// <remarks>��ȡִ��ָ���洢������Ҫ��Commandʵ������ʱҪ�������ƥ����ȫ�����ڲ��Ĳ�����齫�����쳣��</remarks>
        ///// <param name="connection">���ݿ�Connectionʵ��</param>
        //protected virtual DbCommand GetStoredProcedureCommand(string storedProcedureName, DbConnection connection, params object[] parameterValues)
        public virtual DbCommand InitStoredProcedureCommand(string storedProcedureName, params object[] parameterValues)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(storedProcedureName), "storedProcedureName");

            DbCommand command = this.CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);

            cache.SetParameters(command, this);

            ExceptionHelper.FalseThrow<InvalidOperationException>(SameNumberOfParametersAndValues(command, parameterValues), "parameterValues");

            this.AssignParameterValues(command, parameterValues);

            return command;
        }

        /// <summary>
        /// ����һ���洢����Command����
        /// </summary>
        /// <param name="storedProcedureName">�洢��������</param>
        /// <returns>�ƶ���Command����</returns>
        /// <remarks>����һ���洢����Command���󣬽������ڴ����������κβ�����鴦��</remarks>
        public virtual DbCommand CreateStoredProcedureCommand(string storedProcedureName)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(storedProcedureName), "storedProcedureName");
            //if (string.IsNullOrEmpty(storedProcedureName)) throw new ArgumentNullException("storedProcedureName");
            return CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);
        }

        /// <summary>
        /// ���һ������ִ��SQL����Command����
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>Command����</returns>
        public DbCommand GetSqlStringCommand(string sql)
        {
            return CreateCommandByCommandType(CommandType.Text, sql);
        }

        /// <summary>
        /// �ж�Command��������Ĳ��������Ƿ������ֵ�������Ա����ƥ��
        /// </summary>
        /// <param name="command">Command����</param>
        /// <param name="values">����ֵ������</param>
        /// <returns>�Ƿ�ƥ��</returns>
        protected virtual bool SameNumberOfParametersAndValues(DbCommand command, object[] values)
        {
            return command.Parameters.Count == values.Length;
        }

        #endregion

        #region Direct Execution Methods
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="dataSet">�����µ�����</param>
        /// <param name="tableName">��Ҫ���µ����ݱ�����</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="deleteCommand">ɾ������DbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns></returns>
        /// <remarks>
        ///     ���������������ӵķ���
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        private int DoUpdateDataSet(UpdateBehavior behavior,
            DataSet dataSet,
            string tableName,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");

            return DoUpdateDataSet(behavior, dataSet.Tables[tableName], insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="behavior">������Ϊ</param>
        /// <param name="table">��Ҫ���µ����ݱ�</param>
        /// <param name="insertCommand">��������DbCommand</param>
        /// <param name="updateCommand">��������DbCommand</param>
        /// <param name="deleteCommand">ɾ������DbCommand</param>
        /// <param name="updateBatchSize">ÿ�����µ�������</param>
        /// <returns></returns>
        /// <remarks>
        ///     ���������������ӵķ���
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        private int DoUpdateDataSet(UpdateBehavior behavior,
            DataTable table,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            if (insertCommand == null && updateCommand == null && deleteCommand == null)
            {
                throw new ArgumentNullException("command");
            }

            using (DbDataAdapter adapter = GetDataAdapter(behavior))
            {
                IDbDataAdapter explicitAdapter = (IDbDataAdapter)adapter;

                if (insertCommand != null)
                {
                    explicitAdapter.InsertCommand = insertCommand;
                }
                if (updateCommand != null)
                {
                    explicitAdapter.UpdateCommand = updateCommand;
                }
                if (deleteCommand != null)
                {
                    explicitAdapter.DeleteCommand = deleteCommand;
                }

                if (updateBatchSize != null)
                {
                    adapter.UpdateBatchSize = (int)updateBatchSize;

                    if (insertCommand != null)
                        adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

                    if (updateCommand != null)
                        adapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;

                    if (deleteCommand != null)
                        adapter.DeleteCommand.UpdatedRowSource = UpdateRowSource.None;
                }

                int rows = adapter.Update(table);

                return rows;
            }
        }

        /// <summary>
        /// ִ��Command���ص�ֵ
        /// </summary>
        /// <param name="command">Commandʵ��</param>
        /// <returns>��ֵ</returns>
        private object DoExecuteScalar(DbCommand command)
        {
            ExceptionHelper.TrueThrow<ArgumentException>(command.CommandType == CommandType.TableDirect,
                Resource.ExecuteScalarNotSupportTableDirectException);

            using (DbContext context = DbContext.GetContext(this.name))
            {
                command.Connection = context.Connection;

                DoDbEvent(command, DbEventType.BeforeExecution);
                object returnValue = command.ExecuteScalar();

                if (command.CommandType != CommandType.Text)
                {
                    // ���� SQL Server ��Stored Procedure��Function����ֵ����ʽ�ϴ��ڲ�ͬ�������������Ӧ���޸�
                    returnValue = (returnValue == null) ? command.Parameters[DefaultReturnValueParameterName].Value : returnValue;
                }

                DoDbEvent(command, DbEventType.AfterExecution);

                return returnValue;
            }
        }

        private int DoExecuteNonQuery(DbCommand command)
        {
            //OpenConnection(command);
            using (DbContext context = DbContext.GetContext(this.name))
            {
                command.Connection = context.Connection;

                DoDbEvent(command, DbEventType.BeforeExecution);
                int rowsAffected = command.ExecuteNonQuery();
                DoDbEvent(command, DbEventType.AfterExecution);

                return rowsAffected;
            }
        }

        private DbDataReader DoExecuteReader(DbCommand command)
        {
            if (null == command.Connection)
                command.Connection = DbContext.GetContext(this.name, false).Connection;

            if (ConnectionState.Open != command.Connection.State)
                command.Connection.Open();

            DoDbEvent(command, DbEventType.BeforeExecution);
            DbDataReader reader = command.ExecuteReader();
            DoDbEvent(command, DbEventType.AfterExecution);

            return reader;
        }

        /// <summary>
        /// ��DataSet�����Command���صĽ��
        /// </summary>
        /// <param name="command">Commandʵ��(Ҫ���ʱCommand�����Connection�Ѿ����ò���ʼ��)</param>
        /// <param name="dataSet">������DataSet</param>
        /// <param name="tableNames">ÿ����ѯ�����DataTable����</param>
        /// <param name="pageNo">Ҫ�󷵻��������ڵ�ҳ�롾��0��ʼ��</param>
        /// <param name="pageSize">Ҫ�󷵻�����ÿһҳ�����������Ϊ�����ʾ�������ݡ�</param>
        private void DoLoadDataSet(DbCommand command, DataSet dataSet, int pageNo, int pageSize, string[] tableNames)
        {
            CheckTableNames(tableNames);

            using (DbContext context = DbContext.GetContext(this.name))
            {
                command.Connection = context.Connection;

                using (DbDataAdapter adapter = this.factory.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;

                    AddTableMappingsInAdapter(adapter, tableNames);

                    DoDbEvent(command, DbEventType.BeforeExecution);

                    if (pageSize == 0)
                        adapter.Fill(dataSet);
                    else
                    {
                        string srcTableName = tableNames.Length > 0 ? tableNames[0] : "Table";
                        adapter.Fill(dataSet, pageNo * pageSize, pageSize, srcTableName);
                    }

                    DoDbEvent(command, DbEventType.AfterExecution);
                }
            }
        }

        private static void CheckTableNames(string[] tableNames)
        {
            for (int i = 0; i < tableNames.Length; i++)
                ExceptionHelper.CheckStringIsNullOrEmpty(tableNames[i], "tableNames[" + i + "]");
        }

        private static void AddTableMappingsInAdapter(DbDataAdapter adapter, string[] tableNames)
        {
            for (int i = 0; i < tableNames.Length; i++)
            {
                string systemCreatedTableName = (i == 0) ? SystemCreatedTableNameRoot : SystemCreatedTableNameRoot + i;
                adapter.TableMappings.Add(systemCreatedTableName, tableNames[i]);
            }
        }
        #endregion

        #region Parameter Mechanism
        /// <summary>
        /// �������ݿ������ṩָ���Ĳ�������
        /// <remarks>
        ///     Ϊ�������������ݿ��޹أ��������в������ƾ�ͨ���÷������в�������ƥ�䡣
        ///     ����洢���̲���entryId, ��Oracle�в���entryId������SQL Server�в���@entryId
        /// </remarks>
        /// </summary>
        /// <param name="parameterName">Ӧ�ö���Ĳ�������</param>
        /// <returns>���ݲ�ͬ���ݿ������������Ĳ�������</returns>
        public virtual string BuildParameterName(string parameterName)
        {
            return parameterName;
        }

        /// <summary>
        /// Ϊһ��Parameter����ֵ
        /// </summary>
        /// <param name="command">��������</param>
        /// <param name="value">ֵ</param>
        /// <param name="parameterName">���ݿ��������</param>
        public virtual void SetParameterValue(DbCommand command, string parameterName, object value)
        {
            command.Parameters[BuildParameterName(parameterName)].Value = (value == null) ? DBNull.Value : value;
        }

        /// <summary>
        /// ��ȡһ��Parameter�����ֵ
        /// </summary>
        /// <param name="command">��������</param>
        /// <param name="parameterName">���ݿ��������</param>
        public virtual object GetParameterValue(DbCommand command, string parameterName)
        {
            return command.Parameters[BuildParameterName(parameterName)].Value;
        }

        /// <summary>
        /// ����ΪCommand�����ÿ��Parameter��ֵ
        /// </summary>
        /// <param name="command">��������</param>
        /// <param name="values">��Ҫ��ӵ�һ��ֵ</param>
        protected virtual void AssignParameterValues(DbCommand command, object[] values)
        {
            int parameterIndexShift = this.UserParametersStartIndex();

            for (int i = 0; i < values.Length; i++)
            {
                IDataParameter parameter = command.Parameters[i + parameterIndexShift];
                this.SetParameterValue(command, parameter.ParameterName, values[i]);
            }
        }

        /// <summary>
        /// ����һ��Parameter����ͬʱΪ�丳ֵ
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <param name="dbType">������Ӧ����������</param>
        /// <param name="size">�������ݳ���Ҫ��</param>
        /// <param name="direction">��������������ͣ�ö�٣�</param>
        /// <param name="nullable">�����Ƿ�����Ϊ��</param>
        /// <param name="precision">������ȷֵ</param>
        /// <param name="scale">������С</param>
        /// <param name="sourceColumn">��Ӧ��Source��Column</param>
        /// <param name="sourceVersion">��ӦSourceClumn�İ汾��</param>
        /// <param name="value">������������ֵ</param>
        /// <returns>׼���ò������ݵ�DBParameter����</returns>
        /// <remarks>����һ��Parameter����ͬʱΪ�ڸ�ֵ</remarks>
        protected DbParameter CreateParameter(string parameterName,
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
            DbParameter parameter = this.CreateParameter(parameterName);
            this.ConfigureParameter(parameter, parameterName, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);

            return parameter;
        }

        /// <summary>
        /// ����һ��Parameter����
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <param name="dbType">������Ӧ����������</param>
        /// <param name="size">�������ݳ���Ҫ��</param>
        /// <param name="direction">��������������ͣ�ö�٣�</param>
        /// <param name="nullable">�����Ƿ�����Ϊ��</param>
        /// <param name="sourceColumn">��Ӧ��Source��Column</param>
        /// <returns>����һ��Parameter����ͬʱΪ�ڸ�ֵ</returns>
        /// <remarks>
        ///     ���������������ӵķ���
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected DbParameter CreateParameter(string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            bool nullable,
            string sourceColumn)
        {
            DbParameter parameter = this.CreateParameter(parameterName);
            this.ConfigureParameter(parameter, parameterName, dbType, size, direction, nullable, sourceColumn);

            return parameter;
        }

        /// <summary>
        /// ����һ��Parameter����
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <param name="dbType">ִ������</param>
        /// <param name="size">��С</param>
        /// <param name="direction">��������</param>
        /// <param name="sourceColumn">������</param>
        /// <returns>����һ��Parameter����ͬʱΪ�ڸ�ֵ</returns>
        /// <remarks>
        ///     ���������������ӵķ���
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected DbParameter CreateParameter(string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            string sourceColumn)
        {
            DbParameter parameter = CreateParameter(parameterName);
            ConfigureParameter(parameter, parameter.ParameterName, dbType, size, direction, sourceColumn);
            return parameter;
        }

        /// <summary>
        /// ����һ��Parameter����
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <returns>����һ��Parameter����ͬʱΪ�ڸ�ֵ</returns>
        protected DbParameter CreateParameter(string parameterName)
        {
            DbParameter parameter = this.factory.CreateParameter();
            parameter.ParameterName = BuildParameterName(parameterName);
            return parameter;
        }

        /// <summary>
        /// ����ض����ݿ������£�Parameter��Command�е���ʼλ�á�
        /// </summary>
        /// <returns>��ʼ�±�</returns>
        protected virtual int UserParametersStartIndex()
        {
            return 0;
        }

        /// <summary>
        /// ����ָ��������ΪPrameter��ֵ
        /// </summary>
        /// <param name="parameter">����</param>
        /// <param name="parameterName">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="size">��С</param>
        /// <param name="direction">��������</param>
        /// <param name="nullable">�Ƿ�ɿ�</param>
        /// <param name="sourceColumn">����������</param>
        /// <remarks>
        ///     ���������������ӵķ���
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected virtual void ConfigureParameter(DbParameter parameter,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            bool nullable,
            string sourceColumn)
        {
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Size = size;
            parameter.Direction = direction;
            parameter.IsNullable = nullable;
            parameter.SourceColumn = sourceColumn;
        }

        /// <summary>
        /// ����ָ��������ΪPrameter��ֵ
        /// </summary>
        /// <param name="parameter">����</param>
        /// <param name="parameterName">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="size">��С</param>
        /// <param name="direction">��������</param>
        /// <param name="sourceColumn">����������</param>
        /// <remarks>
        ///     ���������������ӵķ���
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected virtual void ConfigureParameter(DbParameter parameter,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            string sourceColumn)
        {
            this.ConfigureParameter(parameter, parameterName, dbType, size, direction, true, sourceColumn);
        }

        /// <summary>
        /// ����ָ��������ΪPrameter��ֵ
        /// </summary>
        /// <param name="dbType">��������</param>
        /// <param name="direction">���������������</param>
        /// <param name="nullable">�Ƿ�ɿ�</param>
        /// <param name="parameter">����</param>
        /// <param name="parameterName">��������</param>
        /// <param name="precision">����</param>
        /// <param name="scale">��С</param>
        /// <param name="size">����</param>
        /// <param name="sourceColumn">����������</param>
        /// <param name="sourceVersion">�汾</param>
        /// <param name="value">ֵ</param>
        protected virtual void ConfigureParameter(DbParameter parameter,
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
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Value = (value == null) ? DBNull.Value : value;
            //parameter.Size = size;//modify by ccic\yuanyong 20080409
            parameter.Size = (size == 0 && dbType == DbType.String) ? value.ToString().Length : size;
            parameter.Direction = direction;
            parameter.IsNullable = nullable;
            parameter.SourceColumn = sourceColumn;
            parameter.SourceVersion = sourceVersion;
        }

        /// <summary>
        /// ����һ��Parameter
        /// </summary>
        /// <param name="dbType">��������</param>
        /// <param name="direction">���������������</param>
        /// <param name="nullable">�Ƿ�ɿ�</param>
        /// <param name="command">Sql���</param>
        /// <param name="parameterName">��������</param>
        /// <param name="precision">����</param>
        /// <param name="scale">��С</param>
        /// <param name="size">����</param>
        /// <param name="sourceColumn">����������</param>
        /// <param name="sourceVersion">�汾</param>
        /// <param name="value">ֵ</param>
        public virtual void AddParameter(DbCommand command,
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
            DbParameter parameter = this.CreateParameter(parameterName, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            command.Parameters.Add(parameter);
        }
        /// <summary>
        /// ����һ��Parameter
        /// </summary>
        /// <param name="dbType">��������</param>
        /// <param name="direction">���������������</param>
        /// <param name="command">Sql���</param>
        /// <param name="parameterName">��������</param>
        /// <param name="sourceColumn">����������</param>
        /// <param name="sourceVersion">�汾</param>
        /// <param name="value">ֵ</param>
        public void AddParameter(DbCommand command,
            string parameterName,
            DbType dbType,
            ParameterDirection direction,
            string sourceColumn,
            DataRowVersion sourceVersion,
            object value)
        {
            this.AddParameter(command, parameterName, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
        }

        /// <summary>
        /// ����һ��Parameter
        /// </summary>
        /// <param name="command">Sql���</param>
        /// <param name="parameterName">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="size">����</param>
        /// <param name="direction">��������</param>
        /// <param name="nullable">�Ƿ�ɿ�</param>
        /// <param name="sourceColumn">����������</param>
        /// <remarks>
        ///     ���������������ӵķ���
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        public virtual void AddParameter(DbCommand command,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            bool nullable,
            string sourceColumn)
        {
            DbParameter parameter = this.CreateParameter(parameterName, dbType, size, direction, nullable, sourceColumn);

            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// ����һ��Parameter
        /// </summary>
        /// <param name="command">Sql���</param>
        /// <param name="parameterName">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="size">����</param>
        /// <param name="direction">��������</param>
        /// <param name="sourceColumn">����������</param>
        /// <remarks>
        ///     ���������������ӵķ���
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        public virtual void AddParameter(DbCommand command,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            string sourceColumn)
        {
            DbParameter parameter = this.CreateParameter(parameterName, dbType, size, direction, sourceColumn);
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// ����һ��Out Parameter
        /// </summary>
        /// <param name="command">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="parameterName">���ݿ��������</param>
        /// <param name="size">����</param>
        public void AddOutParameter(DbCommand command, string parameterName, DbType dbType, int size)
        {
            this.AddParameter(command, parameterName, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value);
        }

        /// <summary>
        /// ����һ��In Parameter
        /// </summary>
        /// <param name="command">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="parameterName">���ݿ��������</param>
        public void AddInParameter(DbCommand command, string parameterName, DbType dbType)
        {
            this.AddInParameter(command, parameterName, dbType, null);
        }

        /// <summary>
        /// ����һ��In Parameter
        /// </summary>
        /// <param name="command">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="parameterName">���ݿ��������</param>
        /// <param name="value">ֵ</param>
        public void AddInParameter(DbCommand command, string parameterName, DbType dbType, object value)
        {
            this.AddParameter(command, parameterName, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// ����һ��In Parameter
        /// </summary>
        /// <param name="command">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="parameterName">���ݿ��������</param>
        /// <param name="sourceColumn">���ݿ�����������</param>
        /// <param name="sourceVersion">����</param>
        public void AddInParameter(DbCommand command, string parameterName,
            DbType dbType,
            string sourceColumn,
            DataRowVersion sourceVersion)
        {
            this.AddParameter(command, parameterName, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// ͨ���¼���¶�ڲ����������
        /// </summary>
        /// <remarks>���ڴ����Ӧ�ò���ʹ��������ԣ����������Ż��Ŀ��ǣ��Ȱ�Delegate����ǰ���ж�</remarks>
        /// <param name="command">�ڲ����������</param>
        /// <param name="eventType">���ݷ��ʵ���ʱ������</param>
        protected void DoDbEvent(IDbCommand command, DbEventType eventType)
        {
            //if (false == this.hasCustomizedDbEventArgs)
            //    return;

            if ((BeforeExecution != null) && (eventType == DbEventType.BeforeExecution))
                TriggerDbEvent(command, BeforeExecution);
            else if ((AfterExecution != null) && (eventType == DbEventType.AfterExecution))
                TriggerDbEvent(command, AfterExecution);
        }

        private void TriggerDbEvent(object executor, DbEventHandler targetHandler)
        {
            DbEventArgs args = new DbEventArgs(); //DbConnectionManager.GetEventArgsType(name);
            args.Executor = executor;
            targetHandler(this, args);     // �����ⲿ�¼�
        }
        #endregion

        #region Protected Helper Methods
        /// <summary>
        /// ���ݸ�����������Data Adapter
        /// </summary>
        /// <param name="updateBehavior">
        /// </param>        
        /// <returns>Data Adapter������</returns>
        /// <remarks>
        ///     ���������������ӵķ���
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected DbDataAdapter GetDataAdapter(UpdateBehavior updateBehavior)
        {
            DbDataAdapter adapter = factory.CreateDataAdapter();

            if (updateBehavior == UpdateBehavior.Continue)
            {
                this.SetUpRowUpdatedEvent(adapter);
            }

            return adapter;
        }

        /// <summary>
        /// ���ɼ򵥵�DbCommand���� 
        /// <remarks>
        ///     �൱��Provider Identpendent��new()��һ��Command����
        /// </remarks>
        /// </summary>
        /// <param name="commandText">Commandִ�����</param>
        /// <param name="commandType">Command����</param>
        /// <returns>Command����</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected DbCommand CreateCommandByCommandType(CommandType commandType, string commandText)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(commandText), "commandText");

            using (DbContext context = DbContext.GetContext(this.name))
            {
                DbCommand command = this.factory.CreateCommand();

                command.CommandTimeout = (int)context.CommandTimeout.TotalSeconds;
                command.CommandType = commandType;
                command.CommandText = commandText;
                command.Connection = context.Connection;
                command.Transaction = context.LocalTransaction;

                return command;
            }
        }

        /// <summary>
        /// ���ɼ򵥵�DbCommand���� 
        /// <remarks>
        ///     �൱��Provider Identpendent��new()��һ��Command����
        /// </remarks>
        /// </summary>
        /// <param name="originalCommand">��Ҫִ�е����ݿ����</param>
        /// <returns>Command����</returns>
        protected DbCommand CreateCommandByCommandType(IDbCommand originalCommand)
        {
            return this.CreateCommandByCommandType(originalCommand.CommandType, originalCommand.CommandText);
        }

        /// <summary>
        /// ΪDataAdapter���¹��������¼�ί��
        /// </summary>
        /// <param name="adapter">Data Adapter</param>
        /// <remarks>
        ///     ���������������ӵķ���
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected virtual void SetUpRowUpdatedEvent(DbDataAdapter adapter)
        {
        }

        #endregion
    }
}
