#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	NotEnlistDbContext.cs
// Remark	��	Generic database processing context��
// -------------------------------------------------
//	1.2			���			20080919		��ԭ��DbContext�Ĵ���Ǩ�ƹ���
// -------------------------------------------------
#endregion

#region using

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Data.Common;
using System.Transactions;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data.Properties;

#endregion

namespace MCS.Library.Data
{
	[System.Diagnostics.DebuggerNonUserCode]
	internal class AutoEnlistDbContext : DeluxeDbContextBase
	{
		#region private classes
		private class GraphWithTransaction : Dictionary<Transaction, Connections>
		{
		}
		#endregion

		#region Private members

		/// <summary>
		/// Current context entity management target (with transaction support).
		/// <remarks>
		///     the Key type is a System.Transaction.Transaction
		/// </remarks>
		/// </summary>
		private static GraphWithTransaction graphWithTx = null;
		#endregion

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="name">��������</param>
		/// <param name="autoClose">�Ƿ��Զ��ر�����</param>
		public AutoEnlistDbContext(string name, bool autoClose)
			: base(name, autoClose)
		{
		}

		#region Protected methods
        /// <summary>
        /// ��д��ȡ��ǰ������ʹ�õ��������ӵĺ���
        /// </summary>
        /// <param name="ts">�������</param>
        /// <returns>��������</returns>
		protected override DbConnection OnGetConnectionWithTransaction(Transaction ts)
		{
			Connections connections = null;

			GraphWithTransaction graph = AutoEnlistDbContext.GraphWithTx;

			lock (graph)
			{
				// current transaction exists only in current HttpContext or Thread
				if (graph.TryGetValue(ts, out connections) == false)
				{
					connections = new Connections();
					graph.Add(Transaction.Current, connections);
				}
			}

			ReferenceConnection rConnection = null;

			lock (connections)
			{
				if (connections.TryGetValue(this.Name, out rConnection) == false)
				{
					rConnection = new ReferenceConnection(this.Name, DbConnectionManager.GetConnection(this.Name));
					IsConnectionCreator = true;

					connections.Add(this.Name, rConnection);
				}
				else
					rConnection.ReferenceCount++;
			}

			return rConnection.Connection;
		}
        /// <summary>
        /// �������ݿ��������ʱ�������¼�
        /// </summary>
        /// <param name="args">�¼�����</param>
		protected override void OnTransactionCompleted(TransactionEventArgs args)
		{
			GraphWithTransaction graph = GraphWithTx;

			lock (graph)
			{
				Connections connections;

				if (graph.TryGetValue(args.Transaction, out connections))
				{
					try
					{
						lock (connections)
						{
							foreach (KeyValuePair<string, ReferenceConnection> item in connections)
							{
								DbConnection connection = item.Value.Connection;

								if (connection.State != ConnectionState.Closed)
								{
									connection.Close();

									WriteTraceInfo(connection.DataSource + "." + connection.Database
										+ "[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "]",
									" Close Connection ");
								}
							}
						}
					}
					finally
					{
						graph.Remove(args.Transaction);
					}
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Get connection graph when executing without transaction.
		/// </summary>
		/// <returns></returns>
		private static GraphWithTransaction GraphWithTx
		{
			get
			{
				WriteTraceInfo("GetGraphWithTx ManagedThreadId :"
					+ Thread.CurrentThread.ManagedThreadId.ToString());

				GraphWithTransaction result;

                lock (GraphWithTxSyncObject)
				{
					if (AutoEnlistDbContext.graphWithTx == null)
						AutoEnlistDbContext.graphWithTx = new GraphWithTransaction();

					result = AutoEnlistDbContext.graphWithTx;
				}

				return result;
			}
		}
		#endregion
	}
}
