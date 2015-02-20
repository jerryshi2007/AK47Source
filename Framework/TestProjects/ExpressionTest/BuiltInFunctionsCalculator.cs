using MCS.Library.Core;
using MCS.Library.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTest
{
    internal class BuiltInFunctionsCalculator
    {
        public static readonly BuiltInFunctionsCalculator Instance = new BuiltInFunctionsCalculator();

        private BuiltInFunctionsCalculator()
        {
        }

        public bool IsFunction(string funcName)
        {
            funcName.CheckStringIsNullOrEmpty("funcName");

            BuiltInFunctionInfoCollection funcsInfo = BuiltInFunctionHelper.GetBuiltInFunctionsInfo(this.GetType());

            return funcsInfo.Contains(funcName);
        }

        [BuiltInFunction("Abs", "取一个数字的绝对值")]
        public decimal AbsFunction(decimal a)
        {
            return Math.Abs(a);
        }

        [BuiltInFunction("Max", "取两个数字的最大值")]
        public decimal MaxFunction(decimal a, decimal b)
        {
            return Math.Max(a, b);
        }

        [BuiltInFunction("Min", "取两个数字的最小值")]
        public decimal MinFunction(decimal a, decimal b)
        {
            return Math.Min(a, b);
        }
    }
}
