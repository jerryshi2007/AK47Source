
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test.DeluxeSearch
{
    public class SupplierQueryAdapter : ObjectDataSourceQueryAdapterBase<Supplier, SupplierCollection>
    {

        protected override string GetConnectionName()
        {
            return DatabaseUtils.SinooceanProductresearch;
        }
        /// <summary>
        /// 指定查询条件
        /// </summary>
        /// <param name="qc"></param>
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.SelectFields = @"Code,SupplierApplicationCode,SupplierCode,SupplierLevelCode,SupplierCompanyCode,Address,Shareholder,TotalAssets,MainResults,Authentication,AuthenticationAttachment,Other,DirectReason,MainProductCategories,LegalPerson,RegisterAddress,RegisterMoney,RegisterYear,BusinessLicenceNumber,OrgnizationCodeNumber,BusinessLicenceStartDate,BusinessLicenceEndDate,TaxNo,SupplierCompanyCnName,SupplierCompanyEnName,SupplierName,WebSite,Status,Creator,CreateTime,Modifier,ModifyTime,ValidStatus";
            qc.FromClause = ORMapping.GetMappingInfo<Supplier>().TableName;
            qc.OrderByClause = GetOrderByString(qc);
            base.OnBuildQueryCondition(qc);
        }

        /// <summary>
        /// 获取排序字串
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        protected string GetOrderByString(QueryCondition qc)
        {
            //排序
            if (string.IsNullOrEmpty(qc.OrderByClause))
                qc.OrderByClause = "SupplierCode";
            else
                qc.OrderByClause = string.Format("{0}, SupplierCode", qc.OrderByClause);

            return qc.OrderByClause;
        }
    }
}