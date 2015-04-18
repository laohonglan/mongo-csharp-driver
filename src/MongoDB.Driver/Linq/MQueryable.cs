﻿/* Copyright 2010-2014 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Driver.Linq
{
    internal class MQueryable<TInput, TOutput> : IOrderedMongoQueryable<TOutput>
    {
        private readonly IMongoQueryProvider _queryProvider;
        private readonly Expression _expression;

        public MQueryable(IMongoQueryProvider queryProvider)
        {
            _queryProvider = Ensure.IsNotNull(queryProvider, "queryProvider");
            _expression = Expression.Constant(this, typeof(IMongoQueryable<TOutput>));
        }

        public MQueryable(IMongoQueryProvider queryProvider, Expression expression)
        {
            _queryProvider = Ensure.IsNotNull(queryProvider, "queryProvider");
            _expression = Ensure.IsNotNull(expression, "expression");
        }

        public Type ElementType
        {
            get { return typeof(TOutput); }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public IQueryProvider Provider
        {
            get { return _queryProvider; }
        }

        public QueryableExecutionModel BuildExecutionModel()
        {
            return ((MQueryProvider<TInput>)_queryProvider).BuildExecutionModel(_expression);
        }

        public IEnumerator<TOutput> GetEnumerator()
        {
            var results = _queryProvider.Execute(_expression);
            return ((IEnumerable<TOutput>)results).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Task<IAsyncCursor<TOutput>> ToCursorAsync(CancellationToken cancellationToken)
        {
            return _queryProvider.ExecuteAsync<IAsyncCursor<TOutput>>(_expression, cancellationToken);
        }

        public override string ToString()
        {
            return BuildExecutionModel().ToString();
        }
    }
}
