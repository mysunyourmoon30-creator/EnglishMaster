USE [master];
GO

IF DB_ID(N'EnglishMasterInternal') IS NULL
BEGIN
    CREATE DATABASE [EnglishMasterInternal];
END
GO

IF SUSER_ID(N'englishmaster_app') IS NULL
BEGIN
    CREATE LOGIN [englishmaster_app]
        WITH PASSWORD = N'EnglishMaster_Local_123!',
        CHECK_POLICY = ON,
        CHECK_EXPIRATION = OFF;
END
GO

USE [EnglishMasterInternal];
GO

IF USER_ID(N'englishmaster_app') IS NULL
BEGIN
    CREATE USER [englishmaster_app] FOR LOGIN [englishmaster_app];
END
GO

ALTER ROLE [db_owner] ADD MEMBER [englishmaster_app];
GO
