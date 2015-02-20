using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services.Description;
using System.Net;
using System.IO;
using System.Web.Services.Protocols;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace MCS.Library.SOA.Web.WebControls.Test.ServiceImport
{
	public partial class ImportServiceInfo : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void importButton_Click(object sender, EventArgs e)
		{
			//string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";

			string serviceUri = "ServiceImport/ImportedWebService.asmx";

			string url = Request.Url.GetComponents(
								UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) +
								(Request.ApplicationPath == "/" ?
									Request.ApplicationPath + serviceUri :
									Request.ApplicationPath + "/" + serviceUri);
			WebClient wc = new WebClient();
			using (Stream stream = wc.OpenRead(url + "?wsdl"))
			{
				ServiceDescription sd = ServiceDescription.Read(stream);

				//ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
				//sdi.AddServiceDescription(sd, "", "");
				//CodeNamespace cn = new CodeNamespace(@namespace);

				//string classname = "ImportedWebService";

				////生成客户端代理类代码
				//CodeCompileUnit ccu = new CodeCompileUnit();
				//ccu.Namespaces.Add(cn);
				//sdi.Import(cn, ccu);
				//CSharpCodeProvider csc = new CSharpCodeProvider();

				////设定编译参数
				//CompilerParameters cplist = new CompilerParameters();
				//cplist.GenerateExecutable = false;
				//cplist.GenerateInMemory = true;
				//cplist.ReferencedAssemblies.Add("System.dll");
				//cplist.ReferencedAssemblies.Add("System.XML.dll");
				//cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
				//cplist.ReferencedAssemblies.Add("System.Data.dll");

				////编译代理类
				//CompilerResults cr = csc.CompileAssemblyFromDom(cplist, ccu);
				//if (true == cr.Errors.HasErrors)
				//{
				//    System.Text.StringBuilder sb = new System.Text.StringBuilder();
				//    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
				//    {
				//        sb.Append(ce.ToString());
				//        sb.Append(System.Environment.NewLine);
				//    }
				//    throw new Exception(sb.ToString());
				//}

				////生成代理实例，并调用方法
				//System.Reflection.Assembly assembly = cr.CompiledAssembly;
				//Type t = assembly.GetType(@namespace + "." + classname, true, true);
				//object obj = Activator.CreateInstance(t);

				//System.Reflection.MethodInfo mi = t.GetMethod("GetAlternatedUser");

				//TestUser user = new TestUser();

				//user.Name = "Xulei";

				//object result = mi.Invoke(obj, new Object[] { user });
			}

			System.ServiceModel.Channels.Message message = System.ServiceModel.Channels.Message.CreateMessage(System.ServiceModel.Channels.MessageVersion.Soap11,
				"http://tempuri.org/HelloWorld");
			
		}

		private static string GetWsClassName(string wsUrl)
		{
			string[] parts = wsUrl.Split('/');
			string[] pps = parts[parts.Length - 1].Split('.');

			return pps[0];
		}

	}
}