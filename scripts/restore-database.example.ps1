<#
Example only. Do not store real passwords or production secrets in this file.
Restore to staging or an isolated recovery database before production.
#>

param(
    [string]$SqlServer = "<sql-server>",
    [string]$Database = "EnglishMaster_RestoreTest",
    [string]$BackupPath = "<backup-file.bak>"
)

Write-Host "Restoring SQL Server backup $BackupPath to $Database"
sqlcmd -S $SqlServer -Q "RESTORE DATABASE [$Database] FROM DISK = N'$BackupPath' WITH REPLACE, RECOVERY;"

Write-Host "Restore example completed. Start the app against this database and verify health, login, admin pages, public pages, media, and publishing artifacts."
