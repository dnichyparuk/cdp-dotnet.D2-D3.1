﻿using System;
using System.Linq;
using System.Linq.Expressions;
using IQueryable._01.E3SClient;
using Sample03;

namespace IQueryable._01
{
    public class E3SLinqProvider : IQueryProvider
    {
        private readonly E3SQueryClient _e3SClient;

        public E3SLinqProvider(E3SQueryClient client)
        {
            _e3SClient = client;
        }

        public System.Linq.IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new E3SQuery<TElement>(expression, this);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var itemType = TypeHelper.GetElementType(expression.Type);

            var translator = new ExpressionToFtsRequestTranslator();
            var queryString = translator.Translate(expression);
            var queries = queryString.Split(new [] { Constants.QUERY_AND_DELIMITER }, StringSplitOptions.None);

            return (TResult)_e3SClient.SearchFTS(itemType, query: queries);
        }
    }
}
