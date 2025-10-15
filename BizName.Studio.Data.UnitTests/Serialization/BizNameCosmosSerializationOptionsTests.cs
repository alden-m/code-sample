using System.Text.Json;
using Microsoft.Azure.Cosmos;
using BizName.Studio.Data.Serialization;

namespace BizName.Studio.Data.UnitTests.Serialization;

public class BizNameCosmosSerializationOptionsTests
{
    [Fact]
    public void Default_PropertyNamingPolicy_Should_Be_CamelCase()
    {
        // Arrange & Act
        var options = BizNameCosmosSerializationOptions.Default;
        
        // Assert
        options.PropertyNamingPolicy.Should().Be(JsonNamingPolicy.CamelCase);
    }

    [Fact]
    public void LinqDefault_PropertyNamingPolicy_Should_Be_CamelCase()
    {
        // Arrange & Act
        var options = BizNameCosmosSerializationOptions.LinqDefault;
        
        // Assert
        options.PropertyNamingPolicy.Should().Be(CosmosPropertyNamingPolicy.CamelCase);
    }
}
