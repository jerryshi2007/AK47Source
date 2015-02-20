﻿CREATE TABLE [dbo].[FUNC_SET_TO_FUNCS_DELETE] (
    [FUNC_SET_ID] NVARCHAR (36) NOT NULL,
    [FUNC_ID]     NVARCHAR (36) NOT NULL,
    [SORT_ID]     INT           NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_FUNC_SET_TO_FUNCS_DELETE]
    ON [dbo].[FUNC_SET_TO_FUNCS_DELETE]([FUNC_SET_ID] ASC, [FUNC_ID] ASC) WITH (FILLFACTOR = 50, PAD_INDEX = ON);

