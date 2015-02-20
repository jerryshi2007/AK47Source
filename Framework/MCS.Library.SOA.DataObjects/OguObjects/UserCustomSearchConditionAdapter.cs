using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
    public class UserCustomSearchConditionAdapter : UpdatableAndLoadableAdapterBase<UserCustomSearchCondition,UserCustomSearchConditionCollection>
    {
        public static readonly UserCustomSearchConditionAdapter Instance = new UserCustomSearchConditionAdapter();

		/// <summary>
		/// 构造函数
		/// </summary>
        private UserCustomSearchConditionAdapter()
		{

		}


	    protected override string GetConnectionName()
        {
            return ConnectionDefine.UserRelativeInfoConnectionName;
        }
    }
}
