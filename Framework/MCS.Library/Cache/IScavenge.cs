#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	IScavenge.cs
// Remark	��	CacheQueue����ӿ���
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Caching
{
    /// <summary>
    /// �ӿڣ�����CacheQueueͨ��ʵ�ִ˽ӿ���������������
    /// </summary>
    public interface IScavenge
    {
        /// <summary>
        /// Cache����������
        /// </summary>
        void DoScavenging();
    }
}
