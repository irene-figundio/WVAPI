-- Migration Script to add Soft Delete fields
DECLARE @TableName NVARCHAR(256)
DECLARE @Sql NVARCHAR(MAX)

DECLARE TableCursor CURSOR FOR
SELECT t.name
FROM sys.tables t
WHERE t.type = 'U'

OPEN TableCursor
FETCH NEXT FROM TableCursor INTO @TableName

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@TableName) AND name = 'IsDeleted')
    BEGIN
        SET @Sql = 'ALTER TABLE [' + @TableName + '] ADD IsDeleted BIT NULL;'
        EXEC sp_executesql @Sql
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@TableName) AND name = 'DeletionDate')
    BEGIN
        SET @Sql = 'ALTER TABLE [' + @TableName + '] ADD DeletionDate DATETIME NULL;'
        EXEC sp_executesql @Sql
    END

    FETCH NEXT FROM TableCursor INTO @TableName
END

CLOSE TableCursor
DEALLOCATE TableCursor
