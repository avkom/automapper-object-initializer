using System;
using System.Linq.Expressions;

namespace AutoMapper.ObjectInitializer
{
    public static class MappingExpressionExtensions
    {
        public static IMappingExpression<TSource, TDestination> ConstructAndMapUsing<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TSource, TDestination>> ctor)
        {
            if (ctor.Body is MemberInitExpression memberInitExpression)
            {
                var lambdaExpression = Expression.Lambda<Func<TSource, TDestination>>(
                    memberInitExpression.NewExpression,
                    ctor.Parameters);

                mappingExpression.ConstructUsing(lambdaExpression);

                return MapUsing(mappingExpression, ctor);
            }

            throw new ArgumentException("Parameter is not an object initializer expression.", nameof(ctor));
        }

        public static IMappingExpression<TSource, TDestination> MapUsing<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TSource, TDestination>> ctor)
        {
            if (ctor.Body is MemberInitExpression memberInitExpression)
            {
                foreach (MemberBinding memberBinding in memberInitExpression.Bindings)
                {
                    if (memberBinding is MemberAssignment memberAssignment)
                    {
                        if (memberAssignment.Expression is DefaultExpression)
                        {
                            mappingExpression.ForMember(
                                memberBinding.Member.Name,
                                opt => opt.Ignore());
                            break;
                        }

                        Expression mapFromExpressionBody;

                        if (memberAssignment.Expression is MethodCallExpression methodCallExpression &&
                            methodCallExpression.Method.DeclaringType == typeof(MappingOptions) &&
                            methodCallExpression.Method.Name == nameof(MappingOptions.MapFrom))
                        {
                            mapFromExpressionBody = methodCallExpression.Arguments[0];
                        }
                        else
                        {
                            mapFromExpressionBody = memberAssignment.Expression;
                        }

                        dynamic mapFromExpression = Expression.Lambda(
                            mapFromExpressionBody,
                            ctor.Parameters);

                        mappingExpression.ForMember(
                            memberBinding.Member.Name,
                            opt => opt.MapFrom(mapFromExpression));
                    }
                }

                return mappingExpression;
            }

            throw new ArgumentException("Parameter is not an object initializer expression.", nameof(ctor));
        }
    }
}
