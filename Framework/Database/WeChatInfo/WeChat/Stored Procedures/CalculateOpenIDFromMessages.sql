CREATE PROCEDURE [WeChat].[CalculateOpenIDFromMessages]	
AS

UPDATE F SET F.OpenID = T.FromOpenID FROM [WeChat].[Friends] AS F
INNER JOIN 
(
SELECT r.FakeID,r.NickName,r.AccountID,i.FromOpenID FROM [WeChat].[IncomeMessages] i
inner join [WeChat].[RecentMessages] r
ON i.SentTime=r.SentTime and i.ToOpenID=r.AccountID

) AS T ON F.FakeID=T.FakeID AND F.AccountID=T.AccountID