namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// A static class containing standard extension methods that are useful when working with expressions.
/// </summary>
public static class ExpressionsExtensions
{
    /// <summary>
    /// Use this method to retrieve information about the member an expression is pointing to.
    /// </summary>
    /// <typeparam name="T">The type of parameter accepted by the function represented by the extended <paramref name="expression"/>.</typeparam>
    /// <typeparam name="TValue">The type of value returned by the function represented by the extended <paramref name="expression"/>.</typeparam>
    /// <param name="expression">The extended <see cref="Expression"/> instance.</param>
    /// <returns>Returns the <see cref="MemberInfo"/> instance the extended expression is pointing to.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the extended <paramref name="expression"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the extended <paramref name="expression"/> does not point to a member.</exception>
    public static MemberInfo GetMemberInfo<T, TValue>(this Expression<Func<T, TValue>> expression)
    {
        if (expression is null) throw new ArgumentNullException(nameof(expression));

        MemberExpression? memberExpression;
        if (expression.Body is UnaryExpression body) memberExpression = body.Operand as MemberExpression;
        else memberExpression = expression.Body as MemberExpression;

        var memberInfo = memberExpression?.Member;
        if (memberInfo is null) throw new InvalidOperationException("The member expression was not successfully interpreted.");

        return memberInfo;
    }

    /// <summary>
    /// Use this method in order to construct an <see cref="Expression"/> pointing to a given property.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="PropertyInfo"/> describing the property that should be referenced.</param>
    /// <typeparam name="T">The type containing the provided <paramref name="propertyInfo"/> (should be equal to its reflected type).</typeparam>
    /// <typeparam name="TValue">The type of value that should be retrieved from the requested property (if this value does not match the property type, a conversion will be applied).</typeparam>
    /// <returns>Returns a subsequently built expression pointing to the requested property.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="propertyInfo"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="MemberInfo.ReflectedType"/> of the provided <paramref name="propertyInfo"/> does not match <typeparamref name="T"/>.</exception>
    public static Expression<Func<T, TValue>> ConstructPropertyAccessor<T, TValue>(this PropertyInfo propertyInfo)
    {
        if (propertyInfo is null) throw new ArgumentNullException(nameof(propertyInfo));
        if (propertyInfo.ReflectedType != typeof(T)) throw new InvalidOperationException($"The provided property was obtained from a different type. Property name: {propertyInfo.Name}, T: {TypeNames<T>.Value}, Reflected type: {TypeNames.Get(propertyInfo.ReflectedType)}");
        if (!propertyInfo.CanRead) throw new InvalidOperationException("The property is not readable.");

        var parameter = Expression.Parameter(typeof(T));

        Expression accessPropertyValue = Expression.Property(parameter, propertyInfo);
        if (propertyInfo.PropertyType != typeof(TValue)) accessPropertyValue = Expression.Convert(accessPropertyValue, typeof(TValue));

        return Expression.Lambda<Func<T, TValue>>(accessPropertyValue, parameter);
    }

    public static Expression<Action<T, TValue>> ConstructPropertySetter<T, TValue>(this PropertyInfo propertyInfo)
    {
        if (propertyInfo is null) throw new ArgumentNullException(nameof(propertyInfo));
        if (propertyInfo.ReflectedType != typeof(T)) throw new InvalidOperationException($"The provided property was obtained from a different type. Property name: {propertyInfo.Name}, T: {TypeNames<T>.Value}, Reflected type: {TypeNames.Get(propertyInfo.ReflectedType)}");
        if (!propertyInfo.CanWrite) throw new InvalidOperationException("The property is read-only.");

        var instanceParameter = Expression.Parameter(typeof(T));
        var valueParameter = Expression.Parameter(typeof(TValue));

        Expression propertyExpression = Expression.Property(instanceParameter, propertyInfo);
        Expression valueExpression = valueParameter;
        if (propertyInfo.PropertyType != typeof(TValue)) valueExpression = Expression.Convert(valueExpression, propertyInfo.PropertyType);

        var assignExpression = Expression.Assign(propertyExpression, valueExpression);

        return Expression.Lambda<Action<T, TValue>>(assignExpression, instanceParameter, valueParameter);
    }
}