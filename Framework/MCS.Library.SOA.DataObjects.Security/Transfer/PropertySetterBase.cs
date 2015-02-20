using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	/// <summary>
	/// 属性值设置器的基类
	/// </summary>
	public abstract class PropertySetterBase
	{
		/// <summary>
		/// 设置属性的值
		/// </summary>
		/// <param name="srcObject"></param>
		/// <param name="srcPropertyName"></param>
		/// <param name="targetObject"></param>
		/// <param name="targetObjectName"></param>
		/// <param name="targetPropertyName"></param>
		/// <param name="context"></param>
		/// <param name="setterContext"></param>
		public abstract void SetValue(object srcObject, string srcPropertyName, object targetObject, string targetPropertyName, string context, SetterContext setterContext);

		/// <summary>
		/// 当对象的属性值全部设置完后，且已经提交后的后续处理
		/// </summary>
		/// <param name="srcObject"></param>
		/// <param name="srcPropertyName"></param>
		/// <param name="targetObject"></param>
		/// <param name="targetPropertyName"></param>
		/// <param name="context"></param>
		/// <param name="setterContext"></param>
		public abstract void AfterObjectUpdated(object srcObject, string srcPropertyName, object targetObject, string targetPropertyName, string context, SetterContext setterContext);

		public void TraceIt(string srcObjString, string srcProperty, string targetObjString, string targetPropertyName, string context, string srcValue, string targetValue)
		{
			Trace.WriteLine(string.Format("源对象:{0}, 源属性:{1},源属性值:{2}, 目的对象:{3}，目的属性:{4},上下文:{5},目的属性值:{6}", srcObjString, srcProperty, srcValue ?? "<null>", targetObjString, targetPropertyName, context, targetValue ?? "<null>"), this.GetType().Name);
		}
	}
}
