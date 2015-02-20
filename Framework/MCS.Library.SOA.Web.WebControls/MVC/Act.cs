using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MCS.Library.Core;

namespace MCS.Web.Library.MVC
{
	[Serializable]
	public class Act
	{
		private string actID = string.Empty;
		private SceneCollection scenes = null;

		internal Act(XmlNode node)
		{
			this.actID = XmlHelper.GetAttributeText(node, "actID");
			this.scenes = new SceneCollection();

			foreach(XmlElement elem in node.SelectNodes("Scene"))
				scenes.Add(new Scene(elem));
		}

		public string ActID
		{
			get
			{
				return this.actID;
			}
			set
			{
				this.actID = value;
			}
		}

		public SceneCollection Scenes
		{
			get
			{
				if (this.scenes == null)
					this.scenes = new SceneCollection();

				return this.scenes;
			}
		}
	}

	[Serializable]
	public class ActCollection : KeyedCollection<string, Act>
	{
		internal ActCollection()
		{
		}

		protected override string  GetKeyForItem(Act item)
		{
			return item.ActID;
		}
	}
}
