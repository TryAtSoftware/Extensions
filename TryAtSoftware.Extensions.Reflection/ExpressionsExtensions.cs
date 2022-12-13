namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

public static class ExpressionsExtensions
{
    [NotNull]
    public static MemberInfo GetMemberInfo<T, TValue>([NotNull] this Expression<Func<T, TValue>> propertyLambda)
    {
        if (propertyLambda is null)
            throw new ArgumentNullException(nameof(propertyLambda));

        MemberExpression memberExpression;
        if (propertyLambda.Body is UnaryExpression body)
            memberExpression = body.Operand as MemberExpression;
        else
            memberExpression = propertyLambda.Body as MemberExpression;

        var memberInfo = memberExpression?.Member;
        if (memberInfo is null)
            throw new InvalidOperationException("The member expression was not successfully interpreted.");

        return memberInfo;
    }

    public static Expression<Func<T, TValue>> ConstructPropertyAccessor<T, TValue>([NotNull] this PropertyInfo propertyInfo)
    {
        if (propertyInfo is null) throw new ArgumentNullException(nameof(propertyInfo));
        if (propertyInfo.ReflectedType != typeof(T)) throw new InvalidOperationException($"The provided property was obtained from a different type. Property name: {propertyInfo.Name}, T: {TypeNames<T>.Value}, Reflected type: {TypeNames.Get(propertyInfo.ReflectedType)}");
        
        var parameter = Expression.Parameter(typeof(T));
        
        Expression accessPropertyValue = Expression.Property(parameter, propertyInfo);
        if (propertyInfo.PropertyType != typeof(TValue)) accessPropertyValue = Expression.Convert(accessPropertyValue, typeof(TValue));
        
        return Expression.Lambda<Func<T, TValue>>(accessPropertyValue, parameter);
    }
}