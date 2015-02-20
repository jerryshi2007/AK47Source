using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Reflection
{
    using System.Linq.Expressions;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicFieldDelegation<T> : DynamicMemberDelegationBase<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="cases"></param>
        protected override void AddGetValueCaseExpression(Type type, ParameterExpression instance, IList<SwitchCase> cases)
        {
            foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                MemberExpression fieldValue = Expression.Field(Expression.Convert(instance, type), fieldInfo.Name);
                ConstantExpression fieldName = Expression.Constant(fieldInfo.Name, typeof(string));

                cases.Add(Expression.SwitchCase(
                    Expression.Convert(fieldValue, typeof(object)),
                    fieldName));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="newValue"></param>
        /// <param name="cases"></param>
        protected override void AddSetValueCaseExpression(Type type, ParameterExpression instance, ParameterExpression newValue, IList<SwitchCase> cases)
        {
            foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                MemberExpression fieldValue = Expression.Field(Expression.Convert(instance, type), fieldInfo.Name);
                BinaryExpression setValue = Expression.Assign(fieldValue, Expression.Convert(newValue, fieldInfo.FieldType));
                ConstantExpression fieldName = Expression.Constant(fieldInfo.Name, typeof(string));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), fieldName));
            }
        }
    }
}
