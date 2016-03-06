using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace Ngonzalez.MongoRepository
{
    public static class AsyncCursorExtensions
    {
        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IAsyncCursorSource<T> asyncCursorSource) =>
            new AsyncEnumerableAdapter<T>(asyncCursorSource);

        private class AsyncEnumerableAdapter<T> : IAsyncEnumerable<T>
        {
            private readonly IAsyncCursorSource<T> _asyncCursorSource;

            public AsyncEnumerableAdapter(IAsyncCursorSource<T> asyncCursorSource)
            {
                _asyncCursorSource = asyncCursorSource;
            }

            public IAsyncEnumerator<T> GetEnumerator() =>
                new AsyncEnumeratorAdapter<T>(_asyncCursorSource);
        }

        private class AsyncEnumeratorAdapter<T> : IAsyncEnumerator<T>
        {
            private readonly IAsyncCursorSource<T> _asyncCursorSource;
            private IAsyncCursor<T> _asyncCursor;
            private IEnumerator<T> _batchEnumerator;

            public T Current => _batchEnumerator.Current;

            public AsyncEnumeratorAdapter(IAsyncCursorSource<T> asyncCursorSource)
            {
                _asyncCursorSource = asyncCursorSource;
            }

            public async Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                if (_asyncCursor == null)
                {
                    _asyncCursor = await _asyncCursorSource.ToCursorAsync(cancellationToken);
                }
                if (_batchEnumerator != null && _batchEnumerator.MoveNext())
                {
                    return true;
                }
                if (_asyncCursor != null && await _asyncCursor.MoveNextAsync(cancellationToken))
                {
                    _batchEnumerator?.Dispose();
                    _batchEnumerator = _asyncCursor.Current.GetEnumerator();
                    return _batchEnumerator.MoveNext();
                }
                return false;
            }

            public void Dispose()
            {
                _asyncCursor?.Dispose();
                _asyncCursor = null;
            }
        }
    }
}
