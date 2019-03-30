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

                foreach (MemberBinding memberBinding in memberInitExpression.Bindings)
                {
                    if (memberBinding is MemberAssignment memberAssignment)
                    {
                        var mapExpression = Expression.Lambda<Func<TSource, object>>(
                            memberAssignment.Expression,
                            Expression.Parameter(typeof(TSource)));

                        mappingExpression.ForMember(
                            memberBinding.Member.Name,
                            opt => opt.MapFrom(mapExpression));
                    }
                }

                return mappingExpression;
            }

            throw new ArgumentException("Parameter is not an object initializer expression.", nameof(ctor));
        }
    }
}
