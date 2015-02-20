using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.Client;

namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	public class ClientAUSchemaCategory
	{
		/// <summary>
		/// 分类ID
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 分类名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 分类的版本开始时间
		/// </summary>
		public DateTime VersionStartTime { get; set; }
		/// <summary>
		/// 分类的版本结束时间
		/// </summary>
		public DateTime VersionEndTime { get; set; }
		/// <summary>
		/// 分类的状态
		/// </summary>
		public ClientSchemaObjectStatus Status { get; set; }
		/// <summary>
		/// 分类的全路径
		/// </summary>
		public string FullPath { get; set; }
		/// <summary>
		/// 上级分类的ID
		/// </summary>
		public string ParentID { get; set; }

		public int Rank { get; set; }
	}
}
