using IMS.CoreBusiness;

namespace IMS.UseCases.Reports.Interfaces
{
    public interface ISearchProductTransactionUseCase
    {
        Task<IEnumerable<ProductTransaction>> ExecuteAsync(string prodName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? transactionType);
    }
}