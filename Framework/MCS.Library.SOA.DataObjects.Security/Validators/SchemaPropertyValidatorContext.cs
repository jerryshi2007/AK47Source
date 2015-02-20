using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Validators
{
	/// <summary>
	/// Schema对象属性验证器的容器对象
	/// </summary>
	[ActionContextDescription(Key = "SchemaPropertyValidatorContext")]
	public class SchemaPropertyValidatorContext : ActionContextBase<SchemaPropertyValidatorContext>
	{
		public SchemaPropertyValidatorContext()
		{
		}

		/// <summary>
		/// Schema对象的属性验证器所涉及到的对象
		/// </summary>
		public SchemaObjectBase Target
		{
			get;
			internal set;
		}

		/// <summary>
		/// Schema对象的属性验证器所涉及到的容器对象
		/// </summary>
		public SchemaObjectBase Container
		{
			get;
			internal set;
		}
	}
}
