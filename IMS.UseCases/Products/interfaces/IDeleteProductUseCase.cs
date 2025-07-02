namespace IMS.UseCases.Products.interfaces
{
    public interface IDeleteProductUseCase
    {
        Task ExecuteAsync(int ProductId);
    }
}