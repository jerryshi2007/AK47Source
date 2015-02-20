using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 表单相关的CommandState的虚基类，携带Data属性
	/// </summary>
	[Serializable]
	public abstract class FormCommandState : CommandStateBase, ICommandStatePersist
	{
		private object data = null;

		public object Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		public virtual AppCommonInfo ToAppCommonInfo(string content)
		{
			return null;
		}

		public object CloneBusinessObject()
		{
			FormCommandState result = (FormCommandState)TypeCreator.CreateInstance(this.GetType());

			if (Data != null)
			{
				if (Data is IClonableBusinessObject)
				{
					object data = this.Data;

					result.Data = ((IClonableBusinessObject)data).GenerateNewObject();
				}
				else
				{
					BinaryFormatter bf = new BinaryFormatter();

					using (MemoryStream stream = new MemoryStream(1024))
					{
						bf.Serialize(stream, this.Data);

						stream.Position = 0;

						result.Data = bf.Deserialize(stream);
					}
				}
			}

			return result;
		}
	}
}
