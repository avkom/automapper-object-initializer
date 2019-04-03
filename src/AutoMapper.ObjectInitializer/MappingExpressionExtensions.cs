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
                var ctorExpressionWithoutMemberInit = Expression.Lambda<Func<TSource, TDestination>>(
                    memberInitExpression.NewExpression,
                    ctor.Parameters);

                return mappingExpression
                    .ConstructUsing(ctorExpressionWithoutMemberInit)
                    .MapUsing(ctor);
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
                        // DestinationProperty = default
                        if (memberAssignment.Expression is DefaultExpression)
                        {
                            mappingExpression.ForMember(memberBinding.Member.Name, opt => opt.Ignore());
                            break;
                        }

                        // DestinationProperty = MappingOptions.MapFrom<DateTime>(src.SourceProperty)
                        if (memberAssignment.Expression is MethodCallExpression methodCallExpression &&
                            methodCallExpression.Method.DeclaringType == typeof(MappingOptions) &&
                            methodCallExpression.Method.Name == nameof(MappingOptions.MapFrom))
                        {
                            dynamic mapFromExpression = Expression.Lambda(methodCallExpression.Arguments[0], ctor.Parameters);
                            mappingExpression.ForMember(memberBinding.Member.Name, opt => opt.MapFrom(mapFromExpression));
                        }

                        // DestinationProperty = src.SourceProperty
                        else
                        {
                            dynamic mapFromExpression = Expression.Lambda(memberAssignment.Expression, ctor.Parameters);
                            mappingExpression.ForMember(memberBinding.Member.Name, opt => opt.MapFrom(mapFromExpression));
                        }
                    }
                }

                return mappingExpression;
            }

            throw new ArgumentException("Parameter is not an object initializer expression.", nameof(ctor));
        }
    }
}
