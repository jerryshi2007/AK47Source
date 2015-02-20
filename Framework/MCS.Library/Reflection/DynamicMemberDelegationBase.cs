using MCS.Library.Core;
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
    public abstract class DynamicMemberDelegationBase<T> : IMemberAccessor, IMemberDelegation
    {
        private Func<object, string, object> GetValueDelegate;

        private Action<object, string, object> SetValueDelegate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public virtual object GetValue(T instance, string memberName)
        {
            return GetValueDelegate(instance, memberName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="newValue"></param>
        public virtual void SetValue(T instance, string memberName, object newValue)
        {
            SetValueDelegate(instance, memberName, newValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public virtual object GetValue(object instance, string memberName)
        {
            return GetValueDelegate(instance, memberName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="newValue"></param>
        public virtual void SetValue(object instance, string memberName, object newValue)
        {
            SetValueDelegate(instance, memberName, newValue);
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitDelegations()
        {
            this.GetValueDelegate = GenerateGetValue(typeof(T));
            this.SetValueDelegate = GenerateSetValue(typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Func<object, string, object> GenerateGetValue(Type type)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression memberName = Expression.Parameter(typeof(string), "memberName");

            ParameterExpression target = Expression.Variable(type, "target");
            BinaryExpression assignTarget = Expression.Assign(target, Expression.Convert(instance, type));

            List<SwitchCase> cases = new List<SwitchCase>();

            AddGetValueCaseExpression(type, target, cases);

            BlockExpression methodBody = null;

            if (cases.Any())
            {
                SwitchExpression switchEx = Expression.Switch(
                        memberName,
                        Expression.Constant(null),
                        cases.ToArray()
                    );

                methodBody = Expression.Block(typeof(object), new[] { target }, assignTarget, switchEx);
            }
            else
                methodBody = Expression.Block(typeof(object), new[] { target }, assignTarget, Expression.Constant(null));

            return Expression.Lambda<Func<object, string, object>>(methodBody, instance, memberName).Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Action<object, string, object> GenerateSetValue(Type type)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression memberName = Expression.Parameter(typeof(string), "memberName");
            ParameterExpression newValue = Expression.Parameter(typeof(object), "newValue");

            ParameterExpression target = Expression.Variable(type, "target");
            BinaryExpression assignTarget = Expression.Assign(target, Expression.Convert(instance, type));

            List<SwitchCase> cases = new List<SwitchCase>();

            AddSetValueCaseExpression(type, target, newValue, cases);

            BlockExpression methodBody = null;

            if (cases.Any())
            {
                SwitchExpression switchEx = Expression.Switch(
                        memberName,
                        Expression.Constant(null),
                        cases.ToArray());

                methodBody = Expression.Block(typeof(object), new[] { target }, assignTarget, switchEx);
            }
            else
                methodBody = Expression.Block(typeof(object), new[] { target }, assignTarget, Expression.Constant(null));

            return Expression.Lambda<Action<object, string, object>>(methodBody, instance, memberName, newValue).Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="cases"></param>
        protected abstract void AddGetValueCaseExpression(Type type, ParameterExpression instance, IList<SwitchCase> cases);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="newValue"></param>
        /// <param name="cases"></param>
        protected abstract void AddSetValueCaseExpression(Type type, ParameterExpression instance, ParameterExpression newValue, IList<SwitchCase> cases);
    }
}
