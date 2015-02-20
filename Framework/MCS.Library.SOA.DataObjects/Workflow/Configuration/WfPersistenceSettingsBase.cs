using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程保存额外的扩展信息的基类
	/// </summary>
	public abstract class WfPersistenceSettingsBase : ConfigurationSection
	{
		private bool _UseDefault = false;
		private object _SyncObject = new object();
		private WfExtraProcessPersistManagerCollection _PersistersList = null;

		protected WfPersistenceSettingsBase()
		{
		}

		protected WfPersistenceSettingsBase(bool useDefault)
		{
			this._UseDefault = useDefault;
		}

		[ConfigurationProperty("persisters")]
		protected TypeConfigurationCollection Persisters
		{
			get
			{
				return (TypeConfigurationCollection)this["persisters"];
			}
		}

		public WfExtraProcessPersistManagerCollection GetPersisters()
		{
			if (this._PersistersList == null)
			{
				lock (this._SyncObject)
				{
					if (this._PersistersList == null)
					{
						if (this._UseDefault)
							this._PersistersList = GetDefaultPersisters();
						else
							this._PersistersList = GetConfigedPersisters();
					}
				}
			}

			return this._PersistersList;
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return true;
		}

		protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
		{
			return true;
		}

		protected virtual WfExtraProcessPersistManagerCollection GetDefaultPersisters()
		{
			WfExtraProcessPersistManagerCollection result = new WfExtraProcessPersistManagerCollection();

			return result;
		}

		private WfExtraProcessPersistManagerCollection GetConfigedPersisters()
		{
			WfExtraProcessPersistManagerCollection result = new WfExtraProcessPersistManagerCollection();

			foreach (TypeConfigurationElement typeElement in this.Persisters)
				result.Add(typeElement.CreateInstance<IWfExtraProcessPersistManager>());

			return result;
		}
	}
}
