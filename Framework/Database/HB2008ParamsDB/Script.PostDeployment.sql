/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
IF NOT EXISTS(SELECT TOP 1 * FROM [dbo].[SWITCHES] WHERE [SWITCH_NAME] = 'AllowSignIn')
	INSERT [dbo].[SWITCHES] ([SWITCH_NAME], [SWITCH_VALUE], [DESCRIPTION]) VALUES('AllowSignIn', '0', '现在是系统维护期，禁止登录')