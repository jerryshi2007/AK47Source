using System;
using System.Collections.Generic;
using System.Transactions;
using System.Configuration;

using MCS.Library.Configuration;
using MCS.Library.Data.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Data
{
    /// <summary>
    /// Ϊ�˶���TransactionScope�����ò�����Ƶ�ר�ù����ࡣ
    /// </summary>
    public static class TransactionScopeFactory
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.Parse("00:10:00");
        private const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;

        private static TransactionConfigurationSection GetConfiguration()
        {
            TransactionConfigurationSection section = ConfigurationBroker.GetSection("transactions") as TransactionConfigurationSection;

            if (section == null)//ϵͳ����Ĭ������
                section = new TransactionConfigurationSection();

            return section;
        }

        /// <summary>
        /// ������� TransactionScope ���õĳ�ʱ����
        /// </summary>
        public static TimeSpan Timeout
        {
            get
            {
                TransactionConfigurationSection section = GetConfiguration();
                if (section == null)
                    return DefaultTimeout;
                else
                    return section.Timeout;
            }
        }

        /// <summary>
        /// ������� TransactionScope ���õĽ��׸����
        /// </summary>
        public static IsolationLevel IsolationLevel
        {
            get
            {
                TransactionConfigurationSection section = GetConfiguration();
                if (section == null)
                    return DefaultIsolationLevel;
                else
                    return section.IsolationLevel;
            }
        }

        /// <summary>
        /// ����������ϰ��ύ�ɹ�����¼�
        /// </summary>
        /// <param name="action"></param>
        /// <param name="throwError">�Ƿ��׳��쳣</param>
        public static void AttachCommittedAction(Action<TransactionEventArgs> action, bool throwError)
        {
            if (Transaction.Current != null && action != null)
            {
                Transaction.Current.TransactionCompleted += (sender, e) =>
                {
                    if (e.Transaction.TransactionInformation.Status == TransactionStatus.Committed)
                    {
                        try
                        {
                            action(e);
                        }
                        catch (System.Exception)
                        {
                            if (throwError)
                                throw;
                        }
                    }
                };
            }
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create()
        {
            return Create(TransactionScopeOption.Required);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope CreateWithAsyncFlow()
        {
            return Create(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ�������ó�ʱʱ��
        /// </summary>
        /// <param name="timeout">��ʱʱ��</param>
        /// <param name="asyncFlowOption">�Ƿ�֧���첽</param>
        /// <returns></returns>
        public static TransactionScope Create(TimeSpan timeout, TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Suppress)
        {
            return Create(TransactionScopeOption.Required, timeout, TransactionScopeFactory.IsolationLevel, asyncFlowOption);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <param name="scopeOption">����Χ��ѡ��</param>
        /// <param name="asyncFlowOption">�Ƿ�֧���첽</param>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create(TransactionScopeOption scopeOption, TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Suppress)
        {
            return Create(scopeOption, TransactionScopeFactory.Timeout, TransactionScopeFactory.IsolationLevel, asyncFlowOption);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <param name="scopeOption">����Χ��ѡ��</param>
        /// <param name="scopeTimeout">TimeSpan��ʽ��ʾ�ĳ�ʱʱ��</param>
        /// <param name="asyncFlowOption">�Ƿ�֧���첽</param>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create(TransactionScopeOption scopeOption, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Suppress)
        {
            ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(scopeTimeout <= TimeSpan.Zero, "scopeTimeout");

            TransactionOptions options = new TransactionOptions();
            options.Timeout = scopeTimeout;
            options.IsolationLevel = TransactionScopeFactory.IsolationLevel;

            return Create(scopeOption, options, asyncFlowOption);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <param name="scopeOption">����Χ��ѡ��</param>
        /// <param name="scopeTimeout">��ʱʱ��</param>
        /// <param name="isolationLevel">���뼶��</param>
        /// <param name="asyncFlowOption">�Ƿ�֧���첽</param>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create(TransactionScopeOption scopeOption, TimeSpan scopeTimeout, IsolationLevel isolationLevel, TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Suppress)
        {
            if (scopeTimeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("scopeTimeout");

            // ��װTS����
            TransactionOptions options = new TransactionOptions();
            options.Timeout = scopeTimeout;
            options.IsolationLevel = isolationLevel;

            return Create(scopeOption, options, asyncFlowOption);
        }

        /// <summary>
        /// ����TransactionScope����ִ�в���
        /// </summary>
        /// <param name="action"></param>
        public static void DoAction(Action action)
        {
            DoAction(TransactionScopeOption.Required, action);
        }

        /// <summary>
        /// ����TransactionScope����ִ�в���
        /// </summary>
        /// <param name="scopeOption"></param>
        /// <param name="action"></param>
        public static void DoAction(TransactionScopeOption scopeOption, Action action)
        {
            if (action != null)
            {
                using (TransactionScope scope = Create(scopeOption))
                {
                    action();

                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <param name="scopeOption">����ѡ��</param>
        /// <param name="transactionOptions">���񸽼���Ϣ</param>
        /// <param name="asyncFlowOption">�Ƿ�֧���첽</param>
        /// <returns>�����Է�Χ����</returns>
        private static TransactionScope Create(TransactionScopeOption scopeOption, TransactionOptions transactionOptions, TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Suppress)
        {
            return new TransactionScope(scopeOption, transactionOptions, asyncFlowOption);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <param name="scopeOption">����Χ��ѡ��</param>
        /// <param name="isolationLevel">���뼶��</param>
        /// <param name="asyncFlowOption">�Ƿ�֧���첽</param>
        /// <returns>�����Է�Χ����</returns>
        private static TransactionScope Create(TransactionScopeOption scopeOption, IsolationLevel isolationLevel, TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Suppress)
        {
            return Create(scopeOption, TransactionScopeFactory.Timeout, isolationLevel, asyncFlowOption);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// ���� Transaction �Դ����׸����˵������˲��漰��������ȡ�ò����Ĳ���
        /// </summary>
        /// <param name="transactionToUse">����</param>
        /// <param name="scopeTimeout">��ʱ��Χ</param>
        /// <param name="asyncFlowOption">�Ƿ�֧���첽</param>
        /// <returns>�����Է�Χ����</returns>
        private static TransactionScope Create(Transaction transactionToUse, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Suppress)
        {
            if (scopeTimeout < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("scopeTimeout");

            return new TransactionScope(transactionToUse, scopeTimeout, asyncFlowOption);
        }
    }
}
