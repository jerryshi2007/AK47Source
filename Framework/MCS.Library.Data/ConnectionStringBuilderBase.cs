using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Accessories;
namespace MCS.Library.Data
{
    /// <summary>
    /// ���������������ļ������������ݿ�����Ӵ�����
    /// </summary>
    public class ConnectionStringElement
    {
        /// <summary>
        /// ���Ӵ��߼�����
        /// </summary>
        public string Name;

        /// <summary>
        /// ������������
        /// </summary>
        public string ProviderName;
        
		/// <summary>
        /// ���Ӵ�
        /// </summary>
        public string ConnectionString;
        
		/// <summary>
        /// ���ݷ����¼���������
        /// </summary>
        public string EventArgsType;

		/// <summary>
		/// Commandִ�еĳ�ʱʱ��
		/// </summary>
		public TimeSpan CommandTimeout = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// �������Ӵ���������Ļ���
    /// </summary>
    public abstract class ConnectionStringBuilderBase : BuilderBase<string>
    {
    }
}
