using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;

namespace MCS.Library.WcfExtensions
{
	public class WfJsonWebHttpBehaviorElement : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get
			{
				return typeof(WfJsonWebHttpBehavior);
			}
		}

		protected override object CreateBehavior()
		{
			return new WfJsonWebHttpBehavior();
		}
	}
}
