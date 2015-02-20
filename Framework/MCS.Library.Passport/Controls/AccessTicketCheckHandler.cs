using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Passport;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 票据检查的所需要的事件参数
	/// </summary>
	public class AccessTicketCheckEventArgs : EventArgs
	{
		internal AccessTicketCheckEventArgs(AccessTicket ticket, bool isValid, string errorMessage)
		{
			this.Ticket = ticket;
			this.IsValid = isValid;
			this.ErrorMessage = errorMessage;
		}

		/// <summary>
		/// 检查的票据。只读属性。如果无法获得票据，则此属性为null
		/// </summary>
		public AccessTicket Ticket
		{
			get;
			private set;
		}

		/// <summary>
		/// 票据是否合法。应用可以设置
		/// </summary>
		public bool IsValid
		{
			get;
			set;
		}

		/// <summary>
		/// 错误信息
		/// </summary>
		public string ErrorMessage
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 票据检查时抛出的事件。应用可以接收到此事件，进行自己的判断。在eventArgs中，
	/// 此控件已经进行了自己的判断，应用可以修改此结果
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="eventArgs"></param>
	public delegate void AccessTicketCheckHandler(object sender, AccessTicketCheckEventArgs eventArgs);
}
