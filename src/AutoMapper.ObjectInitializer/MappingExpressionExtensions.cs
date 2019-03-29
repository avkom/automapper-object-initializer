using System;
using System.Linq.Expressions;

namespace AutoMapper.ObjectInitializer
{
    public static class MappingExpressionExtensions
    {
        public static IMappingExpression<TSource, TDestination> MapUsing<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TSource, TDestination>> ctor)
        {
            return mappingExpression;
        }
    }
}
