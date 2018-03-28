﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTree._02
{
    public class WeirdMapper : IWeirdMapper
    {
        public void CreateMap<TSource, TDestination>()
        {
            var mapFunction = BuildMapFunction<TSource, TDestination>();
            var mapperRule = new KeyValuePair<Type, Type>(typeof(TSource), typeof(TDestination));
            if (_maps.ContainsKey(mapperRule))
                _maps.Remove(mapperRule);

            _maps.Add(mapperRule, mapFunction);
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            var mapperRule = new KeyValuePair<Type, Type>(typeof(TSource), typeof(TDestination));
            if (!(_maps[mapperRule] is Expression<Func<TSource, TDestination>> expression))
                throw new WeirdMapperException();

            return expression.Compile()(source);
        }

        private Expression<Func<TSource, TDestination>> BuildMapFunction<TSource, TDestination>()
        {
            var sourceParam = Expression.Parameter(typeof(TSource));
            var sourceMembers = typeof(TSource).GetMembers(BindingFlags.Instance | BindingFlags.Public).Where(x => x is PropertyInfo || x is FieldInfo);
            var destMembers = typeof(TDestination).GetMembers(BindingFlags.Instance | BindingFlags.Public).Where(x => x is PropertyInfo || x is FieldInfo);
            var joined = sourceMembers
                .Join(destMembers, x => x.Name, y => y.Name, (x, y) => new { Source = x, Dest = y });
            var members = joined
                .Select(x => Expression.Bind(x.Dest, Expression.PropertyOrField(sourceParam, x.Source.Name)));

            var body = Expression.MemberInit(Expression.New(typeof(TDestination)), members);

            return Expression.Lambda<Func<TSource, TDestination>>(
                body,
                sourceParam
            );
        }

        private readonly IDictionary<KeyValuePair<Type, Type>, Expression> _maps =
            new Dictionary<KeyValuePair<Type, Type>, Expression>();
    }
}