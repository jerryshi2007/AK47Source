using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.Accredit;
using MCS.Library.Configuration;
using MCS.Library.Services;
using MCS.Library.OGUPermission;

namespace MCS.Services.AD2Accredit
{
	public partial class ADToAccreditControl : UserControl
	{
		private int _ProcessedObjectsCount = 0;

		public ADToAccreditControl()
		{
			InitializeComponent();
		}

		private static ADHelper GetADHelper()
		{
			ServerInfo serverInfo = ServerInfoConfigSettings.GetConfig().ServerInfos["dc"].ToServerInfo();

			return ADHelper.GetInstance(serverInfo);
		}

		private void LoadRootNode()
		{
			using (DirectoryEntry entry = GetADHelper().NewEntry(ADToDBConfigSettings.GetConfig().RootOUPath))
			{
				//HROrganization root = ADHelper.(HRTransferConfigSection.GetConfig().SourceRootPath);

				treeView.Nodes.Add(CreateRootTreeNodeByObj(entry));
			}
		}

		private static TreeNode CreateRootTreeNodeByObj(DirectoryEntry entry)
		{
			TreeNode node = new TreeNode();

			ADHelper helper = GetADHelper();

			string name = helper.GetPropertyStrValue("displayName", entry);

			if (name.IsNullOrEmpty())
				name = helper.GetPropertyStrValue("name", entry);

			node.Text = name;
			node.Tag = helper.GetPropertyStrValue("distinguishedName", entry);

			node.Nodes.Add(CreateFakeNode());

			return node;
		}

		private void buttonRefresh_Click(object sender, EventArgs e)
		{
			LoadRootNode();
		}

		private static TreeNode CreateFakeNode()
		{
			TreeNode node = new TreeNode();

			node.Text = "Loading...";
			node.Tag = "Fake";

			return node;
		}

		private void BindSearchResult(SearchResult sr, TreeNode parent)
		{
			ADHelper helper = GetADHelper();

			string name = helper.GetSearchResultPropertyStrValue("displayName", sr);

			if (name.IsNullOrEmpty())
				name = helper.GetSearchResultPropertyStrValue("name", sr);

			TreeNode node = new TreeNode();

			node.Text = name;
			node.Tag = helper.GetSearchResultPropertyStrValue("distinguishedName", sr);

			parent.Nodes.Add(node);

			string objectClass = AD2DBHelper.TranslateObjectClass(sr.Properties["objectClass"][1].ToString());

			int imageIndex = 0;

			switch (objectClass)
			{
				case "ORGANIZATIONS":
					node.Nodes.Add(CreateFakeNode());
					break;
				case "USERS":
					imageIndex = 1;

					if (helper.GetUserAccountPolicy(sr).UserAccountDisabled)
						imageIndex = 3;

					break;
				case "GROUPS":
					imageIndex = 2;
					break;
			}

			node.ImageIndex = imageIndex;
			node.SelectedImageIndex = imageIndex;
		}

		private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
		{
			if (e.Action == TreeViewAction.Expand)
			{
				if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Tag != null && e.Node.Nodes[0].Tag.ToString() == "Fake")
				{
					if (e.Node.Tag != null)
					{
						ADHelper helper = GetADHelper();

						ADSearchConditions conditions = new ADSearchConditions(SearchScope.OneLevel);

						using (DirectoryEntry entry = helper.NewEntry(e.Node.Tag.ToString()))
						{
							e.Node.Nodes.Clear();

							foreach (SearchResult sr in helper.ExecuteSearch(entry,
								ADSearchConditions.GetFilterByMask(ADSchemaType.Groups | ADSchemaType.Users | ADSchemaType.Organizations), conditions))
							{
								BindSearchResult(sr, e.Node);
							}
						}
					}
				}
			}
		}

		private void buttonConvert_Click(object sender, EventArgs e)
		{
			using (AD2DBInitialParams initParams = AD2DBInitialParams.GetParams())
			{
				textBoxLog.Text = string.Empty;

				ServiceLog log = new ServiceLog("ADToAccredit");

				log.AddTextBoxTraceListener(textBoxLog);

				initParams.Log = log;

				this._ProcessedObjectsCount = 0;

				OutputStatus();

				initParams.BeforeProcessADObject += new BeforeProcessADObjectDelegate(initParams_BeforeProcessADObject);

				AD2DBConvertion converter = new AD2DBConvertion(initParams);

				converter.DoConvert();

				OutputStatus();

				OguMechanismFactory.GetMechanism().RemoveAllCache();
				PermissionMechanismFactory.GetMechanism().RemoveAllCache();
			}
		}

		private void initParams_BeforeProcessADObject(SearchResult sr, AD2DBInitialParams initParams, ref bool bContinue)
		{
			this._ProcessedObjectsCount++;

			if (this._ProcessedObjectsCount % 100 == 0)
				OutputStatus();

			Application.DoEvents();
		}

		private void OutputStatus()
		{
			labelStatus.Text = string.Format("Processed: {0:#,##0}", this._ProcessedObjectsCount);
		}
	}
}
