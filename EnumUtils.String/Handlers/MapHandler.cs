using System;
using System.Collections.Concurrent;

namespace EnumUtils.String.Handlers
{
    internal sealed class MapHandler<TSource, TResult>
    where TSource : notnull
    where TResult : notnull
    {
        private readonly ConcurrentDictionary<TSource, TResult> _toResult;
        private readonly ConcurrentDictionary<TResult, TSource> _toSource;

        public MapHandler()
        {
            _toSource = new ConcurrentDictionary<TResult, TSource>();
            _toResult = new ConcurrentDictionary<TSource, TResult>();
        }

        public bool TryGet(TSource source, out TResult? result)
        {
            if (source == null)
            {
                throw new NullReferenceException(nameof(source));
            }

            return _toResult.TryGetValue(source, out result);
        }

        public bool TryGet(TResult result, out TSource? source)
        {
            if (result == null)
            {
                throw new NullReferenceException(nameof(result));
            }

            return _toSource.TryGetValue(result, out source);
        }

        public bool TryAdd(TSource source, TResult result)
        {
            if (source == null)
            {
                throw new NullReferenceException(nameof(source));
            }

            if (result == null)
            {
                throw new NullReferenceException(nameof(result));
            }

            return _toResult.TryAdd(source, result) && _toSource.TryAdd(result, source);
        }
    }
}
