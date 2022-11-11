select * from [dbo].[Table]

UPDATE [dbo].[Table]
SET Login = 'updated'
FROM
(SELECT * FROM [dbo].[Table] WHERE ID='2') AS Selected
WHERE [dbo].[Table].Id = Selected.Id