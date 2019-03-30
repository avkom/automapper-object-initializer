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
            if (ctor.Body is NewExpression)
            {
                return mappingExpression.ConstructUsing(ctor);
            }

            if (ctor.Body is MemberInitExpression memberInitExpression)
            {
                LambdaExpression lambdaExpression =
                    Expression.Lambda<Func<TSource, TDestination>>(memberInitExpression.NewExpression, ctor.Parameters);
                mappingExpression.ConstructUsing((Expression<Func<TSource, TDestination>>) lambdaExpression);

                return mappingExpression;
            }

            throw new ArgumentException("Parameter is not an object initializer expression.", nameof(ctor));
        }
    }
}
