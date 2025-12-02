-- Migration to add Description column to pp_roles table
-- Run this SQL script if the column doesn't exist

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[pp_roles]') AND name = 'Description')
BEGIN
    ALTER TABLE [dbo].[pp_roles]
    ADD [Description] nvarchar(max) NULL;
    
    PRINT 'Description column added to pp_roles table';
END
ELSE
BEGIN
    PRINT 'Description column already exists in pp_roles table';
END

