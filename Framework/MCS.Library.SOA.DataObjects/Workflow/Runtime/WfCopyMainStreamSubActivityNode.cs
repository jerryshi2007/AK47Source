using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	internal class WfCopyMainStreamSubActivityNode
	{
		private WfCopyMainStreamActivityNode _ActivityNode = null;
		private IWfTransitionDescriptor _FromTransition = null;

		public WfCopyMainStreamSubActivityNode(IWfTransitionDescriptor fromTransition, WfCopyMainStreamActivityNode activityNode)
		{
			this._FromTransition = fromTransition;
			this._ActivityNode = activityNode;
		}

		public IWfTransitionDescriptor FromTransition
		{
			get
			{
				return this._FromTransition;
			}
		}

		public WfCopyMainStreamActivityNode ActivityNode
		{
			get
			{
				return this._ActivityNode;
			}
		}

		internal string ActivityKey
		{
			get
			{
				return this._ActivityNode != null ? this._ActivityNode.ActivityKey : string.Empty;
			}
		}
	}

	internal class WfCopyMainStreamSubActivityNodeCollection : EditableDataObjectCollectionBase<WfCopyMainStreamSubActivityNode>
	{
	}
}
