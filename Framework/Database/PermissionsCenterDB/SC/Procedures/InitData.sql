/* 初始化示例数据 */
CREATE PROCEDURE [SC].[InitData]
AS
BEGIN
	exec SC.ClearAllData

	DECLARE @now DATETIME

	SET @now = GETDATE()

	DECLARE @adminID NVARCHAR(36)

	SET @adminID = LOWER(CAST(NEWID() AS NVARCHAR(36)))

	DECLARE @virtualRoot NVARCHAR(36)

	SET @virtualRoot = 'e588c4c6-4097-4979-94c2-9e2429989932'

	DECLARE @root NVARCHAR(36)

	SET @root = LOWER(CAST(NEWID() AS NVARCHAR(36)))

	DECLARE @roleID NVARCHAR(36)

	SET @roleID = LOWER(CAST(NEWID() AS NVARCHAR(36)))

	--组织信息
	--插入根组织机构
	INSERT [SC].[SchemaObject] ([ID], [VersionStartTime], [VersionEndTime], [Status], [Data], [SchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (@root, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, 
	N'<Object SchemaType="Organizations" ID="' + @root + '" Name="机构人员" DisplayName="机构人员" Comment="" CodeName="机构人员" DepartmentClass="0" DepartmentType="0" DepartmentRank="0" CustomsCode="" AllowAclInheritance="true" />', N'Organizations',
	@now, @adminID, N'管理员')

	--插入根组织机构对象快照
	INSERT [SC].[SchemaObjectSnapshot] ([ID], [VersionStartTime], [VersionEndTime], [Status], [CreateDate], [Name], [DisplayName], [CodeName], [AccountDisabled], [SearchContent], [RowUniqueID], [SchemaType], [CreatorID], [CreatorName], [Comment])
	VALUES (@root, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, @now, N'机构人员', N'机构人员', N'机构人员', 0, N'机构人员', LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Organizations', @adminID, N'管理员', N'')

	--插入根组织机构的快照
	INSERT [SC].[SchemaOrganizationSnapshot] ([ID], [VersionStartTime], [VersionEndTime], [Status], [CreateDate], [Name], [DisplayName], [CodeName], [AllowAclInheritance], [SearchContent], [RowUniqueID], [SchemaType], [CreatorID], [CreatorName], [Comment])
	VALUES (@root, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, @now, N'机构人员', N'机构人员', N'机构人员', 1, N'机构人员', LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Organizations', @adminID, N'管理员', N'')

	--管理员（人员信息）
	--插入管理员对象信息
	INSERT [SC].[SchemaObject] ([ID], [VersionStartTime], [VersionEndTime], [Status], [Data], [SchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (@adminID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1,
	N'<Object SchemaType="Users" ID="' + @adminID + '" Name="管理员" DisplayName="管理员" Comment="" CodeName="admin" LastName="管" FirstName="理员" OwnerID="' + @root + '" OwnerName="机构人员" PhotoKey="" PasswordNotRequired="False" DontExpirePassword="False" AccountInspires="" AccountExpires="" AccountDisabled="False" Mail="" Sip="" MP="" WP="" OtherMP="" Address="" CadreType="0" UserRank="10" Occupation="" CompanyName="" DepartmentName="机构人员" HREmployeePropertyCode="" HRRegularDate="" HRDimissionDate="" HRWorkCityCode="" HRWorkCity="" HRPersonalMobile="" HRVocationLevel="" HRLaborRelationCompanyCode="" />',
	N'Users', @now, @adminID, N'管理员')

	--插入管理员对象快照
	INSERT [SC].[SchemaObjectSnapshot] ([ID], [VersionStartTime], [VersionEndTime], [Status], [CreateDate], [Name], [DisplayName], [CodeName], [AccountDisabled], [SearchContent], [RowUniqueID], [SchemaType], [CreatorID], [CreatorName], [Comment])
	VALUES (@adminID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, @now, N'管理员', N'管理员', N'admin', 0, N'admin 管理员', LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Users', @adminID, N'管理员', N'')

	--插入管理员用户快照
	INSERT [SC].[SchemaUserSnapshot] ([ID], [VersionStartTime], [VersionEndTime], [Status], [CreateDate], [Name], [DisplayName], [CodeName], [FirstName], [LastName], [SearchContent], [RowUniqueID], [SchemaType], [Mail], [Sip], [MP], [WP], [Address], [CreatorID], [CreatorName], [OwnerID], [OwnerName], [AccountDisabled], [PasswordNotRequired], [DontExpirePassword], [AccountExpires], [AccountInspires], [Comment])
	VALUES (@adminID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, @now, N'管理员', N'管理员', N'admin', N'理员', N'管', N'admin 管理员', LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Users', N'', N'', N'', N'', N'', @adminID, N'管理员', @root, N'机构人员', 0, 0, 0, NULL, NULL, N'')

	--插入用户的密码
	INSERT [SC].[UserPassword] ([UserID], [PasswordType], [AlgorithmType], [Password])
	VALUES (@adminID, 'MCS.Authentication', 'MCS.MD5', 'B0-81-DB-E8-5E-1E-C3-FF-C3-D4-E7-D0-22-74-00-CD')

	--插入组织关系对象
	INSERT [SC].[SchemaRelationObjects] ([ParentID], [ObjectID], [VersionStartTime], [VersionEndTime], [Status], [IsDefault], [InnerSort], [FullPath], [GlobalSort], [Data], [SchemaType], [ParentSchemaType], [ChildSchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (@virtualRoot, @root, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, 1, 1, N'机构人员', N'000001',
	N'<Object SchemaType="RelationObjects" ParentID="' + @virtualRoot + '" ParentSchemaType="Organizations" InnerSort="1" ID="' + @root + '" ChildSchemaType="Organizations" Default="true" FullPath="机构人员" GlobalSort="000001" />', N'RelationObjects', N'Organizations', N'Organizations',
	@now, @adminID, '管理员')

	--插入组织关系对象快照信息
	INSERT [SC].[SchemaRelationObjectsSnapshot] ([ParentID], [ObjectID], [VersionStartTime], [VersionEndTime], [Status], [IsDefault], [InnerSort], [FullPath], [GlobalSort], [SchemaType], [SearchContent], [RowUniqueID], [ParentSchemaType], [ChildSchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (@virtualRoot, @root, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, 1, 1, N'机构人员', N'000001', N'RelationObjects', NULL, LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Organizations', N'Organizations', @now, NULL, NULL)

	--插入人与组织关系对象
	INSERT [SC].[SchemaRelationObjects] ([ParentID], [ObjectID], [VersionStartTime], [VersionEndTime], [Status], [IsDefault], [InnerSort], [FullPath], [GlobalSort], [Data], [SchemaType], [ParentSchemaType], [ChildSchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (@root, @adminID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, 1, 1, N'机构人员\管理员', N'000001000001',
	N'<Object SchemaType="RelationObjects" ParentID="' + @Root + '" ParentSchemaType="Organizations" InnerSort="1" ID="' + @adminID + '" ChildSchemaType="Users" Default="true" FullPath="机构人员\管理员" GlobalSort="000001000001" />', N'RelationObjects', N'Organizations', N'Users',
	@now, @adminID, '管理员')

	--插入人与组织关系对象快照信息
	INSERT [SC].[SchemaRelationObjectsSnapshot] ([ParentID], [ObjectID], [VersionStartTime], [VersionEndTime], [Status], [IsDefault], [InnerSort], [FullPath], [GlobalSort], [SchemaType], [SearchContent], [RowUniqueID], [ParentSchemaType], [ChildSchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (@root, @adminID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, 1, 1, N'机构人员\管理员', N'000001000001', N'RelationObjects', NULL, LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Organizations', N'Users', @now, NULL, NULL)

	--应用授权
	--插入应用对象
	INSERT [SC].[SchemaObject] ([ID], [VersionStartTime], [VersionEndTime], [Status], [Data], [SchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (N'11111111-1111-1111-1111-111111111111', @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1,
	N'<Object SchemaType="Applications" ID="11111111-1111-1111-1111-111111111111" Name="通用授权" DisplayName="通用授权" Comment="通用授权管理平台" CodeName="APP_ADMIN" ResourceLevel="" />',
	N'Applications', @now, NULL, NULL)

	--插入应用对象通用快照
	INSERT [SC].[SchemaObjectSnapshot] ([ID], [VersionStartTime], [VersionEndTime], [Status], [CreateDate], [Name], [DisplayName], [CodeName], [AccountDisabled], [SearchContent], [RowUniqueID], [SchemaType], [CreatorID], [CreatorName], [Comment])
	VALUES (N'11111111-1111-1111-1111-111111111111', @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, @now, N'通用授权', N'通用授权', N'通用授权', 0, N'通用授权', LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Applications', @adminID, N'管理员', N'通用授权管理平台')

	--插入应用对象快照
	INSERT [SC].[SchemaApplicationSnapshot] ([ID], [VersionStartTime], [VersionEndTime], [Status], [CreateDate], [Name], [DisplayName], [CodeName], [SearchContent], [RowUniqueID], [SchemaType], [CreatorID], [CreatorName], [Comment])
	VALUES (N'11111111-1111-1111-1111-111111111111', @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, @now, N'通用授权', N'通用授权', N'APP_ADMIN', N'APP_ADMIN 通用授权管理平台 通用授权', LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Applications', @adminID, '管理员', N'通用授权管理平台')

	--插入角色对象
	INSERT [SC].[SchemaObject] ([ID], [VersionStartTime], [VersionEndTime], [Status], [Data], [SchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (@roleID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, N'<Object SchemaType="Roles" ID="' + @roleID + '" Name="系统总管理员" DisplayName="系统总管理员" Comment="" CodeName="SYSTEM_MAX_ADMINISTRATOR" />',
	N'Roles', @now, @adminID, N'管理员')

	--插入角色对象通用快照
	INSERT [SC].[SchemaObjectSnapshot] ([ID], [VersionStartTime], [VersionEndTime], [Status], [CreateDate], [Name], [DisplayName], [CodeName], [AccountDisabled], [SearchContent], [RowUniqueID], [SchemaType], [CreatorID], [CreatorName], [Comment])
	VALUES (@roleID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, @now, N'系统总管理员', N'系统总管理员', N'系统总管理员', 0, N'SYSTEM_MAX_ADMINISTRATOR 系统总管理员', LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Roles', @adminID, N'管理员', N'')

	--插入角色对象快照
	INSERT [SC].[SchemaRoleSnapshot] ([ID], [VersionStartTime], [VersionEndTime], [Status], [CreateDate], [Name], [DisplayName], [CodeName], [SearchContent], [RowUniqueID], [SchemaType], [CreatorID], [CreatorName], [Comment])
	VALUES (@roleID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, @now, N'系统总管理员', N'系统总管理员', N'SYSTEM_MAX_ADMINISTRATOR', N'SYSTEM_MAX_ADMINISTRATOR 系统总管理员', LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Roles', @adminID, N'管理员', N'')

	--插入应用与角色之间的关系对象
	INSERT [SC].[SchemaMembers] ([ContainerID], [MemberID], [VersionStartTime], [VersionEndTime], [Status], [InnerSort], [Data], [SchemaType], [ContainerSchemaType], [MemberSchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (N'11111111-1111-1111-1111-111111111111', @roleID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, 0,
	N'<Object SchemaType="MemberRelations" ContainerID="11111111-1111-1111-1111-111111111111" ContainerSchemaType="Applications" InnerSort="0" ID="' + @roleID + '" MemberSchemaType="Roles" />',
	N'MemberRelations', N'Applications', N'Roles', @now, @adminID, N'管理员')

	--插入应用与角色之间的关系对象快照
	INSERT [SC].[SchemaMembersSnapshot] ([ContainerID], [MemberID], [VersionStartTime], [VersionEndTime], [Status], [InnerSort], [SchemaType], [SearchContent], [RowUniqueID], [ContainerSchemaType], [MemberSchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (N'11111111-1111-1111-1111-111111111111', @roleID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, 0, N'MemberRelations', NULL, LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Applications', N'Roles', @now, @adminID, N'管理员')

	--插入角色与用户之间的关系对象
	INSERT [SC].[SchemaMembers] ([ContainerID], [MemberID], [VersionStartTime], [VersionEndTime], [Status], [InnerSort], [Data], [SchemaType], [ContainerSchemaType], [MemberSchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (@roleID, @adminID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, 0,
	N'<Object SchemaType="MemberRelations" ContainerID="' + @roleID + '" ContainerSchemaType="Roles" InnerSort="0" ID="' + @adminID + '" MemberSchemaType="Users" />',
	N'MemberRelations', N'Roles', N'Users', @now, @adminID, N'管理员')

	--插入角色与用户之间的关系对象快照
	INSERT [SC].[SchemaMembersSnapshot] ([ContainerID], [MemberID], [VersionStartTime], [VersionEndTime], [Status], [InnerSort], [SchemaType], [SearchContent], [RowUniqueID], [ContainerSchemaType], [MemberSchemaType], [CreateDate], [CreatorID], [CreatorName])
	VALUES (@roleID, @adminID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, 0, N'MemberRelations', NULL, LOWER(CAST(NEWID() AS NVARCHAR(36))), N'Roles', N'Users', @now, @adminID, N'管理员')

	--用户和容器之间的快照
	INSERT [SC].[UserAndContainerSnapshot] ([UserID], [ContainerID], [VersionStartTime], [VersionEndTime], [Status], [UserSchemaType], [ContainerSchemaType])
	VALUES (@adminID, @roleID, @now, CAST(N'9999-09-09 00:00:00.000' AS DateTime), 1, N'Users', N'Roles')
END