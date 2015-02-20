using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;

//意见列表控件相关的事件参数类

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 渲染流转环节（可编辑）的事件
	/// </summary>
	public class RenderOneActivityEventArgs : EventArgs
	{
		private ActivityEditMode editMode = ActivityEditMode.None;
		private IWfActivity currentActivity = null;
		private IWfActivityDescriptor activityDescriptor = null;

		internal RenderOneActivityEventArgs()
		{
		}

		public IWfActivityDescriptor ActivityDescriptor
		{
			get { return this.activityDescriptor; }
			internal set { this.activityDescriptor = value; }
		}

		public IWfActivity CurrentActivity
		{
			get { return this.currentActivity; }
			internal set { this.currentActivity = value; }
		}

		public ActivityEditMode EditMode
		{
			get { return this.editMode; }
			set { this.editMode = value; }
		}
	}

	public class OpinionListViewBindEventArgs : EventArgs
	{
		private GenericOpinion _opinion = null;
		private IWfActivityDescriptor _wfActDesc = null;
		private Control _container = null;
		private bool _readOnly = true;

		internal OpinionListViewBindEventArgs(GenericOpinion currentOpinion, IWfActivityDescriptor currentWfActDesc, Control container, bool readOnly)
		{
			_opinion = currentOpinion;
			_wfActDesc = currentWfActDesc;
			_container = container;
			_readOnly = readOnly;
		}

		public GenericOpinion CurrentOpinion
		{
			get
			{
				return _opinion;
			}
		}

		public IWfActivityDescriptor CurrentActDesc
		{
			get
			{
				return _wfActDesc;
			}
		}

		public Control Container
		{
			get
			{
				return _container;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
		}
	}
}
