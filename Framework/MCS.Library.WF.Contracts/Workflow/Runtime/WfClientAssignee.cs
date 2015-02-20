using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    [DataContract]
    [Serializable]
    public class WfClientAssignee
    {
        private bool _Selected = true;

        public WfClientAssignee()
        {
        }

        public WfClientAssignee(WfClientUser user)
        {
            this.User = user;
        }

        public WfClientAssigneeType AssigneeType
        {
            get;
            set;
        }

        public WfClientUser User
        {
            get;
            set;
        }

        public WfClientUser Delegator
        {
            get;
            set;
        }

        public bool Selected
        {
            get
            {
                return this._Selected;
            }
            set
            {
                this._Selected = value;
            }
        }

        public string Url
        {
            get;
            set;
        }

        public WfClientUserResourceDescriptor ToResource()
        {
            return new WfClientUserResourceDescriptor(this.User);
        }
    }

    [DataContract]
    [Serializable]
    public class WfClientAssigneeCollection : EditableDataObjectCollectionBase<WfClientAssignee>
    {
        public static readonly WfClientAssigneeCollection EmptyAssignees = new WfClientAssigneeCollection();

        /// <summary>
        /// 添加一组用户
        /// </summary>
        /// <param name="users"></param>
        public void Add(params WfClientUser[] users)
        {
            this.Add((IEnumerable<WfClientUser>)users);
        }

        /// <summary>
        /// 增加一组用户
        /// </summary>
        /// <param name="users"></param>
        public void Add(IEnumerable<WfClientUser> users)
        {
            if (users != null)
            {
                foreach (WfClientUser user in users)
                {
                    this.Add(new WfClientAssignee(user));
                }
            }
        }

        public bool Contains(WfClientUser target)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(target != null, "target");

            return this.Contains(target.ID);
        }

        public bool Contains(string userID)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(userID, "userID");

            return Find(assignee => (assignee.User != null) && string.Compare(assignee.User.ID, userID, true) == 0) != null;
        }

        /// <summary>
        /// 查找某个用户的委托人，如果没有找到，则返回自己
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public WfClientUser FindDelegator(WfClientUser user)
        {
            user.NullCheck("user");

            WfClientUser result = user;

            foreach (WfClientAssignee assignee in this)
            {
                if (assignee.User != null && string.Compare(user.ID, assignee.User.ID, true) == 0)
                {
                    if (assignee.Delegator != null)
                        result = assignee.Delegator;
                }
            }

            return result;
        }

        /// <summary>
        /// 根据userID找到第一个匹配的用户信息，如果同时存在委派和非委派的，返回非委派的
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public WfClientAssignee FindFirstUser(string userID)
        {
            userID.CheckStringIsNullOrEmpty("userID");

            IList<WfClientAssignee> assignees = FindAll(a => (a.User != null) && string.Compare(a.User.ID, userID, true) == 0);

            WfClientAssignee result = null;

            if (assignees.Count > 0)
            {
                if (assignees.Count == 1)
                {
                    result = assignees[0];
                }
                else
                {
                    WfClientAssignee firstAssignee = null;

                    foreach (WfClientAssignee assignee in assignees)
                    {
                        if (firstAssignee == null)
                            firstAssignee = assignee;

                        if (assignee.AssigneeType == WfClientAssigneeType.Normal)
                        {
                            result = assignee;
                            break;
                        }
                    }

                    if (result == null)
                        result = firstAssignee;
                }
            }

            return result;
        }

        public IEnumerable<WfClientUser> ToUsers()
        {
            List<WfClientUser> users = new List<WfClientUser>();

            foreach (WfClientAssignee assignee in this)
                if (assignee.User != null)
                    users.Add(assignee.User);

            return users;
        }

        /// <summary>
        /// 转换成WfUserResourceDescriptor的集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WfClientUserResourceDescriptor> ToResources()
        {
            List<WfClientUserResourceDescriptor> list = new List<WfClientUserResourceDescriptor>();

            foreach (WfClientAssignee assignee in this)
                list.Add(assignee.ToResource());

            return list;
        }
    }
}
