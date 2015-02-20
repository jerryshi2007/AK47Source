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
    public class DynamicPropertyDelegation<T> : DynamicMemberDelegationBase<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="cases"></param>
        protected override void AddGetValueCaseExpression(Type type, ParameterExpression instance, IList<SwitchCase> cases)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (propertyInfo.GetGetMethod(true) != null)
                {
                    MemberExpression propertyValue = Expression.Property(Expression.Convert(instance, type), propertyInfo.Name);
                    ConstantExpression propertyName = Expression.Constant(propertyInfo.Name, typeof(string));

                    cases.Add(Expression.SwitchCase(
                        Expression.Convert(propertyValue, typeof(object)),
                        propertyName));
                }
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
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (propertyInfo.GetSetMethod(true) != null)
                {
                    MemberExpression property = Expression.Property(Expression.Convert(instance, type), propertyInfo.Name);
                    BinaryExpression setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                    ConstantExpression propertyName = Expression.Constant(propertyInfo.Name, typeof(string));

                    cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyName));
                }
            }
        }
    }
}
