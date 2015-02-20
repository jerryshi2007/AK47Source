using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	public class SampleTreeNode : TreeNodeBase<SampleTreeNode, SampleTreeNodeCollection>
	{
		public SampleTreeNode(string data)
		{
			this.Data = data;
		}

		public string Data { get; set; }
	}

	public class SampleTreeNodeCollection : TreeNodeBaseCollection<SampleTreeNode, SampleTreeNodeCollection>
	{
	}
}
