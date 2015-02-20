WITH TempQuery AS
(
    SELECT {0}, ROW_NUMBER() OVER (ORDER BY {3}) AS 'RowNumberForSplit'
	FROM {1}
	WHERE 1 = 1 {2}
	{4}
)
SELECT * 
FROM TempQuery 
WHERE RowNumberForSplit BETWEEN {5} AND {6};