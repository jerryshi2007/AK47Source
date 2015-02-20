using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// �ڵ��״̬
    /// </summary>
    public enum WfActivityStatus
    {
        /// <summary>
        /// ������
        /// </summary>
		[EnumItemDescription("������", Define.DefaultCulture)]
        Running,

        /// <summary>
        /// ��ɾ�������ڳ��ز�����
        /// </summary>
		[EnumItemDescription("��ɾ�������ڳ��ز�����", Define.DefaultCulture)]
        Deleted,

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
    }

    /// <summary>
    /// �ڵ�ʵ���Ľӿڶ���
    /// </summary>
    public interface IWfActivity
    {
        #region properties

        /// <summary>
        /// �ڵ��ID
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// �ڵ��������Ϣ
        /// </summary>
        IWfActivityDescriptor Descriptor
        {
            get;
        }

        /// <summary>
        /// �ڵ��������Ϣ
        /// </summary>
        IWfProcess Process
        {
            get;
        }

        /// <summary>
        /// �ڵ�״̬
        /// </summary>
        WfActivityStatus Status
        {
            get;
        }

        /// <summary>
        /// �ڵ���ʼʱ��
        /// </summary>
        DateTime StartTime
        {
            get;
        }

        /// <summary>
        /// �ڵ����ʱ��
        /// </summary>
        DateTime EndTime
        {
            get;
        }

        bool IsAborted
        {
            get;
        }

        /// <summary>
        /// ����
        /// </summary>
        IWfTransition FromTransition
        {
            get;
        }

        /// <summary>
        /// ����
        /// </summary>
        IWfTransition ToTransition
        {
            get;
        }

        /// <summary>
        /// �ڵ��������
        /// </summary>
        WfActivityContext Context
        {
            get;
        }


		WfAssigneeCollection Assignees
        {
            get;
        }

		IUser Operator
		{
			get;
			set;
		}

		/// <summary>
		/// �Ƿ��������̵ĵ�һ����
		/// </summary>
		bool IsFirstActivity
		{
			get;
		}

		/// <summary>
		/// �Ƿ��ǵ�ǰ���̵ĵ�һ����
		/// </summary>
		bool IsCurrentProcessFirstActivity
		{
			get;
		}

		/// <summary>
		/// �Ƿ��ǵ�ǰ���̵����һ����
		/// </summary>
		bool IsLastActivity
		{
			get;
		}

		/// <summary>
		/// ��Activity������Ƿ�֧���̣���������Ӧ�ĸ�AnchorActivity�����������Լ�
		/// </summary>
		IWfActivity RootActivity
		{
			get;
		}

		/// <summary>
		/// �������߶�������Ļ�㡣�п���Ϊnull��������ʼ�㡣
		/// </summary>
		IWfTransitionDescriptor FromTransitionDescriptor
		{
			get;
		}

		DataLoadingType LoadingType
		{
			get;
		}
        #endregion

        #region methods

        bool AbleToMoveTo();

        #endregion
    }
}
