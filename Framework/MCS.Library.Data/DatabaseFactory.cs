using System;

using MCS.Library.Core;
using MCS.Library.Data.SqlServer;

namespace MCS.Library.Data
{
    /// <summary>
    /// �������ݿ���߼����ƴ���Databaseʵ���Ĺ�����
    /// </summary>
    public static class DatabaseFactory
    {
        /// <summary>
        /// �������ݿ���߼����ƴ���Databaseʵ��
        /// </summary>
        /// <param name="name">���ݿ��߼�����</param>
        /// <returns>Databaseʵ��</returns>
        public static Database Create(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            //// ���������������ƶ�Ӧ�����ݿ�ʵ��
            //// ����Ժ���Ҫ����DeluxeWorks.Db�ṩ�Զ����ص�����Database���͵Ļ���Ҳ����Ҫ�޸��������
            //switch (DbConnectionManager.GetDbProviderName(name))
            //{
            //    case "System.Data.SqlClient":
            //        return new SqlDatabase(name);
            //    case "System.Data.OracleClient":
            //        return new OracleDatabase(name);
            //    //case "Oracle.DataAccess.Client":
            //    //    return new ODP.OracleDatabase(name);
            //    default:
            //        throw new NotSupportedException(name + " �����������������֧��");
            //}

            return DbConnectionManager.GetDataProvider(name) as Database;
        }

        /// <summary>
        /// ͨ��Context��ȡ���ݿ����ʵ��
        /// </summary>
        /// <param name="context">���������Ķ���</param>
        /// <returns>Databaseʵ��</returns>
        public static Database Create(DbContext context)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(context == null, "context");
            return Create(context.Name);
        }
    }
}
