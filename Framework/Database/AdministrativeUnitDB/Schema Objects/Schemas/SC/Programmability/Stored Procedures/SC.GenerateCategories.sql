--生成分类目录数据

CREATE PROCEDURE [SC].[GenerateCategories]
	
AS
	--两级分类
DECLARE @xml xml

SET @xml = '<dom>
<category name="综合管理" key="9ce0ce56-144f-4771-8309-3cda2cf99b40">
	<category name="综合管理" key="9ce0ce56-144f-4771-8309-3cda2cf99b40">
		<category name="战略定位" key="cd66b4ec-9402-427d-8c4a-a4cc2d451227" />
		<category name="战略规划" key="2b253faa-4a44-4f56-a6af-e7c68ef99cec" />
		<category name="绩效规划" key="9faa5b17-8e55-41fe-bf45-af47df811ace" />
		<category name="产品线研究" key="ef9dab43-274c-43f6-82c6-c784ca38f6f0" />
		<category name="融资管理" key="69b07693-9b1f-4a1f-a318-0c9b7e260451" />
		<category name="现金管理" key="8ed32d6e-1ac5-479e-ae85-cf55586e7c06" />
		<category name="税费管理" key="0717f454-9309-481b-a01a-8fd9e2b7c585" />
		<category name="收费管理" key="8a321b82-406c-4511-b9f7-0a4216fb664c" />
		<category name="支付管理" key="8b1d5749-40c3-4bdf-a17e-dfa886591a27" />
		<category name="经营管理" key="0c58c694-9e41-488d-86d7-2e9b9f9e9d41" />
		<category name="人员提供" key="43f2cad2-28c9-49fd-9dcf-f8c76074ceda" />
		<category name="员工服务" key="8dcce828-400b-422e-b286-1d0c2ff3befe" />
		<category name="人员考核" key="5a699026-e7bc-4470-8440-c5d41e4411e5" />
		<category name="人员激励" key="e11e7747-f3bc-4189-83b7-5235a0347835" />
		<category name="职业发展规划" key="cd2a950b-9bfe-46cc-9a89-48e86523274b" />
		<category name="秘书服务" key="1fbc9408-1f86-457c-9fa3-d0d9f0f1f7df" />
		<category name="行政服务" key="a4194b39-10ff-4eb3-afc7-ca0610cbd55e" />
		<category name="档案管理" key="e53227ec-7e0c-46e6-9ced-f25c1ccaf601" />
		<category name="资料管理" key="aff900b4-5210-4508-9e12-45156267e99d" />
		<category name="固定资产管理" key="fdd129dd-2556-4c76-83dd-f72fcc2a7d99" />
		<category name="政府关系" key="5de85c46-3a03-4b32-8cdc-0c28ebf500e0" />
		<category name="会员关系" key="9e0b88b2-f4f9-4d7a-8a39-ffb9002d49aa" />
		<category name="客户满意度" key="9e6a59d7-168b-42e5-973c-8a27b84d1898" />
		<category name="联盟商家管理" key="5b973ebd-20d2-4267-854a-26ed23e6b53c" />
		<category name="部品材料管理" key="64870096-3a60-4a68-8b9f-0ebde9657c3a" />
		<category name="供应商关系" key="0d46c322-deb1-4660-a412-c27bdce6f78d" />
		<category name="内部审计" key="4c1fa2a2-6ab3-4eea-8226-178527ea2f62" />
		<category name="设计变更与工程洽商" key="83117ddb-7b38-47cc-979e-1e02cb427361" />
		<category name="采购管理" key="c5227f42-92d6-4aae-8dff-a6cf677e0904" />
		<category name="合同管理" key="12b51570-86cc-49f5-88ac-51d8d239e4c0" />
		<category name="合同结算" key="90056ded-07d5-435b-952f-4a0e2976087f" />
		<category name="品牌管理" key="ca8a76fc-2ced-449a-816d-a404f3789887" />
		<category name="媒体关系" key="f5bc0035-6c13-4092-84a9-cafa1d485bbd" />
		<category name="投资者关系" key="b03b2cdc-9d4e-4412-8c80-0fdc8ac753ea" />
		<category name="董事局事务" key="9130df0c-eac0-4b62-b9bb-c018ff057a67" />
		<category name="案件办理" key="9221f51a-7cce-486b-9bb0-26bcb0d26a58" />
		<category name="股权管理" key="1980ae7a-ddb0-4327-999c-c937ae1ee993" />
		<category name="经营性资质办理" key="1710dc1e-bc43-42a3-8323-1b5a3148a4b6" />
		<category name="流程管理" key="2a290142-348c-4d0b-85a2-b739a18771a0" />
		<category name="知识管理" key="83e4f5b9-606c-4e67-8601-4a09fdb67fc7" />
		<category name="信息管理" key="02a29c36-575e-43e5-8034-29bd50b71fa5" />
	</category>
</category>
<category name="房地产金融" key="37883a2d-c7ee-40be-990b-baac751e1300" >
	<category name="房地产金融" key="cf9b894e-d0aa-4ce0-9b0e-cf9db58def53" >
	</category>
</category>
<category name="物业开发" key="cf9b894e-d0aa-4ce0-9b0e-cf9db58def53" >
	<category name="住宅开发" key="a711bcc5-29d5-4434-ad62-53d42c97344a" >
		<category name="土地获取" key="67f1526e-5bec-4f18-83af-76815bfe6d85" />
		<category name="产品定义" key="6f344ffd-f038-4b56-a08b-ba79beca3dae" />
		<category name="项目实施" key="9101df19-29b7-48be-bf7f-d99c330492fa" />
		<category name="现场销售" key="1d85369b-e4bd-448b-9abd-fa6d8bcb6e5e" />
		<category name="签约" key="e81efcf1-ae2c-49c0-82a0-832cc932421b" />
		<category name="产权办理" key="937faeae-6f98-483e-903c-f942e3be4e35" />
		<category name="房屋交付" key="72978243-9368-4817-8e28-d69d73755189" />
		<category name="集中整改" key="f9cace77-ae91-44b5-860b-d145662078d8" />
		<category name="投诉管理" key="0ca6c539-7e27-434d-a750-241954aca325" />
		<category name="物业服务" key="3f2d6226-e10b-479b-84af-a79894965b72" />
	</category>
	<category name="商业开发" key="19036052-27d3-45f2-8e82-5f8fe172d1f8" >
	</category>
</category>
<category name="商业运营" key="ef16b47e-ffeb-4078-8895-597bb7039f3e" >
	<category name="商场" key="529044bc-c752-4444-b1bc-23164bf8cd54" />
	<category name="写字楼" key="a4250518-a791-4f08-8c5f-9c63d2255652" />
	<category name="酒店" key="f78eb647-e6cc-41c0-9c1c-d341fa40ac2a" />
	<category name="高尔夫" key="19b65c8d-6770-4187-ad08-20c4a401ea0c" />
	<category name="商业配套" key="8e62703f-d252-4405-93a4-3f123e86db29" />
	<category name="养老地产" key="e578f8ec-67a5-4b02-b30c-f397ec7b5f3c" />
</category>
<category name="房地产建设" key="e22bbd4f-cebc-470b-ad9f-d85ebd3e3be7" >
	<category name="建设工程勘察" key="ef9e35ef-c00b-463c-b0d1-d64bc7a24c7a" />
	<category name="施工总承包" key="16628b2d-c20a-4526-8b85-178b18d12b1f" />
	<category name="建筑装饰装修" key="833034d6-57c1-44ef-a07e-fe2b9a0152dd" />
	<category name="园林工程" key="cdfd9ae9-1061-4bd2-b6db-77228f48ea5c" />
	<category name="智能建筑" key="8ee15211-41fa-45ca-a1d6-6b1b979c7623" />
</category>
<category name="房地产服务" key="3bf948bb-abb3-46e1-8465-fa18e7d3e3ce" >
	<category name="销售策划和代理" key="f557b496-9e33-436c-8a1f-afda422e08dd" />
	<category name="二手房交易" key="61bdb204-fa11-4e2d-bab6-39dbdf306cd7" />
	<category name="房地产咨询" key="233edcef-a086-4010-9e85-874767a8c22c" />
	<category name="物业咨询和服务" key="7e249360-58c7-4193-b439-630368c61b7f" />
</category>

<!--
  <category name="住宅开发销售" key="C6718357-882C-4336-ADC1-56DA18A9B93B">
    <category name="住宅土地获取" key="ZZKFXX.ZZTDHQ">
      <category name="土地获取" key="ZZKFXX.ZZTDHQ.TDHQ" />
    </category>
    <category name="住宅开发" key="ZZKFXX.ZZKF">
      <category name="产品定义" key="ZZKFXX.ZZKF.CPDY" />
      <category name="项目实施" key="ZZKFXX.ZZKF.XXSS" />
      <category name="现场销售" key="ZZKFXX.ZZKF.XCXX" />
      <category name="签约" key="ZZKFXX.ZZKF.QY" />
      <category name="房屋交付" key="ZZKFXX.ZZKF.FWJF" />
      <category name="产权办理" key="ZZKFXX.ZZKF.CQBL" />
      <category name="集中整改" key="ZZKFXX.ZZKF.JZZG" />
      <category name="投诉管理" key="ZZKFXX.ZZKF.TSGL" />
    </category>
    <category name="住宅建设" key="ZZKFXX.ZZJS">
    </category>
    <category name="住宅装饰" key="ZZKFXX.ZZZS">
    </category>
    <category name="住宅销售" key="ZZKFXX.ZZXS">
    </category>
    <category name="住宅物业服务" key="ZZKFXX.ZZWYFF">
    </category>
    <category name="住宅房地产金融" key="ZZKFXX.ZZFDCJR">
    </category>
    <category name="住宅综合管理" key="ZZKFXX.ZZZHGL">
    </category>
  </category>
  <category name="写字楼开发运营" key="9B06153D-3BA3-44F7-8221-4137263BAA12">

  </category>
  <category name="商业开发运营" key="16C64119-BC1D-42BE-A3DB-AB1C4BC2879D">

  </category>
  <category name="酒店开发运营" key="505E0623-1534-4B5F-93F8-682AA575A471">

  </category>
  <category name="养老地产运营" key="7E90B730-8218-4BBC-AD7C-11979A761F02">

  </category>
  <category name="高尔夫地产开发运营" key="E1CC4EAD-B4D6-4DC1-B56C-DC0FC6FA17ED">

  </category>
  <category name="综合体开发运营" key="ACAE9178-A8DB-404A-87A6-F2BB59063E5C">

  </category>
  -->

</dom>
'
DECLARE @idoc int

EXEC sp_xml_preparedocument @idoc OUTPUT, @xml

--警告：以下语句将清空现有分类和历史分类
TRUNCATE TABLE SC.Categories
UPDATE SC.Categories SET VersionEndTime = GETDATE(), Status = 3

--顶级分类（商业式态）

INSERT INTO SC.Categories(ID, Name,VersionStartTime,FullPath,[Level]) 
SELECT 形态Code AS ID,(商业模式 + '/' + 商业形态) AS Name, GETDATE() AS VersionStartTime,(商业模式 + '/' + 商业形态) AS FullPath ,0 AS [Level]
FROM OPENXML(@idoc,'/dom/category/category', 2)
WITH (商业模式 nvarchar(128) '../@name', 商业形态 nvarchar(128) '@name', 形态Code nvarchar(128) '@key')

--二级分类（服务包）
INSERT INTO SC.Categories(ID, Name, VersionStartTime,FullPath,ParentID,[Level]) 
SELECT ID,Name,GETDATE() AS VersionStartTime, (Parent1Name + '/' + Parent2Name + '/' + Name) AS FullPath , ParentID  ,1 AS [Level]
FROM OPENXML(@idoc,'/dom/category/category/category', 2)
WITH (ID nvarchar(128) '@key', Name nvarchar(128) '@name', AlterKey nvarchar(128) '@key', ParentID nvarchar(128) '../@key', Parent1Name nvarchar(128) '../@name', Parent2Name nvarchar(128) '../../@name')




RETURN 0
