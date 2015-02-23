using MCS.Library.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Test.Expressions
{
    public class UserDictionaryImpl : IExpressionDictionaryCalculator
    {
        private static Dictionary<string, object> _UserInfoDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase){
			{ "Name", "沈峥" },
            { "Age", 42 },
            { "Birthday", new DateTime(1972, 4, 26) },
		};

        public object Calculate(string dictionaryName, string key, ExpressionDictionaryCalculatorContext context)
        {
            object oValue = null;

            _UserInfoDictionary.TryGetValue(key, out oValue);

            return oValue;
        }
    }
}
