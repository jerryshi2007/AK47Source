using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.SOA.Contracts
{
    /// <summary>
    /// ���Ƶ��쳣��
    /// </summary>
    /// <remarks>���Ƶ��쳣��,�����쳣�������ǰ�˳�����ʾ������֧����Ϣ����ʾ��Ϣ������̳���ApplicationException�ࡣ
    /// </remarks>
    public class SOASupportException : ApplicationException
    {
        /// <summary>
        /// SystemSupportException��ȱʡ���캯��
        /// </summary>
        /// <remarks>SystemSupportException��ȱʡ���캯��.
        /// </remarks>
        public SOASupportException()
        {
        }

        /// <summary>
        /// SystemSupportException�Ĵ�������Ϣ�����Ĺ��캯��
        /// </summary>
        /// <param name="strMessage">������Ϣ��</param>
        /// <remarks>SystemSupportException�Ĵ�������Ϣ�����Ĺ��캯��,�ô�����Ϣ������Ϣ�׳��쳣ʱ��ʾ������
        /// <seealso cref="MCS.Library.Expression.ExpTreeExecutor"/>
        /// </remarks>
        public SOASupportException(string strMessage)
            : base(strMessage)
        {
        }

        /// <summary>
        /// SystemSupportException�Ĺ��캯����
        /// </summary>
        /// <param name="strMessage">������Ϣ��</param>
        /// <param name="ex">���¸��쳣���쳣</param>
        /// <remarks>�ù��캯���ѵ��¸��쳣���쳣��¼��������
        /// </remarks>
        public SOASupportException(string strMessage, Exception ex)
            : base(strMessage, ex)
        {
        }
    }
}
