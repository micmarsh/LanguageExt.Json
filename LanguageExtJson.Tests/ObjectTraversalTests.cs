using System.Text.Json;

namespace LanguageExtJson.Tests;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Json<LanguageExt.Fin>;

public class ObjectTraversalTests
{
    private static readonly Lazy<string> ProductString =
        new (() => File.ReadAllText("TestProduct.json"));
    
    [Fact]
    public void NestedArrayLookup()
    {
        // Arrange
        var getReviewers = key("reviews") >> iterate >>
                           (elts => elts.Traverse(key("reviewerEmail"))) >>
                           (elts => elts.Traverse(cast<string>));
        // Act
        var emails = parse(ProductString.Value).Bind(getReviewers).As().ThrowIfFail();
        
        // Assert
        Assert.Equal(["john.doe@x.dummyjson.com",
            "nolan.gonzalez@x.dummyjson.com", 
            "scarlett.wright@x.dummyjson.com"],
            emails);
    }
    
    private record TypesTest(int Int, Seq<Option<string>> Seq);

    [Fact]
    public void SerializedCustomTypes()
    {
        // Arrange
        var testObj = new TypesTest(123, [Some("hello"), Some("world"), None]);
        var str = +serialize(testObj);
        
        // Act
        var asJson = str >> parse;
        var number = asJson >> key(nameof(TypesTest.Int)) >> cast<int>;
        var sequence = asJson >> key(nameof(TypesTest.Seq)) >> cast<Seq<Option<string>>>;
        
        // Assert
        Assert.Equal(testObj.Int, number.ThrowIfFail());
        Assert.Equal(testObj.Seq, sequence.ThrowIfFail());
    }

}