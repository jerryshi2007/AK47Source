using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Ogu
{
    public static class WfClientOguObjectExtenstions
    {
        // <summary>
        /// 判断对象是否为空(Null或ID为空)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this WfClientOguObjectBase obj)
        {
            return obj == null || string.IsNullOrEmpty(obj.ID);
        }

        /// <summary>
        /// 判断对象是否不为空(Null或ID为空)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this WfClientOguObjectBase obj)
        {
            return IsNullOrEmpty(obj) == false;
        }

        /// <summary>
        /// 判断两个对象是否一致。主要是比较ID。如果都是null，也返回true
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool AreSame(this WfClientOguObjectBase src, WfClientOguObjectBase target)
        {
            bool result = false;

            if (src == null && target == null)
                result = true;
            else
                if (src != null && target != null)
                    result = string.Compare(src.ID, target.ID, true) == 0;

            return result;
        }
    }
}
