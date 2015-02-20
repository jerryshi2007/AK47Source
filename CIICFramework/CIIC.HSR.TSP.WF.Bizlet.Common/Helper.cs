using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Common
{
    public static class Helper
    {
        /// <summary>
        /// 从键值对中找出指定的值
        /// </summary>
        /// <param name="dics">键值列表</param>
        /// <param name="key">待查找的键</param>
        /// <returns>对应的值，如果未找到返回空</returns>
        public static string GetDictionaryEntryValue(this DictionaryEntry[] dics,string key)
        {
            string value = string.Empty;
            if (null != dics)
            {
                foreach (DictionaryEntry keyValue in dics)
                {
                    if (keyValue.Key.ToString().Equals(key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        value = keyValue.Value.ToString();
                        break;
                    }
                }
            }
            return value;
        }
    }
}
