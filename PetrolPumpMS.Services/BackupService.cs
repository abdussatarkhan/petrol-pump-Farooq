using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.Services;

/// <summary>
/// Shells out to pg_dump (must be on PATH — it ships with the PostgreSQL
/// installer/client tools) using the same connection details as the app.
/// </summary>
public class BackupService : IBackupService
{
    private readonly IConfiguration _config;
    public BackupService(IConfiguration config) => _config = config;

    public async Task<ServiceResult<string>> BackupToFileAsync(string destinationFolder)
    {
        try
        {
            var connStr = _config.GetConnectionString("Default");
            if (string.IsNullOrWhiteSpace(connStr))
                return ServiceResult<string>.Fail("No connection string configured.");

            var builder = new NpgsqlConnectionStringBuilder(connStr);
            Directory.CreateDirectory(destinationFolder);
            var fileName = $"petrol_pump_backup_{DateTime.Now:yyyyMMdd_HHmmss}.backup";
            var fullPath = Path.Combine(destinationFolder, fileName);

            var psi = new ProcessStartInfo
            {
                FileName = "pg_dump",
                ArgumentList =
                {
                    "-h", builder.Host ?? "localhost",
                    "-p", (builder.Port == 0 ? 5432 : builder.Port).ToString(),
                    "-U", builder.Username ?? "postgres",
                    "-F", "c",
                    "-f", fullPath,
                    builder.Database ?? "petrol_pump_db"
                },
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            psi.Environment["PGPASSWORD"] = builder.Password ?? "";

            using var process = Process.Start(psi);
            if (process is null) return ServiceResult<string>.Fail("Could not start pg_dump. Is it installed and on PATH?");

            string stderr = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                return ServiceResult<string>.Fail($"pg_dump failed: {stderr}");

            return ServiceResult<string>.Ok(fullPath);
        }
        catch (Exception ex)
        {
            return ServiceResult<string>.Fail($"Backup failed: {ex.Message}");
        }
    }
}
