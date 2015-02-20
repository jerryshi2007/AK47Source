using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel.Encryption
{
	/// <summary>
	/// 加密算法
	/// </summary>
	public enum EncryptionAlgorithm
	{
		/// <summary>
		/// 128-bit AES. Default
		/// </summary>
		AES128,
		/// <summary>
		/// 192-bit AES.
		/// </summary>
		AES192,
		/// <summary>
		/// 256-bit AES. 
		/// </summary>
		AES256
	}

	/// <summary>
	/// 如果工作薄加密
	///<seealso cref="ExcelProtection"/> 
	///<seealso cref="ExcelSheetProtection"/> 
	/// </summary>
	public class ExcelEncryption
	{
		/// <summary>
		/// 构造
		/// </summary>
		public ExcelEncryption()
		{
			Algorithm = EncryptionAlgorithm.AES128;
		}

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="encryptionAlgorithm">算法用来加密包。默认是为aes128</param>
		public ExcelEncryption(EncryptionAlgorithm encryptionAlgorithm)
		{
			Algorithm = encryptionAlgorithm;
		}

		private bool _IsEncrypted = false;

		/// <summary>
		/// 加密包
		/// </summary>
		public bool IsEncrypted
		{
			get
			{
				return this._IsEncrypted;
			}
			set
			{
				this._IsEncrypted = value;
				if (this._IsEncrypted)
				{
					if (this._Password == null)
						this._Password = string.Empty;
				}
				else
				{
					this._Password = null;
				}
			}
		}

		private string _Password = null;
		/// <summary>
		/// 使用的密码加密的工作簿。
		/// </summary>
		public string Password
		{
			get
			{
				return this._Password;
			}
			set
			{
				this._Password = value;
				this._IsEncrypted = (value != null);
			}
		}

		/// <summary>
		/// 算法用于加密包。默认是128位AES
		/// </summary>
		public EncryptionAlgorithm Algorithm { get; set; }
	}
}
