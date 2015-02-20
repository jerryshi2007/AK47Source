using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Expression;
using MCS.Library.Workflow.Engine;
using MCS.Library.Core;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WfConditionDescriptor
    {
        private string _Condition = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Condition
        {
            get
            {
                return this._Condition;
            }
            set
            {
                this._Condition = value;
            }
        }

		public bool Evaluate()
		{
			bool result = true;

			if (string.IsNullOrEmpty(this._Condition) == false)
			{
				try
				{
					result = (bool)ExpressionParser.Calculate(this._Condition, new CalculateUserFunction(WfContext.Current.OnCalculateUserFunction), null);
				}
				catch (System.Exception ex)
				{
					ExceptionHelper.FalseThrow(false, "表达式解析错误：({0})\n{1}", this._Condition, ex.Message);
				}
			}

			return result;
		}
    }
}
