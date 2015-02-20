using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 递归加载对象关系的委托
	/// </summary>
	/// <param name="tempRelations"></param>
	/// <param name="context"></param>
	public delegate void LoadingRelationsRecursivelyHandler(SCSimpleRelationObjectCollection tempRelations, object context);
}
