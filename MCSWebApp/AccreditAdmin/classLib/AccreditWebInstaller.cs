using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace MCS.Applications.AccreditAdmin.classLib
{
	[RunInstaller(true)]
	public partial class AccreditWebInstaller : Installer
	{
		/// <summary>
		/// 
		/// </summary>
		public AccreditWebInstaller()
			: base()
		{
			InitializeComponent();
			this.BeforeInstall += new InstallEventHandler(AccreditWebInstaller_BeforeInstall);
			this.Committed += new InstallEventHandler(AccreditWebInstaller_Committed);
			this.AfterUninstall += new InstallEventHandler(AccreditWebInstaller_AfterUninstall);
		}

		void AccreditWebInstaller_AfterUninstall(object sender, InstallEventArgs e)
		{
			try
			{
				string appPath = Path.GetDirectoryName(Path.GetDirectoryName(this.Context.Parameters["assemblypath"]));

				if (true == Directory.Exists(appPath))
				{
					Directory.Delete(appPath, true);
				}
			}
			catch
			{ }
		}

		void AccreditWebInstaller_BeforeInstall(object sender, InstallEventArgs e)
		{
			//可以在这里创建虚拟目录
		}

		void AccreditWebInstaller_Committed(object sender, InstallEventArgs e)
		{
			string accreditConn = this.Context.Parameters["ACCREDITCONNSTR"];
			string hgLogConn = this.Context.Parameters["HGLOGCONNSTR"];
			string passportHostname = this.Context.Parameters["PASSPORTSERVERNAME"];
			string passportHostPort = this.Context.Parameters["PASSPORTSERVERPORT"];

			string binPath = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);
			string appPath = Path.GetDirectoryName(binPath);
			int lastDiv = appPath.LastIndexOf("\\");
			//string virtualDir = appPath.Substring(lastDiv + 1);
			string webConfig = appPath + "/web.config";

			if (false == File.Exists(webConfig))
				throw new FileNotFoundException(webConfig);

			//FileInfo fi = new FileInfo(webConfig);
			//if ((fi.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
			//{
			//    fi.Attributes = (fi.Attributes & ~FileAttributes.ReadOnly);
			//}
			//if ((fi.Attributes & FileAttributes.System) != FileAttributes.System)
			//{
			//    fi.Attributes = (fi.Attributes & ~FileAttributes.System);
			//}
			XmlDocument configDoc = new XmlDocument();
			using (FileStream stream = new FileStream(webConfig, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				configDoc.Load(stream);
			}
			XmlElement root = configDoc.DocumentElement;
#if DEBUG
			foreach (string key in this.Context.Parameters.Keys)
			{
				root.SetAttribute(key, this.Context.Parameters[key]);
			}
#endif
			XmlNode node;
			#region Db Conn
			string rootPath = "./chinaCustoms.framework.deluxeWorks.data/connectionManager/connectionStrings";
			if (false == string.IsNullOrEmpty(accreditConn))
			{
				node = root.SelectSingleNode(rootPath + "/add[@name=\"AccreditAdmin\"]");
				if (null != node)
				{
					node.Attributes["connectionString"].InnerText = accreditConn;
				}
			}
			if (false == string.IsNullOrEmpty(hgLogConn))
			{
				node = root.SelectSingleNode(rootPath + "/add[@name=\"LOG\"]");
				if (null != node)
				{
					node.Attributes["connectionString"].InnerText = hgLogConn;
				}
			}
			node = root.SelectSingleNode(rootPath + "/add[@name=\"DeluxeWorksPassport\"]");
			if (null != node)
			{
				XmlNode parent = node.ParentNode;
				parent.RemoveChild(node);
			}
			#endregion

			#region Service
			rootPath = "./oguPermissionSettings/paths";
			node = root.SelectSingleNode(rootPath + "/add[@name=\"oguServiceAddress\"]");
			string localHost = GetLocalhost();
			if (null != node)
			{

				node.Attributes["uri"].InnerText = "http://" + localHost + "/Accreditadmin/services/OGUReaderService.asmx";
			}
			node = root.SelectSingleNode(rootPath + "/add[@name=\"appAdminServiceAddress\"]");
			if (null != node)
			{
				node.Attributes["uri"].InnerText = "http://" + localHost + "/Appadmin/exports/AppSecurityCheckService.asmx";
			}
			#endregion

			#region Section
			rootPath = "./configSections/section[@name=\"passportSignInSettings\"]";
			node = root.SelectSingleNode(rootPath);
			if (null != node)
			{
				XmlNode parent = node.ParentNode;
				parent.RemoveChild(node);
			}
			rootPath = "./configSections/sectionGroup[@name=\"MCS.Library.passport\"]";
			node = root.SelectSingleNode(rootPath);
			if (null != node)
			{
				node = node.SelectSingleNode("section[@name=\"passportSignInSettings\"]");
				if (null != node)
				{
					XmlNode parent = node.ParentNode;
					parent.RemoveChild(node);
				}
			}
			rootPath = "./MCS.Library.passport/passportSignInSettings";
			node = root.SelectSingleNode(rootPath);
			if (null != node)
			{
				XmlNode parent = node.ParentNode;
				parent.RemoveChild(node);
			}
			rootPath = "./passportSignInSettings";
			node = root.SelectSingleNode(rootPath);
			if (null != node)
			{
				XmlNode parent = node.ParentNode;
				parent.RemoveChild(node);
			}
			#endregion

			#region Passport
			string passport = string.Empty;
			if (true == string.IsNullOrEmpty(passportHostname)
				|| passportHostname.ToLower().Trim() == "localhost"
				|| passportHostname.Replace(" ", "") == "127.0.0.1")
			{
				passport = localHost;
			}
			else
			{
				passport = passportHostname;
			}
			if (false == string.IsNullOrEmpty(passportHostPort) && "80" != passportHostPort)
			{
				passport += ":" + passportHostPort;
			}

			rootPath = "./passportClientSettings/paths";
			node = root.SelectSingleNode(rootPath);
			if (null == node)
			{
				rootPath = "./MCS.Library.passport/passportClientSettings/paths";
				node = root.SelectSingleNode(rootPath);
			}
			if (null != node)
			{
				XmlNode signInNode = node.SelectSingleNode("add[@name=\"signInUrl\"]");
				XmlNode logOffNode = node.SelectSingleNode("add[@name=\"logOffUrl\"]");
				if (null != signInNode)
				{
					signInNode.Attributes["uri"].InnerText = "http://" + passport + "/PassportService/Anonymous/SignInPage.aspx";
				}
				if (null != logOffNode)
				{
					logOffNode.Attributes["uri"].InnerText = "http://" + passport + "/PassportService/Anonymous/LogOffPage.aspx";
				}
			}
			#endregion
			if (File.Exists(webConfig + ".bak"))
			{
				FileInfo fi = new FileInfo(webConfig + ".bak");
				fi.Attributes = fi.Attributes & ~FileAttributes.ReadOnly & ~FileAttributes.System;
				File.Delete(webConfig + ".bak");
			}
			File.Move(webConfig, webConfig + ".bak");

			//root.SetAttribute("1", accreditConn);
			//root.SetAttribute("2", hgLogConn);
			//root.SetAttribute("3", passportHostname);
			//root.SetAttribute("4", passportHostPort);

			configDoc.Save(webConfig);
			FileInfo nfi = new FileInfo(webConfig);
			nfi.Attributes = nfi.Attributes | FileAttributes.ReadOnly | FileAttributes.System;
		}

		private string GetLocalhost()
		{
			return System.Net.Dns.GetHostName();
		}
	}
}