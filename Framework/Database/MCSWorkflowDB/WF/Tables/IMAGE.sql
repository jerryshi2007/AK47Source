CREATE TABLE [WF].[IMAGE] (
    [ID]            NVARCHAR (36)  NOT NULL,
    [RESOURCE_ID]   NVARCHAR (36)  NULL,
    [CLASS]         NVARCHAR (64)  NULL,
    [WIDTH]         INT            CONSTRAINT [DF_IMAGE_WIDTH] DEFAULT ((0)) NOT NULL,
    [HEIGHT]        INT            CONSTRAINT [DF_IMAGE_HEIGHT] DEFAULT ((0)) NOT NULL,
    [NAME]          NVARCHAR (255) NOT NULL,
    [ORIGINAL_NAME] NVARCHAR (255) NULL,
    [NEW_NAME]      NVARCHAR (255) NULL,
    [SIZE]          BIGINT         CONSTRAINT [DF_IMAGE_SIZE] DEFAULT ((0)) NOT NULL,
    [CREATOR_ID]    NVARCHAR (36)  NULL,
    [CREATOR_NAME]  NVARCHAR (64)  NULL,
    [CREATE_TIME]   DATETIME       CONSTRAINT [DF_IMAGE_CREATE_TIME] DEFAULT (getdate()) NULL,
    [UPDATE_TIME]   DATETIME       CONSTRAINT [DF_IMAGE_UPDATE_TIME] DEFAULT (getdate()) NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_IMAGE] PRIMARY KEY CLUSTERED ([ID] ASC)
);

GO

CREATE INDEX [IX_IMAGE_TENANT_CODE] ON [WF].[IMAGE] ([TENANT_CODE])

GO
CREATE NONCLUSTERED INDEX [IDX_IMAGE_RESOURCE_ID]
    ON [WF].[IMAGE]([RESOURCE_ID] ASC);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'上传图片表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'唯一ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'拥有者的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'RESOURCE_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'类别。一个拥有者可以有不同类别的图片',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'CLASS'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'宽',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'WIDTH'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'高',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'HEIGHT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'图片名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'原始文件名',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'ORIGINAL_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'新文件名',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'NEW_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'文件大小',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'SIZE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'CREATOR_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'CREATOR_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'CREATE_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'更新时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'IMAGE',
    @level2type = N'COLUMN',
    @level2name = N'UPDATE_TIME'