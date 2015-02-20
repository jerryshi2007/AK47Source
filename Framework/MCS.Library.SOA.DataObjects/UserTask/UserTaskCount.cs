using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	public class UserTaskCount
	{
		int banCount = 0;
		int yueCount = 0;
		int banExpiredCount = 0;
		int yueExpiredCount = 0;

		internal UserTaskCount(DataTable table)
		{
			foreach (DataRow row in table.Rows)
			{
				if ((string)row["EXPIRED"] == "TOTAL")
				{
					if ((string)row["STATUS"] == ((int)TaskStatus.Ban).ToString())
						this.banCount = (int)row["COUNT"];
					else
						this.yueCount = (int)row["COUNT"];
				}
				else
					if ((string)row["EXPIRED"] == "EXPIRED")
					{
						if ((string)row["STATUS"] == ((int)TaskStatus.Ban).ToString())
							this.banExpiredCount = (int)row["COUNT"];
						else
							this.yueExpiredCount = (int)row["COUNT"];
					}
			}
		}

		public UserTaskCount()
		{
		}

		public int BanCount
		{
			get
			{
				return this.banCount;
			}
			set
			{
				this.banCount = value;
			}
		}

		public int YueCount
		{
			get
			{
				return this.yueCount;
			}
			set
			{
				this.yueCount = value;
			}
		}

		public int BanExpiredCount
		{
			get
			{
				return this.banExpiredCount;
			}
			set
			{
				this.banExpiredCount = value;
			}
		}

		public int YueExpiredCount
		{
			get
			{
				return this.yueExpiredCount;
			}
			set
			{
				this.yueExpiredCount = value;
			}
		}
	}
}
