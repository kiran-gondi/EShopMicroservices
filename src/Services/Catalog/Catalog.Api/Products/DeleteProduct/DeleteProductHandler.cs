namespace Catalog.Api.Products.DeleteProduct;
public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool isSuccess);

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty().WithMessage("Product ID is required");
    }
}
internal class DeleteProductCommandHandler(
  IDocumentSession session)
  : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
  public async Task<DeleteProductResult> Handle(DeleteProductCommand command, 
    CancellationToken cancellationToken)
  {
    bool isUpdateSuccessful;
    
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
