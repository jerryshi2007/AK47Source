using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// ���̵�״̬
    /// </summary>
    public enum WfProcessStatus
    {
        /// <summary>
        /// ������
        /// </summary>
        [EnumItemDescription("������", Define.DefaultCulture)]
        Running,

        /// <summary>
        /// �����
        /// </summary>
		[EnumItemDescription("�����", Define.DefaultCulture)]
        Completed,

        /// <summary>
        /// ����ֹ
        /// </summary>
		[EnumItemDescription("����ֹ", Define.DefaultCulture)]
        Aborted,

        /// <summary>
        /// δ����
        /// </summary>
		[EnumItemDescription("δ����", Define.DefaultCulture)]
        NotRunning
    }

	/// <summary>
	/// ����ļ��ط�ʽ
	/// </summary>
	public enum DataLoadingType
	{
		/// <summary>
		/// ��������ݴ��ڴ��м��أ�new��
		/// </summary>
		Memory,

		/// <summary>
		/// ��������ݴ��ⲿ���أ����ݿ⣩
		/// </summary>
		External
	}

    /// <summary>
    /// ����ʵ���Ľӿڶ���
    /// </summary>
    public interface IWfProcess
    {
        /// <summary>
        /// ���̵�ID
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        string ResourceID
        {
            get;
            set;
        }
        
        #region ����״̬��Ϣ
        /// <summary>
        /// ���̵�״̬
        /// </summary>
        WfProcessStatus Status
        {
            get;
        }

        /// <summary>
        /// ��������ʱ��
        /// </summary>
        DateTime StartTime
        {
            get;
        }

        /// <summary>
        /// ���̽�����Ϣ
        /// </summary>
        DateTime EndTime
        {
            get;
        }
        #endregion ����״̬��Ϣ

        #region �ڵ���Ϣ
        /// <summary>
        /// ��һ���ڵ�ʵ��
        /// </summary>
        IWfActivity FirstActivity
        {
            get;
        }

        /// <summary>
        /// ���һ���ڵ�ʵ��
        /// </summary>
        IWfActivity LastActivity
        {
            get;
        }

        /// <summary>
        /// ��ǰ�Ļ�ڵ�
        /// </summary>
        IWfActivity CurrentActivity
        {
            get;
        }

        /// <summary>
        /// �����нڵ��ʵ��
        /// </summary>
        WfActivityCollection Activities
        {
            get;
        }
        #endregion �ڵ���Ϣ

        #region ������Ϣ
		/// <summary>
		/// ��������Ϣ
		/// </summary>
		IWfProcess RootProcess
		{
			get;
		}

		/// <summary>
		/// ����ʵ���Ĺ���
		/// </summary>
		IWfFactory Factory
		{
			get;
		}

        /// <summary>
        /// 
        /// </summary>
        WfBranchProcessInfo EntryInfo
        {
            get;
        }

        /// <summary>
        /// ������
        /// </summary>
		IUser Creator
		{
			get;
			set;
		}

		/// <summary>
		/// ���̵Ĳ���
		/// </summary>
		IOrganization OwnerDepartment
		{
			get;
			set;
		}

        /// <summary>
        /// ����������
        /// </summary>
        WfProcessContext Context
        {
            get;
        }

		/// <summary>
		/// ���̵ļ��ط�ʽ
		/// </summary>
		DataLoadingType LoadingType
		{
			get;
		}

        /// <summary>
        /// �ڵ���ת
        /// </summary>
        /// <param name="transferParams"></param>
        /// <returns></returns>
        IWfActivity MoveTo(WfTransferParamsBase transferParams);

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="destinationActivityID"></param>
		void Withdraw(IWfActivity destinationActivity);

		/// <summary>
		/// ȡ������
		/// </summary>
		void CancelProcess();

		/// <summary>
		/// �õ����л�����Ϣ
		/// </summary>
		/// <param name="autoCalcaulatePath"></param>
		/// <returns></returns>
		WfActivityLevelGroupCollection GetAllLevels(bool autoCalcaulatePath);
        #endregion ������Ϣ
    }
}
