using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using MCS.Library.Core;
using MCS.Library.Security;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 对象映射过程中，属性加密和解密的类
	/// 此类非线程安全的。
	/// </summary>
	public class ORMappintItemEncryption : DESEncryptorBase
	{
		private static byte[] desKey = { 136, 183, 142, 217, 175, 71, 90, 239 };
		private static byte[] desIV = { 227, 105, 5, 40, 162, 158, 143, 156 };

		private string _Name = string.Empty;
		private DES _DES = null;

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="name"></param>
		public ORMappintItemEncryption(string name)
		{
			name.CheckStringIsNullOrEmpty("name");
			this._Name = name;
		}

		/// <summary>
		/// 名称。
		/// </summary>
		public string Name
		{
			get { return this._Name; }
			set { this._Name = value; }
		}

		/// <summary>
		/// 得到DES对象
		/// </summary>
		/// <returns></returns>
		protected override DES GetDesObject()
		{
			if (this._DES == null)
			{
				DES des = new DESCryptoServiceProvider();

				des.IV = ORMappintItemEncryption.desIV;
				des.Key = ORMappintItemEncryption.desKey;

				this._DES = des;
			}

			return this._DES;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class ORMappintItemEncryptionCollection : EditableKeyedDataObjectCollectionBase<string, ORMappintItemEncryption>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override string GetKeyForItem(ORMappintItemEncryption item)
		{
			return item.Name;
		}
	}

}
