#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	ILogFilter.cs
// Remark	��	�ӿڶ��壬��־�������Ľӿ�
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;
using MCS.Library.Accessories;

namespace MCS.Library.Logging
{
    /// <summary>
    /// �ӿڣ�������־������
    /// </summary>
#if DELUXEWORKSTEST
    public interface ILogFilter : IFilter<LogEntity>
#else
    internal interface ILogFilter : IFilter<LogEntity>
#endif
    {
        /// <summary>
        /// ����
        /// </summary>
        new string Name
        {
            get;
            set;
        }

        /// <summary>
        /// �ӿڷ�����ʵ����־����
        /// </summary>
        /// <param name="log">��־����</param>
        /// <returns>����ֵ��true��ͨ����false����ͨ��</returns>
        new bool IsMatch(LogEntity log);
    }
}
