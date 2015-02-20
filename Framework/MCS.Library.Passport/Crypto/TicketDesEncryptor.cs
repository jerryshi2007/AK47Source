using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 使用Des算法的Ticket加密类
	/// </summary>
	public class TicketDesEncryption : ITicketEncryption
	{
		#region ITicketEncryption Members

		/// <summary>
		/// 加密票据
		/// </summary>
		/// <param name="ticket"></param>
		/// <param name="oParam"></param>
		/// <returns></returns>
		public byte[] EncryptTicket(ITicket ticket, object oParam)
		{
			StringEncryption encoder = new StringEncryption();

			return encoder.EncryptString(ticket.SaveToXml().InnerXml);
		}

		/// <summary>
		/// 解密票据
		/// </summary>
		/// <param name="encryptedData"></param>
		/// <param name="oParam"></param>
		/// <returns></returns>
		public ITicket DecryptTicket(byte[] encryptedData, object oParam)
		{
			StringEncryption decoder = new StringEncryption();

			string xml = decoder.DecryptString(encryptedData);

			return new Ticket(xml);
		}

		#endregion
	}
}
