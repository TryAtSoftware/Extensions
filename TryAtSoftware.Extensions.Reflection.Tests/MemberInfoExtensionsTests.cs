namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Reflection;
using Xunit;

public class MemberInfoExtensionsTests
{
    [Fact]
    public void GetValueShouldThrowExceptionIfNullMemberInfoIsPassed()
    {
        var instance = new TestHelerClass();
        Assert.Throws<ArgumentNullException>(() => ((MemberInfo)null).GetValue(instance));
    }

    [Fact]
    public void GetValueShouldWorkCorrectlyWithFields()
    {
        var instance = new TestHelerClass();
        MemberInfo fieldInfo = typeof(TestHelerClass).GetField(nameof(TestHelerClass.field));

        var value = fieldInfo.GetValue(instance);
        Assert.Equal("This is a field", value);
    }

    [Fact]
    public void GetValueShouldWorkCorrectlyWithProperties()
    {
        var instance = new TestHelerClass();
        MemberInfo propertyInfo = typeof(TestHelerClass).GetProperty(nameof(TestHelerClass.Property));

        var value = propertyInfo.GetValue(instance);
        Assert.Equal("This is a property", value);
    }
    
    [Fact]
    public void GetValueShouldNotWorkWithMethods()
    {
        var instance = new TestHelerClass();
        MemberInfo methodInfo = typeof(TestHelerClass).GetMethod(nameof(TestHelerClass.Method));

        Assert.Throws<InvalidOperationException>(() => methodInfo.GetValue(instance));
    }

    private class TestHelerClass
    {
        public string field = "This is a field";

        public string Property => "This is a property";

        public string Method() => "This is a method";
    }
}
