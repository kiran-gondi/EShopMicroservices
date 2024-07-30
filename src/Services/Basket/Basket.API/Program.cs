using BuildingBlocks.Exceptions.Handler;
using BuildingBlocksMessaging.MassTransit;
using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.

//Application Services
var assembly = typeof(Program).Assembly;
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
  config.RegisterServicesFromAssembly(assembly);
  config.AddOpenBehavior(typeof(ValidationBehavior<,>));
  config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

//Data Services
builder.Services.AddMarten(opts =>
{
  opts.Connection(builder.Configuration.GetConnectionString("Database")!);
  opts.Schema.For<ShoppingCart>().Identity(x => x.UserName); //Mark UserName as the Document Identity OR Identity Key
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
//builder.Services.AddScoped<IBasketRepository, CachedBasketRepository>();

//Manual way of adding the decorator class
//builder.Services.AddScoped<IBasketRepository>(provider => {  
//  var basketRepository = provider.GetRequiredService<IBasketRepository>();
//  return new CachedBasketRepository(basketRepository, provider.GetRequiredService<IDistributedCache>());
//});

builder.Services.AddStackExchangeRedisCache(options => {
  options.Configuration = builder.Configuration.GetConnectionString("Redis");
  //options.InstanceName = "Basket";
});

//Grpc Services
//TODO: Add Grpc Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options => {
  options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
})
  .ConfigurePrimaryHttpMessageHandler(() =>
  {
    var handler = new HttpClientHandler
    {
      ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    return handler;
  });

//Asynchronous Communication Services [THIS IS THE PUBLISHER SIDE]
builder.Services.AddMessageBroker(builder.Configuration);

//Cross-Cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
  .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
  .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

//Configure the HTTP request pipeline.
app.MapCarter();
app.UseExceptionHandler(options => { });
app.UseHealthChecks("/health", 
  new HealthCheckOptions
  {
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
  });

app.Run();
