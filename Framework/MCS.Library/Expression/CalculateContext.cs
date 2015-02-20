using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Expression
{
    internal class CalculateContext
    {
        private bool optimize = true;

        public CalculateContext()
        {
        }

        public object BuiltInFunctionsWrapper
        {
            get;
            set;
        }

        public bool Optimize
        {
            get { return this.optimize; }
            set { this.optimize = value; }
        }

        public object CallerContxt
        {
            get;
            set;
        }

        public CalculateUserFunction CalculateUserFunction
        {
            get;
            set;
        }

        public object GetUserFunctionValue(string strFuncName, ParamObjectCollection arrParams)
        {
            object oValue = null;

            if (this.CalculateUserFunction != null)
            {
                oValue = this.CalculateUserFunction(strFuncName, arrParams, this.CallerContxt);
            }
            else
                if (this.BuiltInFunctionsWrapper != null)
                {
                    oValue = BuiltInFunctionHelper.ExecuteFunction(strFuncName, this.BuiltInFunctionsWrapper, arrParams, this.CallerContxt);
                }

            return oValue;
        }
    }
}
