using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.Test.CoreTest
{
	[TestClass]
	public class UriTest
	{
		private const string UriTestCategory = "UriTestCategory";

		[TestMethod]
		[Description("正常的绝对地址的uri等于测试")]
		[TestCategory(UriTestCategory)]
		public void NormalAbsoluteUriHostAndSchemeTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("http://www.BAIDU.com/test", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.CompareSchemeAndHost(uri2));
		}

		[TestMethod]
		[Description("Host绝对地址的uri不等于测试")]
		[TestCategory(UriTestCategory)]
		public void HostNotEqualAbsoluteTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("http://www.google.com/test", UriKind.RelativeOrAbsolute);

			Assert.IsFalse(uri1.CompareSchemeAndHost(uri2));
		}

		[TestMethod]
		[Description("Scheme绝对地址的uri不等于测试")]
		[TestCategory(UriTestCategory)]
		public void SchemeNotEqualAbsoluteTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("https://www.baidu.com/test", UriKind.RelativeOrAbsolute);

			Assert.IsFalse(uri1.CompareSchemeAndHost(uri2));
		}

		[TestMethod]
		[Description("默认端口的绝对地址的uri等于测试")]
		[TestCategory(UriTestCategory)]
		public void DefaultPortEqualAbsoluteTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("http://www.baidu.com:80/test", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.CompareSchemeAndHost(uri2));
		}

		[TestMethod]
		[Description("默认端口的绝对地址的uri不等于测试")]
		[TestCategory(UriTestCategory)]
		public void DefaultPortNotEqualAbsoluteTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("http://www.baidu.com:81/test", UriKind.RelativeOrAbsolute);

			Assert.IsFalse(uri1.CompareSchemeAndHost(uri2));
		}

		[TestMethod]
		[Description("空Uri的相等测试")]
		[TestCategory(UriTestCategory)]
		public void NullUriEqualTest()
		{
			Uri uri1 = null;
			Uri uri2 = null;

			Assert.IsTrue(uri1.CompareSchemeAndHost(uri2));
		}

		[TestMethod]
		[Description("一个Uri为空的相等测试")]
		[TestCategory(UriTestCategory)]
		public void OneUriIsNullNotEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = null;

			Assert.IsFalse(uri1.CompareSchemeAndHost(uri2));
		}

		[TestMethod]
		[Description("一个绝对，一个相对的Uri的不等测试")]
		[TestCategory(UriTestCategory)]
		public void AbsoluteAndRelativeNotEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("/test", UriKind.RelativeOrAbsolute);

			Assert.IsFalse(uri1.CompareSchemeAndHost(uri2));
		}

		[TestMethod]
		[Description("都是相对地址Uri的相等测试")]
		[TestCategory(UriTestCategory)]
		public void AllRelativeEqualHostAndSchemeTest()
		{
			Uri uri1 = new Uri("/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("/test1", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.CompareSchemeAndHost(uri2));
		}

		[TestMethod]
		[Description("正常路径的相等测试")]
		[TestCategory(UriTestCategory)]
		public void NormalPathEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.ComparePathAndParameters(uri2));
		}

		[TestMethod]
		[Description("正常路径的不相等测试")]
		[TestCategory(UriTestCategory)]
		public void NormalPathNotEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("http://www.baidu.com/test2", UriKind.RelativeOrAbsolute);

			Assert.IsFalse(uri1.ComparePathAndParameters(uri2));
		}

		[TestMethod]
		[Description("相对路径的相等测试")]
		[TestCategory(UriTestCategory)]
		public void RelativePathEqualTest()
		{
			Uri uri1 = new Uri("/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("/test", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.ComparePathAndParameters(uri2));
		}

		[TestMethod]
		[Description("相对路径的不相等测试")]
		[TestCategory(UriTestCategory)]
		public void RelativePathNotEqualTest()
		{
			Uri uri1 = new Uri("/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("/test1", UriKind.RelativeOrAbsolute);

			Assert.IsFalse(uri1.ComparePathAndParameters(uri2));
		}

		[TestMethod]
		[Description("一个是相对路径的相等测试")]
		[TestCategory(UriTestCategory)]
		public void OneIsRelativePathEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("/test", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.ComparePathAndParameters(uri2));
		}

		[TestMethod]
		[Description("一个是相对路径，且绝对路径没有Path的相等测试")]
		[TestCategory(UriTestCategory)]
		public void OneIsRelativePathAndAbsUriNoPathEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("/", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.ComparePathAndParameters(uri2));
		}

		[TestMethod]
		[Description("一个是相对路径，且绝对路径没的Path仅有斜线的相等测试")]
		[TestCategory(UriTestCategory)]
		public void OneIsRelativePathAndAbsUriPathIsSlashEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("/", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.ComparePathAndParameters(uri2));
		}

		[TestMethod]
		[Description("一个是相对路径的不相等测试")]
		[TestCategory(UriTestCategory)]
		public void OneIsRelativePathNotEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("/test1", UriKind.RelativeOrAbsolute);

			Assert.IsFalse(uri1.ComparePathAndParameters(uri2));
		}

		[TestMethod]
		[Description("正常路径和参数的相等测试")]
		[TestCategory(UriTestCategory)]
		public void NormalPathAndParametersEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test?name=S1", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("http://www.baidu.com/test?Name=S1", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.ComparePathAndParameters(uri2));
		}

		[TestMethod]
		[Description("正常路径和参数的不相等测试")]
		[TestCategory(UriTestCategory)]
		public void NormalPathAndParametersNotEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test?name=S1", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("http://www.baidu.com/test?Name=S1&gender=male", UriKind.RelativeOrAbsolute);

			Assert.IsFalse(uri1.ComparePathAndParameters(uri2));
		}

		[TestMethod]
		[Description("正常路径和参数的相等测试")]
		[TestCategory(UriTestCategory)]
		public void NormalPathWithIgnoreParametersEqualTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test?name=S1&gender=male", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("http://www.baidu.com/test?Name=S1", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.ComparePathAndParameters(uri2, "Gender"));
		}

		[TestMethod]
		[Description("AreSame的相等测试")]
		[TestCategory(UriTestCategory)]
		public void NormalAreSameUriTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test?name=S1", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("http://www.baidu.com/test?Name=S1", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.AreSameUri(uri2));
		}

		[TestMethod]
		[Description("都是null的AreSameUri测试")]
		[TestCategory(UriTestCategory)]
		public void NullUriAreSameUriTest()
		{
			Uri uri1 = null;
			Uri uri2 = null;

			Assert.IsTrue(uri1.AreSameUri(uri2));
		}

		[TestMethod]
		[Description("有一个是null的AreSameUri测试")]
		[TestCategory(UriTestCategory)]
		public void OneIsNullUriAreSameUriTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test?name=S1", UriKind.RelativeOrAbsolute);
			Uri uri2 = null;

			Assert.IsFalse(uri1.AreSameUri(uri2));
		}

		[TestMethod]
		[Description("有一个是绝对路径的AreSameUri测试")]
		[TestCategory(UriTestCategory)]
		public void OneIsAbsoluteUriAreSameUriTest()
		{
			Uri uri1 = new Uri("http://www.baidu.com/test?name=S1", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("/test?name=S1", UriKind.RelativeOrAbsolute);

			Assert.IsFalse(uri1.AreSameUri(uri2));
		}

		[TestMethod]
		[Description("都是是相对路径的AreSameUri测试")]
		[TestCategory(UriTestCategory)]
		public void RelativeUriAreSameUriTest()
		{
			Uri uri1 = new Uri("/test?name=S1", UriKind.RelativeOrAbsolute);
			Uri uri2 = new Uri("/test?name=S1", UriKind.RelativeOrAbsolute);

			Assert.IsTrue(uri1.AreSameUri(uri2));
		}

		[TestMethod]
		[Description("获取为空的Uri的目录地址")]
		[TestCategory(UriTestCategory)]
		public void GetNullUriDirectoryTest()
		{
			Uri uri = null;

			Assert.AreEqual(string.Empty, uri.GetDiractoryUri().ToString());
		}

		[TestMethod]
		[Description("一般绝对路径的Uri的目录地址")]
		[TestCategory(UriTestCategory)]
		public void GetNormalAbsoluteUriDirectoryTest()
		{
			Uri uri = new Uri("http://www.baidu.com/test?name=S1", UriKind.RelativeOrAbsolute); ;

			Assert.AreEqual("http://www.baidu.com/", uri.GetDiractoryUri().ToString());
		}

		[TestMethod]
		[Description("一般绝对路径的Uri的目录，以反斜线结尾的地址")]
		[TestCategory(UriTestCategory)]
		public void GetNormalAbsoluteUriWithEndSlashDirectoryTest()
		{
			Uri uri = new Uri("http://www.baidu.com/test/", UriKind.RelativeOrAbsolute); ;

			Assert.AreEqual("http://www.baidu.com/test/", uri.GetDiractoryUri().ToString());
		}

		[TestMethod]
		[Description("一般相对路径的Uri的目录地址")]
		[TestCategory(UriTestCategory)]
		public void GetRelativeUriDirectoryTest()
		{
			Uri uri = new Uri("/test?name=S1", UriKind.RelativeOrAbsolute); ;

			Assert.AreEqual("/", uri.GetDiractoryUri().ToString());
		}

		[TestMethod]
		[Description("没有目录的绝对路径的Uri的目录地址")]
		[TestCategory(UriTestCategory)]
		public void GetAbsoluteWithoutSlashUriDirectoryTest()
		{
			//如果此路径初始化到Uri类，则会自动在后面添加一个反斜线
			string uri = "http://www.baidu.com";

			Assert.AreEqual("http://", UriHelper.GetDiractoryUri(uri));
		}

		[TestMethod]
		[Description("将一个绝对路径转换成另一个绝对路径")]
		[TestCategory(UriTestCategory)]
		public void MakeAbsoluteUriToAnthorAbsolutePathTest()
		{
			Uri target = new Uri("http://www.baidu.com/test.aspx");
			Uri refUri = new Uri("http://www.sina.com.cn/foo/bar.aspx");

			Assert.AreEqual("http://www.sina.com.cn/test.aspx", target.MakeAbsolute(refUri).ToString());
		}

		[TestMethod]
		[Description("将一个根路径的相对路径转换成另一个绝对路径")]
		[TestCategory(UriTestCategory)]
		public void MakeRootRelativeUriToToAbsolutePathTest()
		{
			Uri target = new Uri("/test.aspx", UriKind.RelativeOrAbsolute);
			Uri refUri = new Uri("http://www.sina.com.cn/foo/bar.aspx");

			Assert.AreEqual("http://www.sina.com.cn/test.aspx", target.MakeAbsolute(refUri).ToString());
		}

		[TestMethod]
		[Description("将一个相对路径转换成另一个绝对路径")]
		[TestCategory(UriTestCategory)]
		public void MakeRelativeUriToToAbsolutePathTest()
		{
			Uri target = new Uri("test.aspx", UriKind.RelativeOrAbsolute);
			Uri refUri = new Uri("http://www.sina.com.cn/foo/bar.aspx");

			Assert.AreEqual("http://www.sina.com.cn/foo/test.aspx", target.MakeAbsolute(refUri).ToString());
		}

		[TestMethod]
		[Description("将一个带退回上级目录相对路径转换成另一个绝对路径")]
		[TestCategory(UriTestCategory)]
		public void MakeRelativeUriWithBackDirToToAbsolutePathTest()
		{
			Uri target = new Uri("../test.aspx", UriKind.RelativeOrAbsolute);
			Uri refUri = new Uri("http://www.sina.com.cn/foo/bar.aspx");

			Assert.AreEqual("http://www.sina.com.cn/test.aspx", target.MakeAbsolute(refUri).ToString());
		}

		[TestMethod]
		[Description("将一个相对路径转换成另一个相对路径")]
		[TestCategory(UriTestCategory)]
		public void MakeRelativeUriToToRelativePathTest()
		{
			Uri target = new Uri("test.aspx", UriKind.RelativeOrAbsolute);
			Uri refUri = new Uri("/foo/bar.aspx", UriKind.RelativeOrAbsolute);

			Assert.AreEqual("/foo/test.aspx", target.MakeAbsolute(refUri).ToString());
		}

		[TestMethod]
		[Description("将一个根路径的相对路径转换成另一个相对路径")]
		[TestCategory(UriTestCategory)]
		public void MakeRootRelativeUriToToRelativePathTest()
		{
			Uri target = new Uri("/test.aspx", UriKind.RelativeOrAbsolute);
			Uri refUri = new Uri("/foo/bar.aspx", UriKind.RelativeOrAbsolute);

			Assert.AreEqual("/test.aspx", target.MakeAbsolute(refUri).ToString());
		}

		[TestMethod]
		[Description("得到一个绝对路径的所有目录")]
		[TestCategory(UriTestCategory)]
		public void GetAbsoluteUriDirectoriesTest()
		{
			Uri target = new Uri("http://www.baidu.com/test/index.aspx", UriKind.RelativeOrAbsolute);

			string[] dirs = target.GetAllDirectories();

			Assert.AreEqual(1, dirs.Length);
			Assert.AreEqual("test", dirs[0]);
		}

		[TestMethod]
		[Description("得到一个以斜线为结尾的绝对路径的所有目录")]
		[TestCategory(UriTestCategory)]
		public void GetAbsoluteUriWithEndSlashDirectoriesTest()
		{
			Uri target = new Uri("http://www.baidu.com/test/index/", UriKind.RelativeOrAbsolute);

			string[] dirs = target.GetAllDirectories();

			Assert.AreEqual(2, dirs.Length);
			Assert.AreEqual("test", dirs[0]);
			Assert.AreEqual("index", dirs[1]);
		}

		[TestMethod]
		[Description("得到一个相对路径的所有目录")]
		[TestCategory(UriTestCategory)]
		public void GetRelativeUriDirectoriesTest()
		{
			Uri target = new Uri("/test/index.aspx", UriKind.RelativeOrAbsolute);

			string[] dirs = target.GetAllDirectories();

			Assert.AreEqual(1, dirs.Length);
			Assert.AreEqual("test", dirs[0]);
		}

		[TestMethod]
		[Description("得到一个以斜线为结尾的相对路径的所有目录")]
		[TestCategory(UriTestCategory)]
		public void GetRelativeUriWithEndSlashDirectoriesTest()
		{
			Uri target = new Uri("/test/index/", UriKind.RelativeOrAbsolute);

			string[] dirs = target.GetAllDirectories();

			Assert.AreEqual(2, dirs.Length);
			Assert.AreEqual("test", dirs[0]);
			Assert.AreEqual("index", dirs[1]);
		}

		[TestMethod]
		[Description("得到一个相对路径的所有目录")]
		[TestCategory(UriTestCategory)]
		public void GetRelativeUriWithMultiSlashDirectoriesTest()
		{
			Uri target = new Uri("/test///index.aspx", UriKind.RelativeOrAbsolute);

			string[] dirs = target.GetAllDirectories();

			Assert.AreEqual(1, dirs.Length);
			Assert.AreEqual("test", dirs[0]);
		}

		[TestMethod]
		[Description("合并绝对路径的Uri")]
		[TestCategory(UriTestCategory)]
		public void MergeAbsoluteUriTest()
		{
			Uri baseUri = new Uri("http://www.baidu.com/test/", UriKind.RelativeOrAbsolute);
			Uri relUri = new Uri("index/a.htm", UriKind.RelativeOrAbsolute);

			Uri mergedUri = baseUri.MergePath(relUri);

			Console.WriteLine(mergedUri.ToString());

			Assert.AreEqual("http://www.baidu.com/test/index/a.htm", mergedUri.ToString());
		}

		[TestMethod]
		[Description("合并绝对路径带..的Uri")]
		[TestCategory(UriTestCategory)]
		public void MergeAbsoluteUriWithBackRelPathTest()
		{
			Uri baseUri = new Uri("http://www.baidu.com/test/", UriKind.RelativeOrAbsolute);
			Uri relUri = new Uri("../index/a.htm", UriKind.RelativeOrAbsolute);

			Uri mergedUri = baseUri.MergePath(relUri);

			Console.WriteLine(mergedUri.ToString());

			Assert.AreEqual("http://www.baidu.com/index/a.htm", mergedUri.ToString());
		}

		[TestMethod]
		[Description("合并绝对路径带.的Uri")]
		[TestCategory(UriTestCategory)]
		public void MergeAbsoluteUriWithForwardRelPathTest()
		{
			Uri baseUri = new Uri("http://www.baidu.com/test/", UriKind.RelativeOrAbsolute);
			Uri relUri = new Uri("./index/a.htm", UriKind.RelativeOrAbsolute);

			Uri mergedUri = baseUri.MergePath(relUri);

			Console.WriteLine(mergedUri.ToString());

			Assert.AreEqual("http://www.baidu.com/test/index/a.htm", mergedUri.ToString());
		}

		[TestMethod]
		[Description("单参数字典转换为Url参数")]
		[TestCategory(UriTestCategory)]
		public void SingleNameValueCollectionToUrlParamsTest()
		{
			NameValueCollection parameters = new NameValueCollection();

			parameters["name"] = "沈峥";

			Assert.AreEqual("name=%e6%b2%88%e5%b3%a5", parameters.ToUrlParameters(true));
		}

		[TestMethod]
		[Description("双参数字典转换为Url参数")]
		[TestCategory(UriTestCategory)]
		public void DoubleNameValueCollectionToUrlParamsTest()
		{
			NameValueCollection parameters = new NameValueCollection();

			parameters["name"] = "沈峥";
			parameters["gender"] = "male";

			Assert.AreEqual("name=%e6%b2%88%e5%b3%a5&gender=male", parameters.ToUrlParameters(true));
		}

		[TestMethod]
		[Description("字典转换为Url参数，其中某个参数的key为空，另一个值为空")]
		[TestCategory(UriTestCategory)]
		public void SpecialNameValueCollectionToUrlParamsTest()
		{
			NameValueCollection parameters = new NameValueCollection();

			parameters[""] = "沈峥";
			parameters["gender"] = "";

			Console.WriteLine(parameters.ToUrlParameters(true));
			Assert.AreEqual("gender=", parameters.ToUrlParameters(true));
		}
	}
}
