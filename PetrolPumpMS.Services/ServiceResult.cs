namespace PetrolPumpMS.Services;

/// <summary>
/// Uniform outcome type for service-layer operations so ViewModels can show
/// inline validation/error messages instead of throwing across layers.
/// </summary>
public class ServiceResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }

    public static ServiceResult Ok() => new() { Success = true };
    public static ServiceResult Fail(string error) => new() { Success = false, Error = error };
}

public class ServiceResult<T>
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public T? Data { get; init; }

    public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
    public static ServiceResult<T> Fail(string error) => new() { Success = false, Error = error };
}
