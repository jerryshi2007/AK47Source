using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;
using System.Xml.Linq;


namespace MCS.Library.SOA.DataObjects.Workflow
{
	public enum WfAssigneeType
	{
		Normal,
		Delegated
	}

	[Serializable]
	[XElementSerializable]
	public class WfAssignee : ISimpleXmlSerializer
	{
		private IUser _User = null;
		private IUser _Delegator = null;
		private string _Url = null;
		private bool _Selected = true;

		public WfAssigneeType AssigneeType { get; set; }

		public WfAssignee()
		{
		}

		public WfAssignee(IUser user)
		{
			this._User = (IUser)OguUser.CreateWrapperObject(user);
		}

		public IUser User
		{
			get
			{
				return this._User;
			}
			set
			{
				this._User = (IUser)OguUser.CreateWrapperObject(value);
			}
		}

		public IUser Delegator
		{
			get
			{
				return this._Delegator;
			}
			set
			{
				this._Delegator = (IUser)OguUser.CreateWrapperObject(value);
			}
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
			get
			{
				return this._Url;
			}
			set
			{
				this._Url = value;
			}
		}

		/// <summary>
		/// 转换为Resource对象
		/// </summary>
		/// <returns></returns>
		public WfUserResourceDescriptor ToResource()
		{
			return new WfUserResourceDescriptor(this.User);
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			this.User.ToSimpleXElement(element);

			this.Delegator.ToSimpleXElement(element, "Delegator");

			element.SetAttributeValue("AssigneeType", this.AssigneeType.ToString());

			if (this.Url.IsNotEmpty())
				element.SetAttributeValue("Url", this.Url);
		}

		#endregion
	}

	[Serializable]
	[XElementSerializable]
	public class WfAssigneeCollection : EditableDataObjectCollectionBase<WfAssignee>, ISimpleXmlSerializer
	{
		/// <summary>
		/// 添加一组用户
		/// </summary>
		/// <param name="users"></param>
		public void Add(params IUser[] users)
		{
			Add((IEnumerable<IUser>)users);
		}

		/// <summary>
		/// 增加一组用户
		/// </summary>
		/// <param name="users"></param>
		public void Add(IEnumerable<IUser> users)
		{
			if (users != null)
			{
				foreach (IUser user in users)
				{
					this.Add(new WfAssignee(user));
				}
			}
		}

		public bool Contains(IUser target)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(target != null, "target");

			bool found = false;

			foreach (WfAssignee assignee in this)
			{
				if (string.Compare(assignee.User.ID, target.ID, true) == 0 &&
					string.Compare(assignee.User.FullPath, target.FullPath, true) == 0)
				{
					found = true;
					break;
				}
			}

			return found;
		}

		public bool Contains(string userID)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(userID, "userID");

			return Find(assignee => string.Compare(assignee.User.ID, userID, true) == 0) != null;
		}

		/// <summary>
		/// 查找某个用户的委托人，如果没有找到，则返回自己
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public IUser FindDelegator(IUser user)
		{
			IUser result = user;

			foreach (WfAssignee assignee in this)
			{
				if (string.Compare(user.ID, assignee.User.ID, true) == 0)
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
		public WfAssignee FindFirstUser(string userID)
		{
			userID.CheckStringIsNullOrEmpty("userID");

			IList<WfAssignee> assignees = FindAll(a => string.Compare(a.User.ID, userID, true) == 0);

			WfAssignee result = null;

			if (assignees.Count > 0)
			{
				if (assignees.Count == 1)
				{
					result = assignees[0];
				}
				else
				{
					WfAssignee firstAssignee = null;

					foreach (WfAssignee assignee in assignees)
					{
						if (firstAssignee == null)
							firstAssignee = assignee;

						if (assignee.AssigneeType == WfAssigneeType.Normal)
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

		public WfAclItemCollection ToAcl(string resourceID, string source)
		{
			WfAclItemCollection result = new WfAclItemCollection();

			foreach (WfAssignee assignee in this)
			{
				WfAclItem item = new WfAclItem();

				item.ObjectID = assignee.User.ID;
				item.ObjectName = assignee.User.DisplayName;
				item.ObjectType = SchemaType.Users.ToString();
				item.ResourceID = resourceID;
				item.Source = source;

				result.Add(item);
			}

			return result;
		}

		public IEnumerable<IUser> ToUsers()
		{
			List<IUser> users = new List<IUser>();

			foreach (WfAssignee assignee in this)
				users.Add(assignee.User);

			return users;
		}

		/// <summary>
		/// 转换成WfUserResourceDescriptor的集合
		/// </summary>
		/// <returns></returns>
		public IEnumerable<WfUserResourceDescriptor> ToResources()
		{
			List<WfUserResourceDescriptor> list = new List<WfUserResourceDescriptor>();

			foreach (WfAssignee assignee in this)
				list.Add(assignee.ToResource());

			return list;
		}

		/// <summary>
		/// 过滤出被选择的对象
		/// </summary>
		/// <param name="multiResult">是否允许返回多个结果</param>
		/// <returns></returns>
		public WfAssigneeCollection GetSelectedAssignees()
		{
			WfAssigneeCollection result = new WfAssigneeCollection();

			foreach (WfAssignee assignee in this)
			{
				if (assignee.Selected)
					result.Add(assignee);
			}

			return result;
		}

		public bool AreSameAssignees(WfAssigneeCollection targetAssignees)
		{
			bool result = false;

			if (targetAssignees != null)
			{
				if (this.Count == targetAssignees.Count)
				{
					result = true;

					foreach (WfAssignee targetAssignee in targetAssignees)
					{
						if (this.Contains(targetAssignee.User.ID) == false)
						{
							result = false;
							break;
						}
					}
				}
			}

			return result;
		}

		public void ToSimpleXElement(XElement element, string childNodeName)
		{
			if (this.Count > 0)
			{
				XElement childNode = element.AddChildElement(childNodeName);

				((ISimpleXmlSerializer)this).ToXElement(childNode, string.Empty);
			}
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			foreach (WfAssignee assignee in this)
			{
				XElement childElem = element.AddChildElement("User");

				((ISimpleXmlSerializer)assignee).ToXElement(childElem, refNodeName);
			}
		}

		#endregion
	}
}
