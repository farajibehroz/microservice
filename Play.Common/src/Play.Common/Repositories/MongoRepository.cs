using System.Linq.Expressions;
using MongoDB.Driver;

namespace Play.Common.Repositories
{
    public class MongoRepository<TEntity> : IRepository<TEntity> where TEntity:IEntity
    {
        //var mongoClient = new MongoClient("mongodb://localhost:27017");
        //var dataBase = mongoClient.GetDatabase("Catalog");

        //private readonly string _collectionName = "items";
        private readonly IMongoCollection<TEntity> _dbCollection;
        private readonly FilterDefinitionBuilder<TEntity> _filterBuilder = Builders<TEntity>.Filter;

        public MongoRepository(IMongoDatabase database,string collectionName)
        {          
            _dbCollection = database.GetCollection<TEntity>(collectionName);
        }


        public async Task<IReadOnlyCollection<TEntity>> GetAllAsync()
        {
            return await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
        }


        public async Task<TEntity> GetAsync(Guid id)
        {
            FilterDefinition<TEntity> filterDefinition = _filterBuilder.Eq(x => x.Id, id);
            return await _dbCollection.Find(filterDefinition).FirstOrDefaultAsync();
        }


        public async Task CreateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            await _dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            FilterDefinition<TEntity> filterDefinition = _filterBuilder.Eq(existingItem => existingItem.Id, entity.Id);
            await _dbCollection.ReplaceOneAsync(filterDefinition, entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            FilterDefinition<TEntity> filterDefinition = _filterBuilder.Eq(existingItem => existingItem.Id, id);
            await _dbCollection.DeleteOneAsync(filterDefinition);
        }

        public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter)
        {
          return await _dbCollection.Find(filter).ToListAsync();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _dbCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
