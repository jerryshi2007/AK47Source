using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Office.OpenXml.Excel.Encryption;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class WorkBookProtection : ElementInfo
	{
		//"workbookPassword", 
		//    "revisionsPassword", 
		//    "lockRevision", 
		//    "revisionsAlgorithmName", 
		//    "revisionsHashValue", 
		//    "revisionsSaltValue", 
		//    "revisionsSpinCount", 
		//    "workbookAlgorithmName", 
		//    "workbookHashValue", 
		//    "workbookSaltValue", 
		//    "workbookSpinCount"

		/// <summary>
		/// 否锁定工作簿结构
		/// 如果为True 工作表中 工作簿不能移动，删除，隐藏，取消隐藏，重命名，和新的工作表。
		/// </summary>
		public bool LockStructure
		{
			get
			{
				return base.GetBooleanAttribute("lockStructure");
			}
			set
			{
				base.SetBooleanAttribute("lockStructure", value);
			}
		}

		/// <summary>
		/// 如果为True 工作簿窗口被锁定(每次打开窗口固定大小)
		/// </summary>
		public bool LockWindows
		{
			get
			{
				return base.GetBooleanAttribute("lockWindows");
			}
			set
			{
				base.SetBooleanAttribute("lockWindows", value);
			}

		}

		/// <summary>
		/// 是否锁定修订工作簿
		/// </summary>
		public bool LockRevision
		{
			get
			{
				return base.GetBooleanAttribute("lockRevision");
			}
			set
			{
				base.SetBooleanAttribute("lockRevision", value);
			}
		}

		//todo:
		/// <summary>
		/// 加密类型
		/// </summary>
		// public bool revisionsAlgorithmName

		/// <summary>
		/// 设置工作簿的密码。这不加密的工作簿。 
		/// </summary>
		/// <param name="Password">密码 </param>
		public void SetPassword(string Password)
		{
			if (string.IsNullOrEmpty(Password))
			{
				base.Attributes.Remove("workbookPassword");
			}
			else
			{
				base.SetAttribute("workbookPassword", ((int)EncryptedPackageHandler.CalculatePasswordHash(Password)).ToString("x"));
			}
		}

		protected internal override string NodeName
		{
			get { return "workbookProtection"; }
		}
	}
}
