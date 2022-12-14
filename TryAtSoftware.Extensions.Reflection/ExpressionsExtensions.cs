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

    /// <summary>
    /// Use this method in order to construct an <see cref="Expression"/> pointing to a given property.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="PropertyInfo"/> describing the property that should be referenced.</param>
    /// <typeparam name="T">The type containing the provided <paramref name="propertyInfo"/> (should be equal to its reflected type).</typeparam>
    /// <typeparam name="TValue">The type of value that should be retrieved from the requested property (if this value does not match the property type, a conversion will be applied).</typeparam>
    /// <returns>Returns a subsequently built expression pointing to the requested property.</returns>
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