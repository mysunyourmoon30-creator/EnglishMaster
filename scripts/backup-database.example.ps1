<#
Example only. Do not store real passwords or production secrets in this file.
Run with a connection method approved for your environment.
#>

param(
    [string]$SqlServer = "<sql-server>",
    [string]$Database = "EnglishMaster",
    [string]$BackupDirectory = "<backup-directory>"
)

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$backupPath = Join-Path $BackupDirectory "$Database-$timestamp.bak"

Write-Host "Creating SQL Server backup for $Database at $backupPath"
sqlcmd -S $SqlServer -Q "BACKUP DATABASE [$Database] TO DISK = N'$backupPath' WITH CHECKSUM, COMPRESSION;"
sqlcmd -S $SqlServer -Q "RESTORE VERIFYONLY FROM DISK = N'$backupPath' WITH CHECKSUM;"

Write-Host "Backup example completed. Verify logs and store the file safely."
