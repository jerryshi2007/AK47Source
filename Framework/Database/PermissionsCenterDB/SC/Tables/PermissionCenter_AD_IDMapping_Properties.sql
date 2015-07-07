EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限中心ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'PermissionCenter_AD_IDMapping',
    @level2type = N'COLUMN',
    @level2name = N'SCObjectID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'AD对象GUID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'PermissionCenter_AD_IDMapping',
    @level2type = N'COLUMN',
    @level2name = N'ADObjectGuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最后同步时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'PermissionCenter_AD_IDMapping',
    @level2type = N'COLUMN',
    @level2name = N'LastSynchronizedVersionTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限中心和AD之间的ID映射表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'PermissionCenter_AD_IDMapping',
    @level2type = NULL,
    @level2name = NULL