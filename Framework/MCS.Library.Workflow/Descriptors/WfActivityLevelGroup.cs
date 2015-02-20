using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.Workflow.Descriptors
{
	/// <summary>
	/// 将流程节点按照环节分组
	/// </summary>
	public class WfActivityLevelGroup : GroupNode<string, WfSimpleActivityDescriptorCollection>
	{
		private string description;

		public string Description
		{
			get { return this.description; }
			set { this.description = value; }
		}
	}
}
