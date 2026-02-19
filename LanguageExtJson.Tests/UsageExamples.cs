using System.Text.Json;
using LanguageExt;
using static LanguageExt.Json<LanguageExt.Fin>;
using static LanguageExt.Prelude;

namespace LanguageExtJson.Tests;

public class UsageExamples
{
    public record Product(int id, string title, Seq<string> tags, double price, Option<string> thumbnail);
    

    [Fact]
    public void test_serialize()
    {
        var product = new Product(123, "Product 123", ["tag1", "tag2"], 9.99, None);
        var productString = serialize(product);
        Assert.Equal(product, productString >> deserialize<Product>);
    }

    public record Review(string reviewerEmail, int rating);
    public record Product2(int id, Seq<Review> reviews);
    
    [Fact]
    public void test_parse_query()
    {
        var product = new Product2(123, [
            new Review("foo@bar.com", 5), 
            new Review("you@bar.com", 2),
            new Review("me@bar.com", 1)]);
        var productString = serialize(product);
        var emails = productString >> parse
                                   >> key("reviews")
                                   >> iterate
                                   >> traverse(key("reviewerEmail") >> cast<string>);
        Assert.Equal(product.reviews.Map(r => r.reviewerEmail), emails);
    }

    [Fact]
    public void test_error()
    {
        var product = new Product2(123, [
            new Review("foo@bar.com", 5)
        ]);
        var shouldThrow = serialize(product) >> parse
                                             >> key("reviews")
                                             >> key("reviewzzz");
        
        var ex = Assert.Throws<JsonErrorException>(() => shouldThrow.ThrowIfFail());
        Assert.Contains("reviewzzz", ex.Message);
        Assert.Contains(JsonValueKind.Array.ToString(), ex.Message);
        Assert.Equal(JsonError.Code, ex.Code);
    }

}