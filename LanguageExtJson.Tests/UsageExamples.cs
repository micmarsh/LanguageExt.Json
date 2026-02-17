using LanguageExt;
using static LanguageExt.Json<LanguageExt.Fin>;

namespace LanguageExtJson.Tests;

public class UsageExamples
{
    private static readonly Lazy<string> ProductString =
        new (() => File.ReadAllText("TestProduct.json"));

    public record Product(int id, string title, Seq<string> tags, double price);

    [Fact]
    public void deserialize()
    {
        var productString = ProductString.Value;
        var product = deserialize<Product>(productString).As().ThrowIfFail();
    }

    [Fact]
    public void test_serialize()
    {
        var product = new Product(123, "Product 123", ["tag1", "tag2"], 9.99);
        var productString = serialize(product);
        Assert.Equal(product, productString >> deserialize<Product>);
    }
}