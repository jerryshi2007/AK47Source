using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects
{
	public class GenericOpinionAdapter : UpdatableAndLoadableAdapterBase<GenericOpinion, GenericOpinionCollection>
	{
		public static readonly GenericOpinionAdapter Instance = new GenericOpinionAdapter();

		private GenericOpinionAdapter()
		{
		}

		/// <summary>
		/// 从意见ID加载
		/// </summary>
		/// <param name="opinionIDs"></param>
		/// <returns></returns>
		public GenericOpinionCollection LoadByIDs(params string[] opinionIDs)
		{
			return LoadByIDs((IEnumerable<string>)opinionIDs);
		}

		/// <summary>
		/// 从意见ID加载
		/// </summary>
		/// <param name="opinionIDs"></param>
		/// <returns></returns>
		public GenericOpinionCollection LoadByIDs(IEnumerable<string> opinionIDs)
		{
			opinionIDs.NullCheck("opinionIDs");

			return LoadByInBuilder(inBuilder =>
			{
				inBuilder.DataField = "ID";
				opinionIDs.ForEach(oID => inBuilder.AppendItem(oID));
			},
			orderBuilder => orderBuilder.AppendItem("ISSUE_DATETIME", FieldSortDirection.Ascending));
		}

		/// <summary>
		/// 查找对应文件的所有意见信息
		/// </summary>
		/// <param name="resourceID"></param>
		/// <returns></returns>
		public GenericOpinionCollection LoadFromResourceID(string resourceID)
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");

			GenericOpinionCollection result = Load(
				whereBuilder =>
				{
					whereBuilder.AppendItem("RESOURCE_ID", resourceID);
					whereBuilder.AppendItem("PROCESS_ID", "AbortProcess", "<>");
				},
				orderBuilder => orderBuilder.AppendItem("ISSUE_DATETIME", FieldSortDirection.Ascending));

			return result;
		}

		/// <summary>
		/// 得到作废流程的意见
		/// </summary>
		/// <param name="resourceID"></param>
		/// <returns></returns>
		public GenericOpinion LoadAbortProcessOpinion(string resourceID)
		{
			return LoadAbortProcessOpinions(resourceID).FirstOrDefault();
		}

		/// <summary>
		/// 得到作废流程的意见列表
		/// </summary>
		/// <param name="resourceID"></param>
		/// <returns></returns>
		public GenericOpinionCollection LoadAbortProcessOpinions(string resourceID)
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");

			return Load(builder =>
			{
				builder.AppendItem("RESOURCE_ID", resourceID);
				builder.AppendItem("PROCESS_ID", "AbortProcess");
			}, builder =>
			{
				builder.AppendItem("ISSUE_DATETIME", FieldSortDirection.Descending);
			});
		}

		protected override void BeforeInnerUpdate(GenericOpinion data, Dictionary<string, object> context)
		{
			base.BeforeInnerUpdate(data, context);

			data.AppendDatetime = DateTime.MinValue;
			data.RawExtData = GenericOpinion.ConvertExtraDataToXmlString(data.ExtData);
		}
	}
}
