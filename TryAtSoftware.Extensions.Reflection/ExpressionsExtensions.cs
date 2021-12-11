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
}