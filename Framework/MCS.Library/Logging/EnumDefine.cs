#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	EnumDefine.cs
// Remark	��	��־�����ö�����͵Ķ���
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Logging
{
    /// <summary>
    /// ��־��¼���ȼ�ö��
    /// </summary>
    /// <remarks>
    /// �����弶���ȼ�
    /// </remarks>
    public enum LogPriority
    {
        /// <summary>
        /// ������ȼ�
        /// </summary>
        Lowest,

        /// <summary>
        /// �����ȼ�
        /// </summary>
        BelowNormal,

        /// <summary>
        /// ��ͨ
        /// </summary>
        Normal,

        /// <summary>
        /// �����ȼ�
        /// </summary>
        AboveNormal,

        /// <summary>
        /// ������ȼ�
        /// </summary>
        Highest,
    }
}
