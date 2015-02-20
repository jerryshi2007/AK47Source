using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixCondition : IWfMatrixCondition
    {
        /* 沈峥注释
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Id
        {
            get;
            set;
        }
         */

        public IWfMatrixParameterDefinition Parameter
        {
            get;
            set;
        }

        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// 比较符
        /// </summary>
        public ComparsionSign Sign
        {
            get;
            set;
        }

        /*沈峥注释
        /// <summary>
        /// 序号
        /// </summary>
        public int Sort
        {
            get;
            set;
        }
        */

        /// <summary>
        /// 转换为表达式
        /// </summary>
        /// <returns>表达式</returns>
        public string ToExpression()
        {
            return string.Format(" {0} {1} {2}", Parameter.Name, GetComparsion(Sign), GetValueExpression(this.Parameter, Value));
        }

        /// <summary>
        /// 根据值类型生成表达式右面的串
        /// </summary>
        /// <param name="parameter">表达式类型</param>
        /// <param name="parameter">比较值</param>
        /// <returns>值的表达式中形式</returns>
        public static string GetValueExpression(IWfMatrixParameterDefinition parameter, string value)
        {
            string formatValue = string.Empty;

            switch (parameter.ParameterType)
            {
                case ParaType.Number:
                    formatValue = value;
                    break;
                case ParaType.String:
                    formatValue = string.Format("\"{0}\"", value);
                    break;
            }

            return formatValue;
        }

    

        /// <summary>
        /// 获取比较运算符
        /// </summary>
        /// <param name="sign">运算符类型</param>
        /// <returns>字符表达式</returns>
        public static string GetComparsion(ComparsionSign sign)
        {
            string comparision = string.Empty;
            switch (sign)
            {
                case ComparsionSign.Equal:
                    comparision = "==";
                    break;
                case ComparsionSign.GreaterThan:
                    comparision = ">";
                    break;
                case ComparsionSign.GreaterThanAndEqual:
                    comparision = ">=";
                    break;
                case ComparsionSign.LessThan:
                    comparision = "<";
                    break;
                case ComparsionSign.LessThanAndEqual:
                    comparision = "<=";
                    break;
                case ComparsionSign.NotEqual:
                    comparision = "<>";
                    break;
            }

            return comparision;
        }
    }
}
