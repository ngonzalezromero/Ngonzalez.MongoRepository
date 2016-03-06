using System.Collections.Generic;

namespace Ngonzalez.MongoRepository
{
    public sealed class PagingResult<T>
    {
        public List<T> List { get; set; }
        public int Count { get; set; }

    }
}