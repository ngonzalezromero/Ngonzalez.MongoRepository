using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Threading.Tasks;


namespace Ngonzalez.MongoRepository
{

    public class MongoRepository<T> : IMongoRepository<T> where T : class, new()
    {
        private IMongoCollection<T> _collection;

        public MongoRepository()
        {

        }
        public void Configure(string connection, string databaseName)
        {
            var mongoCollection = typeof(T).Name;
            var client = new MongoClient(connection);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<T>(mongoCollection);
        }

        public Task Insert(IEnumerable<T> list)
        {
            return _collection.InsertManyAsync(list);
        }

        public Task Insert(T entity)
        {
            return _collection.InsertOneAsync(entity);
        }

        public Task Delete(string field, string value)
        {
            var filter = Builders<T>.Filter.Eq(field, value);
            return _collection.DeleteManyAsync(filter);
        }

        public Task<List<T>> Select(Func<T, bool> func)
        {
            return _collection.AsQueryable().ToAsyncEnumerable().Where(func).ToList();
        }

        public Task<List<T>> SelectLast(Func<T, bool> func, Func<T, object> param, int take)
        {
            return _collection.AsQueryable().ToAsyncEnumerable().Where(func).OrderByDescending(param).Take(take).ToList();
        }

        public Task<T> Single(Func<T, bool> expression)
        {
            return _collection.AsQueryable().ToAsyncEnumerable().Where(expression).SingleOrDefault();
        }

        public Task<List<T>> All()
        {
            return _collection.AsQueryable().ToListAsync();
        }

    }
}
