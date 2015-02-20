﻿CREATE TABLE [WF].[GROUP_USERS] (
    [GROUP_ID]  NVARCHAR (36) NOT NULL,
    [USER_ID]   NVARCHAR (36) NOT NULL,
    [USER_NAME] NVARCHAR (64) NOT NULL,
    CONSTRAINT [PK_WF.GROUP_USERS] PRIMARY KEY CLUSTERED ([USER_ID] ASC, [GROUP_ID] ASC)
);

