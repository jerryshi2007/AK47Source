using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MCS.Library.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.Test
{
	[TestClass]
	public class StringTest
	{
		[TestMethod]
		public void MatchWithAsteriskTest()
		{
			Assert.IsTrue(MatchWithAsterisk("test.aspx", "*.aspx"), "test.aspx *.aspx");
			Assert.IsTrue(MatchWithAsterisk("test.aspx", "test.*"), "test.aspx test.*");
			Assert.IsTrue(MatchWithAsterisk("test.aspx", "test.aspx"), "test.aspx test.aspx");
			Assert.IsFalse(MatchWithAsterisk("test.aspx", "test.aspx1"), "test.aspx test.aspx1");
			Assert.IsFalse(MatchWithAsterisk("test.aspx", "*.aspx1"), "test.aspx *.aspx1");
			Assert.IsFalse(MatchWithAsterisk("test.aspx", "test1.*"), "test.aspx test1.*");

			Assert.IsTrue(MatchWithAsterisk("test.aaspx", "test.*aspx"), "test.aaspx test.*aspx");
			Assert.IsFalse(MatchWithAsterisk("test.aaspx", "test.*1aspx"), "test.aaspx test.*1aspx");

			Assert.IsTrue(MatchWithAsterisk("Oh yeah. Totay is weekend!", "*ye*a*e*"), "Oh year.Totay is weekend! *ye*a*e*");
			Assert.IsTrue(MatchWithAsterisk("test.aaspx.bascx", "test.a*.b*"), "test.aaspx.bascx test.a*.b*");

			Assert.IsTrue(MatchWithAsterisk("woo?processKey=KK", "woo?processKey=*"), "woo?processKey=KK woo?processKey=*");
			Assert.IsFalse(MatchWithAsterisk("woo", "woo?processKey=*"), "woo woo?processKey=*");
			Assert.IsFalse(MatchWithAsterisk("woo?process=KK", "woo?processKey=*"), "woo?process=KK woo?processKey=*");

			Assert.IsTrue(MatchWithAsterisk("EmployeeEntryFor/EmployeeEntryForController.ashx?processDescKey=EmployeeEntry&userName=SZ&userOrg=0190001&userHRMgmtUnit=101",
				"EmployeeEntryFor/EmployeeEntryForController.ashx?processDescKey=EmployeeEntry&userName=*&userOrg=*&userHRMgmtUnit=*"));
			Assert.IsFalse(MatchWithAsterisk("EmployeeEntryFor/EmployeeEntryForController.ashx?processDescKey=EmployeeEntry&userName=SZ&userOrg=0190001",
				"EmployeeEntryFor/EmployeeEntryForController.ashx?processDescKey=EmployeeEntry&userName=*&userOrg=*&userHRMgmtUnit=*"));
		}

		private static bool MatchWithAsterisk(string data, string pattern)
		{
			if (data.IsNullOrEmpty() || pattern.IsNullOrEmpty())
				return false;

			string[] ps = pattern.Split('*');

			if (ps.Length == 1) // 没有*的模型
				return MatchWithInterrogation(data, ps[0]);

			var si = data.IndexOf(ps[0], 0);	// 从string头查找第一个串

			if (si != 0)
				return false; // 第一个串没找到或者不在string的头部

			si += ps[0].Length; // 找到了串后,按就近原则,移到未查询过的最左边

			int plast = ps.Length - 1; // 最后一串应单独处理,为了提高效率,将它从循环中取出
			int pi = 0; // 跳过之前处理过的第一串

			while (++pi < plast)
			{
				if (ps[pi] == "")
					continue; //连续的*号,可以忽略

				si = data.IndexOf(ps[pi], si);	// 继续下一串的查找

				if (-1 == si)
					return false; // 没有找到

				si += ps[pi].Length; // 就近原则
			}

			if (ps[plast] == "") // 模型尾部为*,说明所有有效字符串部分已全部匹配,string后面可以是任意字符
				return true;

			// 从尾部查询最后一串是否存在
			int last_index = data.LastIndexOf(ps[plast]);

			// 如果串存在,一定要在string的尾部, 并且不能越过已查询过部分
			return (last_index == data.Length - ps[plast].Length) && (last_index >= si);
		}

		private static bool MatchWithInterrogation(string data, string pattern)
		{
			bool result = false;

			if (data.Length == pattern.Length)
				result = data.IndexOf(pattern) > -1;

			return result;
		}
	}
}
