using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// ���ߵ�ʵ��
    /// </summary>
    public interface IWfTransition
    {
        /// <summary>
        /// �ߵ�ID
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// ���
        /// </summary>
        IWfActivity FromActivity
        {
            get;
        }

        /// <summary>
        /// �յ�
        /// </summary>
        IWfActivity ToActivity
        {
            get;
        }

        /// <summary>
        /// ��ʼʱ�� ???��Ҫô
        /// </summary>
        DateTime StartTime
        {
            get;
        }

        /// <summary>
        /// ���Ƿ�ȡ����ɾ����
        /// </summary>
        bool IsAborted
        {
            get;
        }
    }
}
