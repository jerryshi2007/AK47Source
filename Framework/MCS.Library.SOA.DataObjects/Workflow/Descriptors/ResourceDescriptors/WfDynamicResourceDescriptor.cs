using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Expression;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    [Serializable]
    [XElementSerializable]
    public class WfDynamicResourceDescriptor : WfResourceDescriptor
    {
        private WfConditionDescriptor _Condition = null;

        public static readonly WfDynamicResourceDescriptor EmptyInstance = new WfDynamicResourceDescriptor();

        public WfDynamicResourceDescriptor()
        {
        }

        public WfDynamicResourceDescriptor(string name, string expression)
        {
            this.Name = name;
            this.Condition.Expression = expression;
        }

        /// <summary>
        /// 动态角色的名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public WfConditionDescriptor Condition
        {
            get
            {
                IWfKeyedDescriptor owner = GetOwner();

                if (this._Condition == null)
                    this._Condition = new WfConditionDescriptor(owner);
                else
                    if (this._Condition.Owner == null && owner != null)
                        this._Condition.Owner = owner;

                return this._Condition;
            }
            set
            {
                this._Condition = value;
                this._Condition.Owner = GetOwner();
            }
        }

        private IWfKeyedDescriptor GetOwner()
        {
            IWfKeyedDescriptor result = null;

            if (this.ProcessInstance != null)
                result = this.ProcessInstance.Descriptor;

            return result;
        }

        protected internal override void FillUsers(OguDataCollection<IUser> users)
        {
            try
            {
                users.CopyFrom(EvaluateCondition());
            }
            catch (WfDynamicResourceEvaluationException ex)
            {
                ex.WriteToLog();
            }
        }

        private IEnumerable<IUser> EvaluateCondition()
        {
            List<IUser> users = new List<IUser>();

            if (this.Condition.IsEmpty == false)
            {
                try
                {
                    object evaluateResult = this.Condition.EvaluateObject(new CalculateUserFunction(WfRuntime.ProcessContext.FireEvaluateDynamicResourceCondition));

                    if (evaluateResult != null)
                    {
                        if (evaluateResult is IUser)
                        {
                            AddUserToList((IUser)evaluateResult, users);
                        }
                        else
                            if (evaluateResult is IGroup)
                            {
                                AddUsersToList(((IGroup)evaluateResult).Members, users);
                            }
                            else
                            {
                                if (evaluateResult is IEnumerable && evaluateResult.GetType() != typeof(string))
                                    AddUsersToList((IEnumerable)evaluateResult, users);
                            }
                    }
                }
                catch (System.Exception ex)
                {
                    throw new WfDynamicResourceEvaluationException(string.Format("判断活动{0}的条件，{1}", this.Name, ex.Message), this.Condition);
                }
            }

            return users;
        }

        protected override void ToXElement(XElement element)
        {
            if (this._Condition != null)
                ((ISimpleXmlSerializer)this._Condition).ToXElement(element, "Condition");
        }

        private static void AddUsersToList(IEnumerable sourceUsers, List<IUser> targetUsers)
        {
            foreach (object user in sourceUsers)
            {
                if (user is IUser)
                    AddUserToList((IUser)user, targetUsers);
            }
        }

        private static void AddUserToList(IUser user, List<IUser> targetUsers)
        {
            if (OguBase.IsNotNullOrEmpty(user))
            {
                targetUsers.Add((IUser)OguBase.CreateWrapperObject(user));
            }
        }
    }

    /// <summary>
    /// 线上的条件计算所产生的异常
    /// </summary>
    [Serializable]
    public class WfDynamicResourceEvaluationException : WfEvaluationExceptionBase
    {
        /// <summary>
        /// 
        /// </summary>
        public WfDynamicResourceEvaluationException()
            : base()
        {
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="message"></param>
        public WfDynamicResourceEvaluationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="condition"></param>
        public WfDynamicResourceEvaluationException(string message, WfConditionDescriptor condition)
            : base(message, condition)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public WfDynamicResourceEvaluationException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="condition"></param>
        public WfDynamicResourceEvaluationException(string message, System.Exception innerException, WfConditionDescriptor condition)
            : base(message, innerException, condition)
        {
        }
    }
}
