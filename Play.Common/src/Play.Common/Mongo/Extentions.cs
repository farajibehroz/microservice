using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Repositories;
using Play.Common.Settings;

namespace Play.Common.Mongo
{
    public static class Extentions
    {
        public static IServiceCollection AddMongo(this IServiceCollection serviceCollection)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

            serviceCollection.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                ServerSetting serverSetting = configuration.GetSection(nameof(ServerSetting))
                                                           .Get<ServerSetting>();
                MongoDbSetting mongoDbSettings = configuration.GetSection(nameof(MongoDbSetting)).Get<MongoDbSetting>();
                var mongoDbClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoDbClient.GetDatabase(serverSetting.ServiceName);
            });
            return serviceCollection;
        }

        public static IServiceCollection AddMongoDbRepository<TEntity>(this IServiceCollection serviceCollection, string collectionName) where TEntity : IEntity
        {
            serviceCollection.AddSingleton<IRepository<TEntity>>(x =>
            {
                var mongoDb = x.GetService<IMongoDatabase>();
                return new MongoRepository<TEntity>(mongoDb, collectionName);
            });
            return serviceCollection;
        }

    }

}