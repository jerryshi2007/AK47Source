#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	SlidingTimeDependency.cs
// Remark	��	���ʱ��������
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
    /// ���ʱ���������ͻ��˴����ڳ�ʼ������Ķ���ʱ����Ҫ�ṩһ��TimeSpan���͵Ĺ���ʱ��Σ�
    /// ���ӳ�ʼ��������󵽾�����ʱ���ʱ����Ϊ�����������ص�Cache����ڡ�
    /// </summary>
    public sealed class SlidingTimeDependency : DependencyBase
    {
        private TimeSpan cacheItemExpirationTime = TimeSpan.Zero;

        /// <summary>
        /// ��ȡ��ʼ��ʱ�趨�Ĺ���ʱ����
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\SlidingTimeDependencyTest.cs" region="CacheItemExpirationTimeTest" lang="cs" title="��ʼ��ʱ�趨�Ĺ���ʱ����" />
        /// </remarks>
        public TimeSpan CacheItemExpirationTime
        {
            get { return this.cacheItemExpirationTime;  }
        }

        #region ���캯��
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="expirationTime">����ʱ����</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\SlidingTimeDependencyTest.cs" region="ConstructorTest" lang="cs" title="���캯��" />
        /// </remarks>
        public SlidingTimeDependency(TimeSpan expirationTime)
        {
            this.cacheItemExpirationTime = expirationTime;

            //��������޸�ʱ���������ʱ��
            UtcLastModified = DateTime.UtcNow;
            UtcLastAccessTime = DateTime.UtcNow;
        }
        #endregion 
        
        /// <summary>
        /// ���ԣ���ȡ��Dependency�Ƿ����
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\SlidingTimeDependencyTest.cs" region="HasChangedTest" lang="cs" title="��Dependency�Ƿ����" />
        /// </remarks>
        public override bool HasChanged
        {
            get
            {
                return (DateTime.UtcNow - this.UtcLastModified) > this.CacheItemExpirationTime;
            }
        }
    }
}
