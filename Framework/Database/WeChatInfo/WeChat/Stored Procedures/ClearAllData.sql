CREATE PROCEDURE [WeChat].[ClearAllData]
AS
BEGIN
	TRUNCATE TABLE WeChat.Groups
	TRUNCATE TABLE WeChat.Friends
	TRUNCATE TABLE WeChat.IncomeMessages
	TRUNCATE TABLE WeChat.IncomeMessagesHistory
	TRUNCATE TABLE WeChat.RecentMessages
	TRUNCATE TABLE WeChat.RecentMessagesHistory
END
