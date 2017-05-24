﻿// The MIT License (MIT)
// 
// Copyright (c) 2015-2017 Rasmus Mikkelsen
// Copyright (c) 2015-2017 eBay Software Foundation
// https://github.com/eventflow/EventFlow
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EventFlow.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IReadOnlyCollection<T>> Partition<T>(this IEnumerable<T> items, int partitionSize)
        {
            return new PartitionHelper<T>(items, partitionSize);
        }

        private sealed class PartitionHelper<T> : IEnumerable<IReadOnlyCollection<T>>
        {
            readonly IEnumerable<T> _items;
            readonly int _partitionSize;
            bool _hasMoreItems;

            internal PartitionHelper(IEnumerable<T> i, int ps)
            {
                _items = i;
                _partitionSize = ps;
            }

            public IEnumerator<IReadOnlyCollection<T>> GetEnumerator()
            {
                using (var enumerator = _items.GetEnumerator())
                {
                    _hasMoreItems = enumerator.MoveNext();
                    while (_hasMoreItems)
                        yield return GetNextBatch(enumerator).ToList();
                }
            }

            private IEnumerable<T> GetNextBatch(IEnumerator<T> enumerator)
            {
                for (var i = 0; i < _partitionSize; ++i)
                {
                    yield return enumerator.Current;
                    _hasMoreItems = enumerator.MoveNext();
                    if (!_hasMoreItems)
                    {
                        yield break;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}