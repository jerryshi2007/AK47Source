CREATE TABLE [dbo].[OBJECT_EXT_PROPERTIES] (
    [OBJECT_ID]      NVARCHAR (36)  NOT NULL,
    [PROPERTY_NAME]  NVARCHAR (64)  NOT NULL,
    [PROPERTY_TYPE]  NVARCHAR (64)  NOT NULL,
    [PROPERTY_VALUE] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_OBJECT_EXT_PROPERTIES] PRIMARY KEY CLUSTERED ([OBJECT_ID] ASC, [PROPERTY_NAME] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对象的ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OBJECT_EXT_PROPERTIES', @level2type = N'COLUMN', @level2name = N'OBJECT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'属性的名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OBJECT_EXT_PROPERTIES', @level2type = N'COLUMN', @level2name = N'PROPERTY_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'属性的类型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OBJECT_EXT_PROPERTIES', @level2type = N'COLUMN', @level2name = N'PROPERTY_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'属性值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OBJECT_EXT_PROPERTIES', @level2type = N'COLUMN', @level2name = N'PROPERTY_VALUE';

