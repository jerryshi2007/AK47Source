#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	GenericDbContext.cs
// Remark	��	Generic database processing context��
// -------------------------------------------------
// VERSION  	AUTHOR			DATE			CONTENT
// 1.0		    ����			20070430		����
//	1.1			ccic\yuanyong	20070725		��������internal string ConnName
// -------------------------------------------------
#endregion
#region using
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using System.Data;
using System.Data.Common;
using System.Web;

using MCS.Library.Core;
using MCS.Library.Data.Properties;
#endregion
namespace MCS.Library.Data
{
    /// <summary>
    /// Generic database processing context.
    /// <remarks>
    /// <list type="bullet">
    ///     <item>this context is attatch to current HttpContext(web app) or Thread CurrentContext property.</item>
    ///     <item>the primary goal is to harmonize the Transaction management in a call stack.</item>
    ///     <item>itself could be disposed automatically.</item>
    /// </list>
    /// </remarks>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCode]
    public abstract class DbContext : IDisposable
    {
        private bool _autoClose = true;
        private TimeSpan _commandTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// ��ȡһ��DbContext����
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="autoClose">�Ƿ��Զ��ر�</param>
        /// <returns>DbContext����</returns>
        public static DbContext GetContext(string name, bool autoClose)
        {
            //�õ�ӳ�����������
            name = DbConnectionMappingContext.GetMappedConnectionName(name);

            DbProviderFactory factory = DbConnectionManager.GetDbProviderFactory(name);

            DbConnectionStringBuilder csb = factory.CreateConnectionStringBuilder();

            csb.ConnectionString = DbConnectionManager.GetConnectionString(name);

            bool enlist = true;

            if (csb.ContainsKey("enlist"))
                enlist = (bool)csb["enlist"];

            DbContext result = null;

            if (enlist)
                result = new AutoEnlistDbContext(name, autoClose);
            else
                result = new NotEnlistDbContext(name, autoClose);

            result.InitDbContext(name, autoClose);

            result._autoClose = autoClose;
            result._commandTimeout = DbConnectionManager.GetCommandTimeout(name);

            return result;
        }

        /// <summary>
        /// ���ػ�ȡDbContext����
        /// </summary>
        /// <param name="name">��������</param>
        /// <returns></returns>
        public static DbContext GetContext(string name)
        {
            return GetContext(name, true);
        }

        #region Public ��Ա

        /// <summary>
        /// �Ƿ��Զ��ر�
        /// </summary>
        public bool AutoClose
        {
            get
            {
                return this._autoClose;
            }
            private set
            {
                this._autoClose = value;
            }
        }

        /// <summary>
        /// �������Ӷ���
        /// </summary>
        public abstract DbConnection Connection
        {
            get;
            internal protected set;
        }

        /// <summary>
        /// �����������
        /// </summary>
        public abstract DbTransaction LocalTransaction
        {
            get;
            internal protected set;
        }

        /// <summary>
        /// ������������
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// SQL����ĳ�ʱʱ��
        /// </summary>
        public TimeSpan CommandTimeout
        {
            get
            {
                return this._commandTimeout;
            }
            set
            {
                this._commandTimeout = value;
            }
        }

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// ��ʼ��DbContext
        /// </summary>
        /// <param name="name"></param>
        /// <param name="autoClose"></param>
        protected virtual void InitDbContext(string name, bool autoClose)
        {
        }

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
