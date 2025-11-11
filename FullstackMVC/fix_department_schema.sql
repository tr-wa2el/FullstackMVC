-- SQL Script to fix Department and Instructor table schema
-- Run this manually in your SQL Server database

USE [YourDatabaseName]  -- Replace with your actual database name
GO

-- ====================================
-- FIX DEPARTMENT TABLE
-- ====================================

-- Add the Description column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND name = 'Description')
BEGIN
    ALTER TABLE [dbo].[Departments] 
    ADD [Description] NVARCHAR(500) NULL
    PRINT 'Added Description column to Departments table'
END
ELSE
BEGIN
    PRINT 'Description column already exists in Departments table'
END
GO

-- Check if the old MangerName column exists and ManagerName doesn't
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND name = 'MangerName')
   AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND name = 'ManagerName')
BEGIN
    -- Rename the column from MangerName to ManagerName
    EXEC sp_rename 'Departments.MangerName', 'ManagerName', 'COLUMN'
    PRINT 'Renamed MangerName to ManagerName in Departments table'
END
ELSE
BEGIN
    PRINT 'ManagerName column already exists or MangerName does not exist in Departments table'
END
GO

-- ====================================
-- FIX INSTRUCTOR TABLE
-- ====================================

-- Add the Image column to Instructors table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Instructors]') AND name = 'Image')
BEGIN
    ALTER TABLE [dbo].[Instructors] 
    ADD [Image] NVARCHAR(500) NULL
    PRINT 'Added Image column to Instructors table'
END
ELSE
BEGIN
    PRINT 'Image column already exists in Instructors table'
END
GO

-- ====================================
-- VERIFY CHANGES
-- ====================================

PRINT ''
PRINT '======================================'
PRINT 'DEPARTMENTS TABLE SCHEMA:'
PRINT '======================================'
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Departments'
ORDER BY ORDINAL_POSITION
GO

PRINT ''
PRINT '======================================'
PRINT 'INSTRUCTORS TABLE SCHEMA:'
PRINT '======================================'
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Instructors'
ORDER BY ORDINAL_POSITION
GO

PRINT ''
PRINT '======================================'
PRINT 'SCHEMA UPDATE COMPLETED SUCCESSFULLY!'
PRINT '======================================'
GO