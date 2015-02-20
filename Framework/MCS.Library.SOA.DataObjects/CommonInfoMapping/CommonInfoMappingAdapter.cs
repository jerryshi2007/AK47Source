using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	public class CommonInfoMappingAdapter : UpdatableAndLoadableAdapterBase<CommonInfoMapping, CommonInfoMappingCollection>
	{
		public static readonly CommonInfoMappingAdapter Instance = new CommonInfoMappingAdapter();

		private CommonInfoMappingAdapter()
		{
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}

		public void Update(CommonInfoMappingCollection cimItems)
		{
			cimItems.NullCheck("cimItems");

			if (cimItems.Count >0)
			{
				using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
				{
					CommonInfoMappingSettings.GetConfig().Operations.ForEach(op => op.Update(cimItems));

					scope.Complete();
				}
			}
		}
	}
}
