/*创建SchemaUserSnapshot的全文索引*/
CREATE FULLTEXT INDEX ON [SC].[SchemaUserSnapshot]
    ([SearchContent] LANGUAGE 2052)
    KEY INDEX [IX_SchemaUser_RowID]
    ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO
GO

