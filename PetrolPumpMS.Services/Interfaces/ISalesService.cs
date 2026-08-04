using PetrolPumpMS.Models;

namespace PetrolPumpMS.Services.Interfaces;

public interface ISalesService
{
    Task<List<Sale>> GetSalesAsync(int limit = 200, DateTime? dateFrom = null, DateTime? dateTo = null);

    /// <summary>
    /// Records a sale, updates the nozzle's last reading, deducts sold quantity
    /// from the connected tank, and (if payment mode is Credit) increases the
    /// customer's outstanding balance — all in a single transaction.
    /// </summary>
    Task<ServiceResult> RecordSaleAsync(
        DateTime date, TimeSpan time, int nozzleId, int? employeeId,
        decimal closingReading, PaymentMode paymentMode, int? customerId);
}
