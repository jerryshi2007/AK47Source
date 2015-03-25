#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	ContextCacheManager.cs
// Remark	��	�����������л����Cache��������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.ServiceModel;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
    /// <summary>
    /// �ڵ����������л����Cache����������Cache�������ڣ������ڵ�ǰ�̣߳�WinForm����һ��Http��Web��)���������
    /// </summary>
    public static class ContextCacheManager
    {
        private const string ContextCacheDictionaryKey = "MCS.Library.Caching.ContextCacheDictionary";

        [ThreadStatic]
        private static Dictionary<System.Type, ContextCacheQueueBase> cacheDictionary;

        /// <summary>
        /// ��ȡCache���е�ʵ��������ö���û�б����棬���Զ�̬����һ��ʵ��
        /// </summary>
        /// <typeparam name="T">Cache���е�����</typeparam>
        /// <returns>Cache���е�ʵ��</returns>
        public static T GetInstance<T>() where T : ContextCacheQueueBase
        {
            System.Type type = typeof(T);

            ContextCacheQueueBase instance;
            Dictionary<System.Type, ContextCacheQueueBase> cacheDict = GetCacheDictionary();

            if (cacheDict.TryGetValue(type, out instance) == false)
            {
                instance = (ContextCacheQueueBase)Activator.CreateInstance(typeof(T), true);
                cacheDict.Add(type, instance);
            }

            return (T)instance;
        }

        private static Dictionary<System.Type, ContextCacheQueueBase> GetCacheDictionary()
        {
            Dictionary<System.Type, ContextCacheQueueBase> cacheDict;

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                cacheDict = (Dictionary<System.Type, ContextCacheQueueBase>)HttpContext.Current.Items[ContextCacheDictionaryKey];
            else
                cacheDict = ContextCacheManager.cacheDictionary;

            if (cacheDict == null)
                cacheDict = new Dictionary<Type, ContextCacheQueueBase>();

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                HttpContext.Current.Items[ContextCacheDictionaryKey] = cacheDict;
            else
            {
                ContextCacheManager.cacheDictionary = cacheDict;

                if (OperationContext.Current != null)
                    OperationContext.Current.OperationCompleted += new EventHandler(Current_OperationCompleted);
            }

            return cacheDict;
        }

        /// <summary>
        /// ��WCF���������������ǰ�������Ļ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Current_OperationCompleted(object sender, EventArgs e)
        {
            ContextCacheManager.cacheDictionary = null;
        }
    }
}
