using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Expression
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExpressionExtensions
    {
        private static Dictionary<ExpressionDataType, Type> _DataTypeMappings = new Dictionary<ExpressionDataType, Type>(){
			{ExpressionDataType.Boolean, typeof(bool)},
			{ExpressionDataType.DateTime, typeof(DateTime)},
			{ExpressionDataType.Number, typeof(Decimal)},
			{ExpressionDataType.String, typeof(string)}
		};

        /// <summary>
        /// 将ExpressionDataType转换成.Net的数据类型
        /// </summary>
        /// <param name="pdt"></param>
        /// <returns></returns>
        public static Type ToRealType(this ExpressionDataType pdt)
        {
            Type result = typeof(string);

            TryToRealType(pdt, out result).FalseThrow("不支持ExpressionDataType的{0}类型转换为CLR的数据类型", pdt);

            return result;
        }

        /// <summary>
        /// 试图转换成真实的类型
        /// </summary>
        /// <param name="pdt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool TryToRealType(this ExpressionDataType pdt, out Type type)
        {
            type = typeof(string);

            return _DataTypeMappings.TryGetValue(pdt, out type);
        }
    }
}
