using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using MCS.Library.Core;
using System.Collections.Generic;

namespace MCS.Library.Expression
{
    /// <summary>
    /// 内置函数说明
    /// </summary>
    public class BuiltInFunctionInfo
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        internal BuiltInFunctionInfo(BuiltInFunctionAttribute attribute, MethodInfo methodInfo)
        {
            attribute.NullCheck("attribute");
            methodInfo.NullCheck("methodInfo");

            this.FunctionName = attribute.FunctionName;
            this.Description = attribute.Description;
            this.MethodInfo = methodInfo;
        }

        /// <summary>
        /// 函数名称
        /// </summary>
        public string FunctionName
        {
            get;
            private set;
        }

        /// <summary>
        /// 函数说明
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// 方法的反射信息
        /// </summary>
        public MethodInfo MethodInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="target"></param>
        /// <param name="arrParams"></param>
        /// <param name="callerContext"></param>
        /// <returns></returns>
        public object ExecuteFunction(object target, ParamObjectCollection arrParams, object callerContext)
        {
            ParameterInfo[] methodParams = this.MethodInfo.GetParameters();

            CheckParametersCount(methodParams, arrParams);

            return this.MethodInfo.Invoke(target, PrepareParameters(methodParams, arrParams, callerContext));
        }

        private object[] PrepareParameters(IList<ParameterInfo> methodParams, ParamObjectCollection arrParams, object callerContext)
        {
            object[] result = new object[methodParams.Count];

            for (int i = 0; i < arrParams.Count; i++)
            {
                ParameterInfo pi = methodParams[i];

                try
                {
                    result[i] = DataConverter.ChangeType(arrParams[i].Value, pi.ParameterType);
                }
                catch (System.Exception ex)
                {
                    throw new InvalidCastException(string.Format("内置函数{0}参数{1}类型转换错误。{2}",
                        this.FunctionName, pi.Name, ex.Message), ex);
                }
            }

            if (result.Length == arrParams.Count + 1)
                result[arrParams.Count] = callerContext;

            return result;
        }

        private static void CheckParametersCount(ParameterInfo[] methodParams, ParamObjectCollection arrParams)
        {
            (methodParams.Length == arrParams.Count || methodParams.Length == arrParams.Count + 1).FalseThrow<ArgumentOutOfRangeException>(
                "内置的表达式方法的参数是{0}个，而调用参数是{1}个，不匹配", methodParams.Length, arrParams.Count);
        }
    }

    /// <summary>
    /// 内置的方法信息集合
    /// </summary>
    public class BuiltInFunctionInfoCollection : KeyedCollection<string, BuiltInFunctionInfo>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public BuiltInFunctionInfoCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(BuiltInFunctionInfo item)
        {
            return item.FunctionName;
        }
    }
}
