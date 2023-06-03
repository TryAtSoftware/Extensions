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
    /// Use this method in order to construct an <see cref="Expression"/> for retrieving the value of a given property.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="PropertyInfo"/> describing the property that should be referenced.</param>
    /// <typeparam name="T">The type containing the provided <paramref name="propertyInfo"/> (should be equal to its reflected type).</typeparam>
    /// <typeparam name="TValue">The type of value that should be retrieved from the requested property (if this value does not match the property type, a conversion will be applied).</typeparam>
    /// <returns>Returns a subsequently built expression for retrieving the value of the requested property.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="propertyInfo"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="MemberInfo.ReflectedType"/> of the provided <paramref name="propertyInfo"/> does not match <typeparamref name="T"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the provided <paramref name="propertyInfo"/> is not readable.</exception>
    public static Expression<Func<T, TValue>> ConstructPropertyAccessor<T, TValue>(this PropertyInfo propertyInfo)
    {
        if (propertyInfo is null) throw new ArgumentNullException(nameof(propertyInfo));
        ValidateCorrectReflectedType(propertyInfo, typeof(T));
        if (!propertyInfo.CanRead) throw new InvalidOperationException("The property is not readable.");

        var parameter = Expression.Parameter(typeof(T));

        Expression accessPropertyValue = Expression.Property(parameter, propertyInfo);
        if (propertyInfo.PropertyType != typeof(TValue)) accessPropertyValue = Expression.Convert(accessPropertyValue, typeof(TValue));

        return Expression.Lambda<Func<T, TValue>>(accessPropertyValue, parameter);
    }

    /// <summary>
    /// Use this method in order to construct an <see cref="Expression"/> for setting the value of a given property.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="PropertyInfo"/> describing the property that should be referenced.</param>
    /// <typeparam name="T">The type containing the provided <paramref name="propertyInfo"/> (should be equal to its reflected type).</typeparam>
    /// <typeparam name="TValue">The type of value that should be set to the requested property (if this value does not match the property type, a conversion will be applied).</typeparam>
    /// <returns>Returns a subsequently built expression for setting a value to the requested property.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="propertyInfo"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="MemberInfo.ReflectedType"/> of the provided <paramref name="propertyInfo"/> does not match <typeparamref name="T"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the provided <paramref name="propertyInfo"/> is read-only.</exception>
    public static Expression<Action<T, TValue>> ConstructPropertySetter<T, TValue>(this PropertyInfo propertyInfo)
    {
        if (propertyInfo is null) throw new ArgumentNullException(nameof(propertyInfo));
        ValidateCorrectReflectedType(propertyInfo, typeof(T));
        if (!propertyInfo.CanWrite) throw new InvalidOperationException("The property is read-only.");

        var instanceParameter = Expression.Parameter(typeof(T));
        var valueParameter = Expression.Parameter(typeof(TValue));

        Expression propertyExpression = Expression.Property(instanceParameter, propertyInfo);
        Expression valueExpression = valueParameter;
        if (propertyInfo.PropertyType != typeof(TValue)) valueExpression = Expression.Convert(valueExpression, propertyInfo.PropertyType);

        var assignExpression = Expression.Assign(propertyExpression, valueExpression);

        return Expression.Lambda<Action<T, TValue>>(assignExpression, instanceParameter, valueParameter);
    }

    /// <summary>
    /// Use this method in order to construct an <see cref="Expression"/> for instantiation an object of type <typeparamref name="T"/> using the requested <paramref name="constructorInfo"/>.
    /// </summary>
    /// <param name="constructorInfo">The <see cref="ConstructorInfo"/> describing the constructor that should be used during the instantiation process.</param>
    /// <typeparam name="T">The type of object to be instantiated (should be equal to the reflected type of the extended <paramref name="constructorInfo"/>).</typeparam>
    /// <remarks>
    /// An expression constructed by this extension method can be compiled to a function that accepts an array of values that correspond to the parameters of the extended <paramref name="constructorInfo"/>.
    /// If any of the parameters is optional and its default value should be used, the corresponding element (from the provided array) must be <c>null</c>.
    /// </remarks>
    /// <returns>Returns a subsequently built expression for constructing a new instance of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="constructorInfo"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="MemberInfo.ReflectedType"/> of the provided <paramref name="constructorInfo"/> does not match <typeparamref name="T"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the provided <paramref name="constructorInfo"/> is a constructor of an abstract class.</exception>
    public static Expression<Func<object?[], T>> ConstructObjectInitializer<T>(this ConstructorInfo constructorInfo) => constructorInfo.ConstructObjectInitializer<T>(includeParametersCountValidation: false);

    /// <summary>
    /// Use this method in order to construct an <see cref="Expression"/> for instantiation an object of type <typeparamref name="T"/> using the requested <paramref name="constructorInfo"/>.
    /// </summary>
    /// <param name="constructorInfo">The <see cref="ConstructorInfo"/> describing the constructor that should be used during the instantiation process.</param>
    /// <param name="includeParametersCountValidation">A boolean indicating whether or not the subsequently constructed expression should include a check to validate the correct count of provided values.</param>
    /// <typeparam name="T">The type of object to be instantiated (should be equal to the reflected type of the extended <paramref name="constructorInfo"/>).</typeparam>
    /// <remarks>
    /// An expression constructed by this extension method can be compiled to a function that accepts an array of values that correspond to the parameters of the extended <paramref name="constructorInfo"/>.
    /// If any of the parameters is optional and its default value should be used, the corresponding element (from the provided array) must be <c>null</c>.
    /// </remarks>
    /// <returns>Returns a subsequently built expression for constructing a new instance of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="constructorInfo"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="MemberInfo.ReflectedType"/> of the provided <paramref name="constructorInfo"/> does not match <typeparamref name="T"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the provided <paramref name="constructorInfo"/> is a constructor of an abstract class.</exception>
    public static Expression<Func<object?[], T>> ConstructObjectInitializer<T>(this ConstructorInfo constructorInfo, bool includeParametersCountValidation)
    {
        if (constructorInfo is null) throw new ArgumentNullException(nameof(constructorInfo));
        ValidateCorrectReflectedType(constructorInfo, typeof(T));

        if (constructorInfo.DeclaringType is null) throw new InvalidOperationException("The `DeclaringType` property for the constructor was null.");
        if (constructorInfo.DeclaringType.IsAbstract) throw new InvalidOperationException("This is a constructor of an abstract class.");

        var argumentsParameter = Expression.Parameter(typeof(object?[]));

        var parameters = constructorInfo.GetParameters();
        var parameterExpressions = new Expression[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            parameterExpressions[i] = Expression.ArrayIndex(argumentsParameter, Expression.Constant(i));
            if (parameters[i].HasDefaultValue) parameterExpressions[i] = Expression.Coalesce(parameterExpressions[i], Expression.Constant(parameters[i].DefaultValue));
            if (parameters[i].ParameterType != typeof(object)) parameterExpressions[i] = Expression.Convert(parameterExpressions[i], parameters[i].ParameterType);
        }

        Expression initializeExpression = Expression.New(constructorInfo, parameterExpressions);
        if (includeParametersCountValidation)
        {
            var incorrectParametersLengthException = new InvalidOperationException($"The object initializer requires {parameters.Length} parameters to be provided.");
            var validateParametersLengthExpression = Expression.IfThen(Expression.NotEqual(Expression.ArrayLength(argumentsParameter), Expression.Constant(parameters.Length)), Expression.Throw(Expression.Constant(incorrectParametersLengthException)));
            initializeExpression = Expression.Block(validateParametersLengthExpression, initializeExpression);
        }

        return Expression.Lambda<Func<object?[], T>>(initializeExpression, argumentsParameter);
    }

    private static void ValidateCorrectReflectedType(MemberInfo memberInfo, Type operativeType)
    {
        var reflectedType = memberInfo.ReflectedType;
        if (reflectedType is null) throw new InvalidOperationException($"The `ReflectedType` property for the member {memberInfo.Name} was null.");
        if (!reflectedType.IsAssignableFrom(operativeType)) throw new InvalidOperationException($"The provided member was obtained from a different type. Member name: {memberInfo.Name}, T: {TypeNames.Get(operativeType)}, Reflected type: {TypeNames.Get(reflectedType)}");
    }
}