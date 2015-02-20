using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 每一个分支流程的启动参数
	/// </summary>
	[Serializable]
	[XElementSerializable]
	[XElementFieldSerialize(IgnoreDeserializeError = true)]
	public class WfBranchProcessStartupParams
	{
		private WfAssigneeCollection _Assignees = null;
		private IOrganization _Department = null;
		private Dictionary<string, object> _ApplicationRuntimeParameters = null;

		private object _StartupContext = null;

		public WfBranchProcessStartupParams()
		{
		}

		public WfBranchProcessStartupParams(params IUser[] users)
		{
			this.Assignees.Add(users);
		}

		public WfBranchProcessStartupParams(IEnumerable<IUser> users)
		{
			this.Assignees.Add(users);
		}

		public Dictionary<string, object> ApplicationRuntimeParameters
		{
			get
			{
				if (this._ApplicationRuntimeParameters == null)
					this._ApplicationRuntimeParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

				return this._ApplicationRuntimeParameters;
			}
			internal set
			{
				this._ApplicationRuntimeParameters = value;
			}
		}

		public string ResourceID
		{
			get;
			set;
		}

		public string DefaultTaskTitle
		{
			get;
			set;
		}

		private NameValueCollection _RelativeParams = null;

		public NameValueCollection RelativeParams
		{
			get
			{
				if (this._RelativeParams == null)
					this._RelativeParams = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

				return this._RelativeParams;
			}
			set
			{
				this._RelativeParams = value;
			}
		}

		public object StartupContext
		{
			get { return this._StartupContext; }
			set { this._StartupContext = value; }
		}

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
				Assignees.Add(users);
			}
		}

		/// <summary>
		/// 分支流程的部门
		/// </summary>
		public IOrganization Department
		{
			get { return this._Department; }
			set { this._Department = (IOrganization)OguBase.CreateWrapperObject(value); }
		}

		/// <summary>
		/// 分支流程的起始点的办理人
		/// </summary>
		public WfAssigneeCollection Assignees
		{
			get
			{
				if (this._Assignees == null)
					this._Assignees = new WfAssigneeCollection();

				return this._Assignees;
			}
			set
			{
				this._Assignees = value;
			}
		}
	}

	[Serializable]
	public class WfBranchProcessStartupParamsCollection : EditableDataObjectCollectionBase<WfBranchProcessStartupParams>
	{
		/// <summary>
		/// 根据一组用户创建分支流程启动参数
		/// </summary>
		/// <param name="users"></param>
		public void Add(params IUser[] users)
		{
			Add((IEnumerable<IUser>)users);
		}

		/// <summary>
		/// 根据一组用户创建分支流程启动参数
		/// </summary>
		/// <param name="users"></param>
		public void Add(IEnumerable<IUser> users)
		{
			if (users != null)
			{
				foreach (IUser user in users)
				{
					this.Add(new WfBranchProcessStartupParams(user));
				}
			}
		}
	}
}
