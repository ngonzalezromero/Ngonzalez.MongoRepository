using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ngonzalez.MongoRepository
{
    public interface IMongoRepository<T> where T : class, new()
    {
        void Configure(string connection, string databaseName);
        Task Insert(IEnumerable<T> list);
        Task Insert(T entity);
        Task<List<T>> Select(Func<T, bool> func);
        Task<List<T>> SelectLast(Func<T, bool> func, Func<T, object> param, int take);
        Task<T> Single(Func<T, bool> expression);
        Task<List<T>> All();
        Task Delete(string field, string value);
        Task<PagingResult<T>> Paging(Func<T, bool> condition, Func<T, object> order, bool orderDescending, int pageIndex, int pageSize);
    }
}