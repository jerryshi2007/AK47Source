--生成管理范围对象和容器的快照

CREATE PROCEDURE [SC].[GenerateItemAndContainerSnapshot]
AS
BEGIN
	INSERT INTO SC.ItemAndContainerSnapshot(ContainerID, ItemID, ContainerSchemaType, ItemSchemaType, VersionStartTime, VersionEndTime, [Status])
	SELECT ContainerID, MemberID, ContainerSchemaType, MemberSchemaType, VersionStartTime, VersionEndTime, [Status]
	FROM SC.SchemaMembers
	WHERE ContainerSchemaType = 'AUAdminScopes'
		AND MemberSchemaType IN
		(SELECT SchemaType
		FROM SC.AUAdminScopeTypes
		)
END
