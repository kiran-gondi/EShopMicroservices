namespace Catalog.Api.Products.DeleteProduct;
public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool isSuccess);

internal class DeleteProductCommandHandler(
  IDocumentSession session, ILogger<DeleteProductCommandHandler> logger)
  : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
  public async Task<DeleteProductResult> Handle(DeleteProductCommand command, 
    CancellationToken cancellationToken)
  {
    bool isUpdateSuccessful;
    logger.LogInformation("DeleteProductCommandHandler.Handle called with {@Command}",
    command);

    /*var product = await session.LoadAsync<Product>(command.Id, cancellationToken);

    if (product is null)
    {
      throw new ProductNotFoundException();
    }

    session.Delete(product); //This could be tried later
      */
    session.Delete<Product>(command.Id);

    await session.SaveChangesAsync(cancellationToken);

    isUpdateSuccessful = true;
    return new DeleteProductResult(isUpdateSuccessful);
  }
}
