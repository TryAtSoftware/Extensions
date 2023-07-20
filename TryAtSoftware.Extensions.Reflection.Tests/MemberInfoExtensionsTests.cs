namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Reflection;
using Xunit;

public class MemberInfoExtensionsTests
{
    [Fact]
    public void GetValueShouldThrowExceptionIfNullMemberInfoIsPassed()
    {
        var instance = new MemberInfoExtensionsTestHelperClass();
        Assert.Throws<ArgumentNullException>(() => ((MemberInfo)null!).GetValue(instance));
    }

    [Fact]
    public void GetValueShouldWorkCorrectlyWithFields()
    {
        var instance = new MemberInfoExtensionsTestHelperClass();
        var fieldInfo = typeof(MemberInfoExtensionsTestHelperClass).GetField(nameof(MemberInfoExtensionsTestHelperClass.field));
        Assert.NotNull(fieldInfo);

        var value = fieldInfo.GetValue(instance);
        Assert.Equal("This is a field", value);
    }

    [Fact]
    public void GetValueShouldWorkCorrectlyWithProperties()
    {
        var instance = new MemberInfoExtensionsTestHelperClass();
        var propertyInfo = typeof(MemberInfoExtensionsTestHelperClass).GetProperty(nameof(MemberInfoExtensionsTestHelperClass.Property));
        Assert.NotNull(propertyInfo);

        var value = propertyInfo.GetValue(instance);
        Assert.Equal("This is a property", value);
    }
    
    [Fact]
    public void GetValueShouldNotWorkWithMethods()
    {
        var instance = new MemberInfoExtensionsTestHelperClass();
        var methodInfo = typeof(MemberInfoExtensionsTestHelperClass).GetMethod(nameof(MemberInfoExtensionsTestHelperClass.Method));
        Assert.NotNull(methodInfo);

        Assert.Throws<InvalidOperationException>(() => methodInfo.GetValue(instance));
    }

    private class MemberInfoExtensionsTestHelperClass
    {
        public string field = "This is a field";

#pragma warning disable CA1822
        public string Property => "This is a property";
#pragma warning restore CA1822

#pragma warning disable CA1822
        public string Method() => "This is a method";
#pragma warning restore CA1822
    }
}
