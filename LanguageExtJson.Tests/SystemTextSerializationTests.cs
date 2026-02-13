using System.Text.Json;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtJson.Tests;

public class SystemTextSerializationTests
{
    private record TypesTest(int Int, Option<string> Opt, Seq<int> Seq);
    [Fact]
    public void SystemTextSerialize_WhenOptionOrSeqProperties_ShouldWork()
    {
        // Arrange
        var @object = new TypesTest(1, "2", [3, 4]);
        // Act
        var result = SystemTextRoundTrip<TypesTest>(@object);
        // Assert
        Assert.Equal(@object, result);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("test")]
    [InlineData("test123")]
    public void SystemTextSerialize_WhenOption_ShouldWork(string? value)
    {
        // Arrange
        var @object = Optional(value);
        // Act
        var result = SystemTextRoundTrip<Option<string>>(@object);
        // Assert
        Assert.Equal(@object, result);
    }
    
    [Theory]
    [InlineData()]
    [InlineData(1, 2, 3)]
    [InlineData(1, 2, 3, 4, 5, 6 ,7, 7, 8, 9, 0, 9, 8, 7, 6, 5, 4, 3, 2, 1)]
    public void SystemTextSerializeWhenSeq_ShouldWork(params int[] values)
    {
        // Arrange
        var @object = toSeq(values);
        // Act
        var result = SystemTextRoundTrip<Seq<int>>(@object);
        // Assert
        Assert.Equal(@object, result);
    }
    
    [Fact]
    public void SystemTextSerialize_WhenDeeplyNestedCustomTypes_ShouldWork()
    {
        // Arrange
        var @object = Seq(
            Some(Some(Seq(1, 2, 3))), 
            Some<Option<Seq<int>>>(None),
            Some(Some<Seq<int>>(Empty)),
            None);
        // Act
        var result = SystemTextRoundTrip<Seq<Option<Option<Seq<int>>>>>(@object);
        // Assert
        Assert.Equal(@object, result);
    }

    private static T? SystemTextRoundTrip<T>(object @object) =>
        JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(@object));
}