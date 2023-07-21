namespace TryAtSoftware.Extensions.Collections.Tests;


using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class FluidDictionaryTests
{
    [Fact]
    public void SetAndGet()
    {
        // Arrange
        var dictionary = new FluidDictionary<string>();
        dictionary.Set("key1", 42);

        // Act
        bool success = dictionary.TryGetValue("key1", out int value);

        // Assert
        Assert.True(success);
        Assert.Equal(42, value);
    }

    [Fact]
    public void GetRequiredValue_KeyNotFound()
    {
        // Arrange
        var dictionary = new FluidDictionary<string>();

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => dictionary.GetRequiredValue<int>("key1"));
    }

    [Fact]
    public void GetRequiredValue_InvalidValueType()
    {
        // Arrange
        var dictionary = new FluidDictionary<string>();
        dictionary.Set("key1", "value");

        // Act & Assert
        Assert.Throws<InvalidCastException>(() => dictionary.GetRequiredValue<int>("key1"));
    }

    [Fact]
    public void GetKeys()
    {
        // Arrange
        var dictionary = new FluidDictionary<string>();
        dictionary.Set("key1", 1);
        dictionary.Set("key2", 2);
        dictionary.Set("key3", 3);

        // Act
        var keys = dictionary.Keys.ToList();

        // Assert
        Assert.Contains("key1", keys);
        Assert.Contains("key2", keys);
        Assert.Contains("key3", keys);
        Assert.Equal(3, keys.Count);
    }

    [Fact]
    public void Remove_KeyNotFound()
    {
        // Arrange
        var dictionary = new FluidDictionary<string>();

        // Act
        bool success = dictionary.Remove("key1");

        // Assert
        Assert.False(success);
    }
}
