CREATE TABLE [WF].[ACL] (
    [RESOURCE_ID] NVARCHAR (36) NOT NULL,
    [OBJECT_ID]   NVARCHAR (36) NOT NULL,
    [OBJECT_TYPE] NVARCHAR (32) NULL,
    [OBJECT_NAME] NVARCHAR (64) NULL,
    [SOURCE]      NVARCHAR (64) NULL,
	[TENANT_CODE] NVARCHAR(36) DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_ACL] PRIMARY KEY CLUSTERED ([RESOURCE_ID] ASC, [OBJECT_ID] ASC) NULL
);

GO

CREATE INDEX [IX_ACL_TENANT_CODE] ON [WF].[ACL] ([TENANT_CODE])

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'资源可访问的ACL', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'资源的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACL', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'被授权对象的ID，通常是人的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACL', @level2type = N'COLUMN', @level2name = N'OBJECT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'被授权对象的类型，机构、人、角色', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACL', @level2type = N'COLUMN', @level2name = N'OBJECT_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对象的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACL', @level2type = N'COLUMN', @level2name = N'OBJECT_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ACL的数据来源、例如工作流', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACL', @level2type = N'COLUMN', @level2name = N'SOURCE';

