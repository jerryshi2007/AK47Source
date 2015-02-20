using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Builder;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter
{
    public class AllUserExportAction : ExportAction
    {
        public override string CategoryName
        {
            get
            {
                return "所有人员";
            }
        }

        public override SCObjectSet Execute(HttpRequest req)
        {
            string[] ids = req.Form.GetValues("id");

            if (ids == null || ids.Length < 0)
                throw new HttpException("当获取人员数据时，必须提供ID参数");

            SCObjectSet objectSet = new SCObjectSet();
            objectSet.Scope = "AllUsers";

            objectSet.Objects = ExportQueryHelper.LoadObjects(ids, null);

            objectSet.Relations = ExportQueryHelper.LoadFullRelations(ids);

            objectSet.Membership = ExportQueryHelper.LoadFullMemberships(ids);

            return objectSet;
        }
    }
}