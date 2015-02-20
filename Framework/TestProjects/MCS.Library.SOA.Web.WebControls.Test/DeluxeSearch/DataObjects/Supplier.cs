
using System;
using System.Data.SqlTypes;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.Web.WebControls.Test.DeluxeSearch
{
/// <summary>
    /// 供应商
    /// </summary>
	[Serializable]
    [XElementSerializable]
    [ORTableMapping("Supplier")]
    public class Supplier
    {  
		/// <summary>
		/// 编码
		/// </summary>
		[ORFieldMapping("Code",PrimaryKey = true)]		
		public String Code
		{
			get; set;
		}
		/// <summary>
		/// 供应商申请单编码
		/// </summary>
		[ORFieldMapping("SupplierApplicationCode")]		
		public String SupplierApplicationCode
		{
			get; set;
		}
		/// <summary>
		/// 供应商主数据编码
		/// </summary>
		[ORFieldMapping("SupplierCode")]		
		public String SupplierCode
		{
			get; set;
		}
		/// <summary>
		/// 供应商级别编码
		/// </summary>
		[ORFieldMapping("SupplierLevelCode")]
		public Int32 SupplierLevelCode
		{
			get; set;
		}
		/// <summary>
		/// 供应商公司编码
		/// </summary>
		[ORFieldMapping("SupplierCompanyCode")]		
		public String SupplierCompanyCode
		{
			get; set;
		}
        ///// <summary>
        ///// 供应商管理区域编码
        ///// </summary>
        //[ORFieldMapping("SupplierRegionalCode")]		
        //public String SupplierRegionalCode
        //{
        //    get; set;
        //}
		/// <summary>
		/// 总部地址
		/// </summary>
		[ORFieldMapping("Address")]		
		public String Address
		{
			get; set;
		}
		/// <summary>
		/// 股东发起人及比例
		/// </summary>
		[ORFieldMapping("Shareholder")]		
		public String Shareholder
		{
			get; set;
		}
		/// <summary>
		/// 总资产
		/// </summary>
		[ORFieldMapping("TotalAssets")]		
		public String TotalAssets
		{
			get; set;
		}
		/// <summary>
		/// 近三年主要业绩
		/// </summary>
		[ORFieldMapping("MainResults")]		
		public String MainResults
		{
			get; set;
		}
		/// <summary>
		/// 单位体系认证
		/// </summary>
		[ORFieldMapping("Authentication")]		
		public String Authentication
		{
			get; set;
		}
		/// <summary>
		/// 单位体系认证复印件
		/// </summary>
		[ORFieldMapping("AuthenticationAttachment")]		
		public String AuthenticationAttachment
		{
			get; set;
		}
		/// <summary>
		/// 其他
		/// </summary>
		[ORFieldMapping("Other")]		
		public String Other
		{
			get; set;
		}
		/// <summary>
		/// 直接备选理由
		/// </summary>
		[ORFieldMapping("DirectReason")]		
		public String DirectReason
		{
			get; set;
		}
		/// <summary>
		/// 主力产品种类
		/// </summary>
		[ORFieldMapping("MainProductCategories")]		
		public String MainProductCategories
		{
			get; set;
		}
		/// <summary>
		/// 法人代表
		/// </summary>
		[ORFieldMapping("LegalPerson")]		
		public String LegalPerson
		{
			get; set;
		}
		/// <summary>
		/// 注册地址
		/// </summary>
		[ORFieldMapping("RegisterAddress")]		
		public String RegisterAddress
		{
			get; set;
		}
		/// <summary>
		/// 注册资本
		/// </summary>
		[ORFieldMapping("RegisterMoney")]		
		public Decimal RegisterMoney
		{
			get; set;
		}
		/// <summary>
		/// 注册年份
		/// </summary>
		[ORFieldMapping("RegisterYear")]		
		public String RegisterYear
		{
			get; set;
		}
		/// <summary>
		/// 营业执照编号
		/// </summary>
		[ORFieldMapping("BusinessLicenceNumber")]		
		public String BusinessLicenceNumber
		{
			get; set;
		}
		/// <summary>
		/// 组织机构代码证号
		/// </summary>
		[ORFieldMapping("OrgnizationCodeNumber")]		
		public String OrgnizationCodeNumber
		{
			get; set;
		}
		/// <summary>
		/// 营业执照有效开始日期
		/// </summary>
		[ORFieldMapping("BusinessLicenceStartDate")]		
		public DateTime BusinessLicenceStartDate
		{
			get; set;
		}
		/// <summary>
		/// 营业执照有效结束日期
		/// </summary>
		[ORFieldMapping("BusinessLicenceEndDate")]		
		public DateTime BusinessLicenceEndDate
		{
			get; set;
		}
		/// <summary>
		/// 税务登记证号
		/// </summary>
		[ORFieldMapping("TaxNo")]		
		public String TaxNo
		{
			get; set;
		}
		/// <summary>
		/// 供应商公司名称
		/// </summary>
		[ORFieldMapping("SupplierCompanyCnName")]		
		public String SupplierCompanyCnName
		{
			get; set;
		}
		/// <summary>
		/// 供应商公司名称英文
		/// </summary>
		[ORFieldMapping("SupplierCompanyEnName")]		
		public String SupplierCompanyEnName
		{
			get; set;
		}
		/// <summary>
		/// 供应商名称
		/// </summary>
		[ORFieldMapping("SupplierName")]		
		public String SupplierName
		{
			get; set;
		}
		/// <summary>
		/// 公司网站
		/// </summary>
		[ORFieldMapping("WebSite")]		
		public String WebSite
		{
			get; set;
		}
		/// <summary>
		/// 状态
		/// </summary>
		[ORFieldMapping("Status")]		
		public Int32 Status
		{
			get; set;
		}
		/// <summary>
		/// 创建人
		/// </summary>
		[ORFieldMapping("Creator")]		
		public String Creator
		{
			get; set;
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		[ORFieldMapping("CreateTime")]		
		public DateTime CreateTime
		{
			get; set;
		}
		/// <summary>
		/// 修改人
		/// </summary>
		[ORFieldMapping("Modifier")]		
		public String Modifier
		{
			get; set;
		}
		/// <summary>
		/// 修改时间
		/// </summary>
		[ORFieldMapping("ModifyTime")]		
		public DateTime ModifyTime
		{
			get; set;
		}
		/// <summary>
		/// 有效性
		/// </summary>
		[ORFieldMapping("ValidStatus")]	
		public Boolean ValidStatus
		{
			get; set;
		}
		/// <summary>
		/// 供应商主数据编码
		/// </summary>
		[ORFieldMapping("SubjectSupplierCode")]		
		public String SubjectSupplierCode
		{
			get; set;
		}
		public Supplier()
		{
            Code = string.Empty;
            SupplierApplicationCode = string.Empty;
            SupplierCode = string.Empty;
            SupplierLevelCode = 0;
            SupplierCompanyCode = string.Empty;
            //SupplierRegionalCode = string.Empty;
            Address = string.Empty;
            Shareholder = string.Empty;
            TotalAssets = string.Empty;
            MainResults = string.Empty;
            Authentication = string.Empty;
            AuthenticationAttachment = string.Empty;
            Other = string.Empty;
            DirectReason = string.Empty;
            MainProductCategories = string.Empty;
            LegalPerson = string.Empty;
            RegisterAddress = string.Empty;
            RegisterMoney = 0;
            RegisterYear = string.Empty;
            BusinessLicenceNumber = string.Empty;
            OrgnizationCodeNumber = string.Empty;
            BusinessLicenceStartDate = SqlDateTime.MinValue.Value;
            BusinessLicenceEndDate = SqlDateTime.MinValue.Value;
            TaxNo = string.Empty;
            SupplierCompanyCnName = string.Empty;
            SupplierCompanyEnName = string.Empty;
            SupplierName = string.Empty;
            WebSite = string.Empty;
            Status = 0;
            Creator = string.Empty;
            CreateTime = SqlDateTime.MinValue.Value;
            Modifier = string.Empty;
            ModifyTime = SqlDateTime.MinValue.Value;
            ValidStatus = false;
            SubjectSupplierCode = string.Empty;
		}
    }
    [Serializable]
    [XElementSerializable]
    public class SupplierCollection : EditableDataObjectCollectionBase<Supplier>
    {
    }
}