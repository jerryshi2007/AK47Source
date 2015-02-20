#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	LogException.cs
// Remark	��	�Զ������־�쳣���ͣ����ڷ�װ�����쳣���������쳣������Ϊ��־�쳣���͡�
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
//	1.1		ccic\yuanyong		20080910		���ӱ�ǩ��Serializable��
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Logging
{
    /// <summary>
    /// ��־�쳣��
    /// </summary>
    /// <remarks>
    /// ��־ϵͳ�Զ����쳣����
    /// </remarks>
	//˵����
	//		1��һ������Ȼʵ���� ISerializable�ӿڣ��������û������[Serializable]����Ȼ���ᱻ���л���
	//		2��.NET����ʱ�������κ������� [Serializable]���ԵĶ���������л���
	//			���ͨ��.NET��ܶ����ȱʡ���л������ܹ�ʹһ���౻���л�����ô�����Ķ���Ҳ�ض�����ȷ�����л���
	//			�������Ҫ�Զ������л������������ͱ���ʵ��ISerializable�ӿڣ�ͬʱ����������[Serializable]���ԡ�
	[Serializable]
    public sealed class LogException : Exception
    {
        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="д��־���쳣����"></code>
        /// </remarks>
        public LogException()
            : base()
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="message">�쳣��Ϣ</param>
        /// <remarks>
        /// �����쳣��Ϣ��������־�쳣��
        /// </remarks>
        public LogException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// ���صĹ��캯��
        /// </summary>
        /// <param name="message">�쳣��Ϣ</param>
        /// <param name="exception">ԭʼ�쳣����</param>
        /// <remarks>
        /// ��ԭʼ�쳣��ת���LogException
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="д��־���쳣����"></code>
        /// </remarks>
        public LogException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
