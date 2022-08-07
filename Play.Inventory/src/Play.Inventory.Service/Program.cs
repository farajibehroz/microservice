using Play.Common.Mongo;
using Play.Common.MassTransit;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMongo()
                .AddMongoDbRepository<InventoryItem>("inventoryitems")
                .AddMongoDbRepository<CatalogItem>("catalogitems")
                .UseMassTransitWithRabbitmq();

AddHttpClient(builder);



builder.Services.AddControllers(x =>
{
    x.SuppressAsyncSuffixInActionNames = false;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



void AddHttpClient(WebApplicationBuilder builder)
{
    var jitter = new Random();
    builder.Services.AddHttpClient<CatalogClient>(options =>
    {
        options.BaseAddress = new Uri("https://localhost:7055");
    })
        .AddTransientHttpErrorPolicy(buil => buil.Or<TimeoutRejectedException>().WaitAndRetryAsync(5,
         retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromSeconds(jitter.Next(1, 100)),
         (httpResponseMessage, timespan, context) =>
         {
             var serviceProvider = builder.Services.BuildServiceProvider();
             serviceProvider.GetService<ILogger<CatalogClient>>()
             .LogWarning($"Delaying for {timespan.TotalSeconds} seconds then retry {context} ");
         }))

        .AddTransientHttpErrorPolicy(buil => buil.Or<TimeoutRejectedException>().CircuitBreakerAsync(3,
        TimeSpan.FromSeconds(15),
        (outcome, timespan) => {
            var serviceProvider = builder.Services.BuildServiceProvider();
            serviceProvider.GetService<ILogger<CatalogClient>>()
            .LogWarning($"Opening the circuite for {timespan.TotalSeconds} seconds ");
        },
        () => {
            var serviceProvider = builder.Services.BuildServiceProvider();
            serviceProvider.GetService<ILogger<CatalogClient>>()
            .LogWarning($"The circuite is reseting ... ");
        }))

        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}