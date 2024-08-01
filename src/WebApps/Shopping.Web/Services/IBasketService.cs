namespace Shopping.Web.Services;

public interface IBasketService
{
  [Get("/basket-service/basket/{userName}")]
  Task<GetBasketResponse> GetBasket(string userName);
  [Post("/basket-service/basket")]
  Task<StoreBasketResponse> StoreBasket(StoreBasketRequest request);
  [Delete("/basket-service/basket/{userName}")]
  Task<DeleteBasketResponse> DeleteBasket(string userName);
  [Post("/basket-service/basket/checkout")]
  Task<CheckoutBasketResponse> CheckoutBasket(CheckoutBasketRequest request);

    //C# 8.0 New Feature i.e. Default Interface Methods
    public async Task<ShoppingCartModel> LoadUserBasket()
    {
        //Get Basket If not Exists Create New Basket with Default Logged in User Name "swn"
        var userName = "swn";
        ShoppingCartModel basket;

        try
        {
            var getBasketResponse = await GetBasket(userName);
            basket = getBasketResponse.Cart;
        }
        catch (ApiException apiException) when (apiException.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
                basket = new ShoppingCartModel()
                {
                    UserName = userName,
                    Items = []
                };
        }

        return basket;
    }

}
