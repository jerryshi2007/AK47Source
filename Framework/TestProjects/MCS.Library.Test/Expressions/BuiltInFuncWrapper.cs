using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Expression;

namespace MCS.Library.Test
{
	public enum GenderType
	{
		Male = 1,
		Female = 2
	}

	public class BuiltInFuncWrapper
	{
        public static readonly BuiltInFuncWrapper Instance = new BuiltInFuncWrapper();

	    private BuiltInFuncWrapper()
	    {
	    }

		[BuiltInFunction]
		public int Add(int x, int y)
		{
			return x + y;
		}

		[BuiltInFunction]
		private int Sub(int x, int y)
		{
			return x - y;
		}

		[BuiltInFunction]
		private string Combine(string s1, object callerContext)
		{
			return string.Concat(s1, callerContext);
		}

		[BuiltInFunction]
		private string TypeName()
		{
			return this.GetType().FullName;
		}

		[BuiltInFunction]
		private static int StaticAdd(int a, int b)
		{
			return a + b;
		}

		[BuiltInFunction]
		private static int GenderValue(GenderType gender)
		{
			return (int)gender;
		}

		[BuiltInFunction]
		private static string GenderName(int genderValue)
		{
			return ((GenderType)DataConverter.ChangeType(genderValue, typeof(GenderType))).ToString();
		}
	}
}
