#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	PerformanceCounterInitData.cs
// Remark	��	��ʼ�����ܼ�����ʱ�Ĳ��� 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Core
{
    /// <summary>
    /// ��ʼ�����ܼ�����ʱ�Ĳ���
    /// </summary>
    public class PerformanceCounterInitData
    {
        private string categoryName = string.Empty;
        private string counterName = string.Empty;
        private string instanceName = string.Empty;
        private bool isReadonly = false;
        private string machineName = string.Empty;

        /// <summary>
        /// ���췽��
        /// </summary>
        public PerformanceCounterInitData()
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="categoryName">���ܼ����������</param>
        /// <param name="counterName">���ܼ�����������</param>
        public PerformanceCounterInitData(string categoryName, string counterName)
            : this(categoryName, counterName, string.Empty)
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
		/// <param name="perCategoryName">���ܼ����������</param>
		/// <param name="perCounterName">���ܼ�����������</param>
		/// <param name="perInstanceName">ʵ������</param>
        public PerformanceCounterInitData(string perCategoryName, string perCounterName, string perInstanceName)
        {
			this.categoryName = perCategoryName;
			this.counterName = perCounterName;
			this.instanceName = perInstanceName;
        }

        /// <summary>
        /// ���ܼ����������
        /// </summary>
        public string CategoryName
        {
            get { return this.categoryName; }
            set { this.categoryName = value; }
        }

        /// <summary>
        /// ���ܼ�����������
        /// </summary>
        public string CounterName
        {
            get { return this.counterName; }
            set { this.counterName = value; }
        }

        /// <summary>
        /// ʵ������
        /// </summary>
        public string InstanceName
        {
            get { return this.instanceName; }
            set { this.instanceName = value; }
        }

        /// <summary>
        /// �Ƿ���ֻ��
        /// </summary>
        public bool Readonly
        {
            get { return this.isReadonly; }
            set { this.isReadonly = value; }
        }

        /// <summary>
        /// ���ܼ��������ڵĻ�����
        /// </summary>
        public string MachineName
        {
            get { return this.machineName; }
            set { this.machineName = value; }
        }
    }
}
