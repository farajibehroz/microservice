using MassTransit;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.Mongo;
using Play.Common.Repositories;
using Play.Common.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMongo()
                .AddMongoDbRepository<Item>("Items")
                .UseMassTransitWithRabbitmq();



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
