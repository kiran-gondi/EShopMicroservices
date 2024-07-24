using Discount.Grpc;

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

public class StoreBasketCommandHandler(IBasketRepository basketRepository, 
  DiscountProtoService.DiscountProtoServiceClient discountProto) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
  public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
  {
    await DeductDiscount(discountProto, command, cancellationToken);
    //Store basket in DB (use Marten upsert - if exits = update, if not = create) & Update cache (Redis Distributed Cache)
    await basketRepository.StoreBasket(command.Cart, cancellationToken);

    return new StoreBasketResult(command.Cart.UserName);
  }

  private static async Task DeductDiscount(DiscountProtoService.DiscountProtoServiceClient discountProto, StoreBasketCommand command, CancellationToken cancellationToken)
  {
    //TODO: Communicate with Discount.Grpc and calculate latest prices of products
    foreach (var cartItem in command.Cart.Items)
    {
      var coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = cartItem.ProductName },
        cancellationToken: cancellationToken);
      cartItem.Price -= coupon.Amount;
    }
  }
}
