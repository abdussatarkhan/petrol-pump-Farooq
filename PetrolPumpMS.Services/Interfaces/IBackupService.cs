namespace PetrolPumpMS.Services.Interfaces;

public interface IBackupService
{
    /// <summary>Runs pg_dump against the configured connection and writes a .backup file. Returns the output path on success.</summary>
    Task<ServiceResult<string>> BackupToFileAsync(string destinationFolder);
}
