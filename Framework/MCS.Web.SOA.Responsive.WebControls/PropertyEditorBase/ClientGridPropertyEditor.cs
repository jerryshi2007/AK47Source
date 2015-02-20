using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.UI;
using System.Reflection;
using MCS.Library.Caching;
using System.Web.UI.WebControls;
using MCS.Library.Core;

namespace MCS.Web.Responsive.WebControls
{
    [PropertyEditorDescription("ClientGridPropertyEditor", "MCS.Web.WebControls.ClientGridPropertyEditor")]
	public class ClientGridPropertyEditor : PropertyEditorBase
	{
		public override bool IsCloneControlEditor
		{
			get
			{
				return true;
			}
		}

		public override string DefaultCloneControlName()
		{
			return "ClientGridEditor_ClientGrid";
		}

		public override Control CreateDefalutControl()
		{
			return new ClientGrid() { ID = this.DefaultCloneControlName(), EnableViewState = false };
		}

		private void SettleControlColumnsFromXmlNode(Control currentControl, XmlNode rootNode)
		{
			foreach (XmlNode node in rootNode.ChildNodes)
			{
				ClientGridColumn column = new ClientGridColumn();

				foreach (XmlAttribute attr in node.Attributes)
				{
					PropertyInfo piDest = TypePropertiesCacheQueue.Instance.GetPropertyInfo(column.GetType(), attr.Name);
					if (piDest != null)
					{
						if (string.IsNullOrEmpty(attr.Value) || piDest.CanWrite == false)
							continue;

						if (piDest.PropertyType == typeof(Unit))
							piDest.SetValue(column, Unit.Parse(attr.Value), null);
						else if (piDest.PropertyType == typeof(bool))
						{
							piDest.SetValue(column, bool.Parse(attr.Value), null);
						}
						else
							piDest.SetValue(column, attr.Value, null);
					}
				}

				var subNode = node.SelectSingleNode("editTemplate");

				if (subNode != null)
				{
					column.EditTemplate = new ClientGridColumnEditTemplate();

					foreach (XmlAttribute subAttr in subNode.Attributes)
					{
						PropertyInfo piDest = TypePropertiesCacheQueue.Instance.GetPropertyInfo(column.EditTemplate.GetType(), subAttr.Name);
						if (piDest != null)
						{
							if (string.IsNullOrEmpty(subAttr.Value) || piDest.CanWrite == false)
								continue;

							if (piDest.PropertyType.IsEnum)
							{
								var enumValue = DataConverter.ChangeType(subAttr.Value, piDest.PropertyType);
								piDest.SetValue(column.EditTemplate, enumValue, null);
							}
							else if (piDest.PropertyType == typeof(Unit))
								piDest.SetValue(column.EditTemplate, Unit.Parse(subAttr.Value), null);
							else if (piDest.PropertyType == typeof(bool))
							{
								piDest.SetValue(column.EditTemplate, bool.Parse(subAttr.Value), null);
							}
							else
								piDest.SetValue(column.EditTemplate, subAttr.Value, null);
						}
					}
				}

				ClientGrid gridControl = (ClientGrid)currentControl;
				gridControl.Columns.Add(column);
			}
		}

		public override void InitCloneControlProperties(Control currentControl, ControlPropertyDefineWrapper propertyDefineWrapper)
		{
			base.InitCloneControlProperties(currentControl, propertyDefineWrapper);

			if (!propertyDefineWrapper.UseTemplate)
			{
				var extendedSettings = propertyDefineWrapper.ExtendedSettings;

				if (extendedSettings.IsNotEmpty())
				{
					XmlDocument xmlDoc = new XmlDocument();
					xmlDoc.LoadXml(extendedSettings);

					if (xmlDoc.ChildNodes.Count > 0)
					{
						SettleControlColumnsFromXmlNode(currentControl, xmlDoc.ChildNodes[0]);
					}
				}
			}
		}

		protected internal override void OnPageInit(Page page)
		{
			//Callback时，提前创建模版控件，拦截请求
			//if (page.IsCallback)
			//CreateControls(page);
		}

		protected internal override void OnPagePreRender(Page page)
		{
			//除了CallBack，创建模版控件在LoadViewState之后，避免ViewState混乱
			//if (page.IsCallback == false)
			CreateControls(page);
		}
	}
}
