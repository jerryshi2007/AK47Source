using MCS.Library.Caching;
using MCS.Library.Core;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MCS.Library.Data
{
    /// <summary>
    /// �洢���̲�������
    /// </summary>
    public class DbParameterCache
    {
        #region Internal Type CacheMechanism
        private sealed class CacheMechanism : PortableCacheQueue<string, IDataParameter[]>
        {
            public static readonly CacheMechanism Instance = CacheManager.GetInstance<CacheMechanism>();

            private CacheMechanism() { }
        }
        #endregion

        private CacheMechanism cache = CacheMechanism.Instance;

        /// <summary>
        /// ������õĲ�����󶨵�ָ����Command������
        /// </summary>
        /// <param name="command">Command����</param>
        /// <param name="database">���ݿ�ʵ��</param>
        public void SetParameters(DbCommand command, Database database)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(command == null, "command");

            ExceptionHelper.TrueThrow<ArgumentNullException>(database == null, "database");

            if (cache.ContainsKey(CreateHashKey(command)))
            {
                foreach (IDataParameter parameter in GetCommandParameters(command))
                    command.Parameters.Add(parameter);
            }
            else
            {
                database.DiscoverParameters(command);   // ���������ֻ���ͨ��IOC����ʵ��Database�����
                cache.Add(CreateHashKey(command), Clone(command));
            }
        }

        /// <summary>
        /// ������õĲ�����󶨵�ָ����Command������
        /// </summary>
        /// <param name="command">Command����</param>
        /// <param name="database">���ݿ�ʵ��</param>
        public async Task SetParametersAsync(DbCommand command, Database database)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(command == null, "command");

            ExceptionHelper.TrueThrow<ArgumentNullException>(database == null, "database");

            if (cache.ContainsKey(CreateHashKey(command)))
            {
                foreach (IDataParameter parameter in GetCommandParameters(command))
                    command.Parameters.Add(parameter);
            }
            else
            {
                await database.DiscoverParametersAsync(command);   // ���������ֻ���ͨ��IOC����ʵ��Database�����
                cache.Add(CreateHashKey(command), Clone(command));
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        protected internal void Clear()
        {
            cache.Clear();
        }

        private static string CreateHashKey(IDbCommand command)
        {
            return command.Connection.ConnectionString + ":" + command.CommandText;
        }

        /// <summary>
        /// �ṩĳ�������һ������
        /// </summary>
        private static IDataParameter[] Clone(IDbCommand command)
        {
            IDataParameterCollection parameters = command.Parameters;
            IDataParameter[] parameterArray = new IDataParameter[parameters.Count];
            parameters.CopyTo(parameterArray, 0);
            return Clone(parameterArray);
        }

        /// <summary>
        /// �ṩĳ�������һ������
        /// </summary>
        /// <param name="originalParameters">��������</param>
        private static IDataParameter[] Clone(IDataParameter[] originalParameters)
        {
            IDataParameter[] result = new IDataParameter[originalParameters.Length];
            for (int i = 0; i < originalParameters.Length; i++)
                result[i] = (IDataParameter)((ICloneable)originalParameters[i]).Clone();
            return result;
        }

        /// <summary>
        /// ��ȡһ�黺��Ĳ���
        /// </summary>
        /// <param name="command">Command����</param>
        private IDataParameter[] GetCommandParameters(IDbCommand command)
        {
            IDataParameter[] cachedParameters = cache[CreateHashKey(command)];
            return Clone(cachedParameters);
        }
    }
}
