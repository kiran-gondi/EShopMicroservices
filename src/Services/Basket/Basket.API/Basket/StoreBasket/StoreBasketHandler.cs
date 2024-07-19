namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
public record StoreBasketResult(string UserName);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
      RuleFor(x => x.Cart).NotNull().WithMessage("Cart can not be null");
      RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class StoreBasketCommandHandler(IBasketRepository basketRepository) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
  public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
  {
    ShoppingCart cart = command.Cart;
    //TODO: store basket in DB (user Marten upsert - if exits = update, if not = create)
    //TODO: update cache (Redis Distributed Cache)
    await basketRepository.StoreBasket(command.Cart, cancellationToken);

    return new StoreBasketResult(command.Cart.UserName);
  }
}
