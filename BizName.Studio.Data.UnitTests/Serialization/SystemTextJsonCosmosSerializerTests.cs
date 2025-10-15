using System.Text;
using System.Text.Json.Serialization;
using BizName.Studio.Data.Serialization;

namespace BizName.Studio.Data.UnitTests.Serialization;

public class SystemTextJsonCosmosSerializerTests
{
    [Fact]
    public void Constructor_Default_Should_Use_BizNameCosmosSerializationOptions()
    {
        // Arrange & Act
        var serializer = new SystemTextJsonCosmosSerializer();
        
        // Assert
        serializer.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithCustomOptions_Should_Use_ProvidedOptions()
    {
        // Arrange
        var customOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        
        // Act
        var serializer = new SystemTextJsonCosmosSerializer(customOptions);
        
        // Assert
        serializer.Should().NotBeNull();
    }

    [Fact]
    public void FromStream_WithValidJson_Should_DeserializeCorrectly()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        var entity = TestData.NewTestEntity;
        var json = JsonSerializer.Serialize(entity, BizNameCosmosSerializationOptions.Default);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        
        // Act
        var result = serializer.FromStream<TestData.TestEntity>(stream);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.Name.Should().Be(entity.Name);
        result.Description.Should().Be(entity.Description);
        result.CreatedAt.Should().Be(entity.CreatedAt);
        result.Priority.Should().Be(entity.Priority);
    }

    [Fact]
    public void FromStream_WithEmptyStream_Should_ReturnDefault()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        var emptyStream = new MemoryStream();
        
        // Act
        var result = serializer.FromStream<TestData.TestEntity>(emptyStream);
        
        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FromStream_WithStreamTargetType_Should_ReturnOriginalStream()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        var originalStream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
        
        // Act
        var result = serializer.FromStream<Stream>(originalStream);
        
        // Assert
        result.Should().BeSameAs(originalStream);
    }

    [Fact]
    public void FromStream_WithInvalidJson_Should_ThrowException()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        var invalidJson = "{ invalid json }";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(invalidJson));
        
        // Act & Assert
        var act = () => serializer.FromStream<TestData.TestEntity>(stream);
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ToStream_WithValidObject_Should_SerializeCorrectly()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        var entity = TestData.NewTestEntity;
        
        // Act
        var resultStream = serializer.ToStream(entity);
        
        // Assert
        resultStream.Should().NotBeNull();
        resultStream.Position.Should().Be(0);
        resultStream.CanRead.Should().BeTrue();
        
        using var reader = new StreamReader(resultStream);
        var json = reader.ReadToEnd();
        json.Should().NotBeEmpty();
        
        var deserializedEntity = JsonSerializer.Deserialize<TestData.TestEntity>(json, BizNameCosmosSerializationOptions.Default);
        deserializedEntity.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public void ToStream_WithNull_Should_SerializeAsNull()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        TestData.TestEntity? nullEntity = null;
        
        // Act
        var resultStream = serializer.ToStream(nullEntity);
        
        // Assert
        resultStream.Should().NotBeNull();
        resultStream.Position.Should().Be(0);
        
        using var reader = new StreamReader(resultStream);
        var json = reader.ReadToEnd();
        json.Should().Be("null");
    }

    [Fact]
    public void ToStream_Should_UseCamelCaseNaming()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        var entity = new TestData.TestEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Name",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            Priority = 5
        };
        
        // Act
        var resultStream = serializer.ToStream(entity);
        
        // Assert
        using var reader = new StreamReader(resultStream);
        var json = reader.ReadToEnd();
        
        json.Should().Contain("\"id\":");
        json.Should().Contain("\"name\":");
        json.Should().Contain("\"description\":");
        json.Should().Contain("\"createdAt\":");
        json.Should().Contain("\"priority\":");
    }

    [Fact]
    public void ToStream_Should_IgnoreNullValues()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        var entity = new TestData.RichDataEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Name",
            NullableString = null,
            NullableInt = null
        };
        
        // Act
        var resultStream = serializer.ToStream(entity);
        
        // Assert
        using var reader = new StreamReader(resultStream);
        var json = reader.ReadToEnd();
        
        json.Should().NotContain("\"nullableString\":");
        json.Should().NotContain("\"nullableInt\":");
    }

    [Fact]
    public void ToStream_Should_SerializeEnumsAsStrings()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        var entity = new TestData.RichDataEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Name",
            EnumValue = DayOfWeek.Monday
        };
        
        // Act
        var resultStream = serializer.ToStream(entity);
        
        // Assert
        using var reader = new StreamReader(resultStream);
        var json = reader.ReadToEnd();
        
        json.Should().Contain("\"Monday\"");
    }

    [Fact]
    public void RoundTrip_Should_PreserveObjectData()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        var originalEntity = new TestData.TestEntity
        {
            Id = Guid.Parse("12345678-1234-1234-1234-123456789012"),
            Name = "Test Name",
            Description = "Test Description",
            CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Priority = 5
        };
        
        // Act
        var stream = serializer.ToStream(originalEntity);
        var deserializedEntity = serializer.FromStream<TestData.TestEntity>(stream);
        
        // Assert
        deserializedEntity.Should().BeEquivalentTo(originalEntity);
    }

    [Fact]
    public void RoundTrip_WithComplexObject_Should_PreserveAllProperties()
    {
        // Arrange
        var serializer = new SystemTextJsonCosmosSerializer();
        var originalEntity = new TestData.RichDataEntity
        {
            Id = Guid.Parse("12345678-1234-1234-1234-123456789012"),
            Name = "Complex Test",
            NullableString = "Not null",
            IntValue = 42,
            NullableInt = 100,
            LongValue = 9876543210L,
            BoolValue = true,
            DecimalValue = 123.45m,
            DoubleValue = 678.90,
            FloatValue = 12.34f,
            CharValue = 'X',
            DateTimeValue = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            DateTimeOffsetValue = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            TimeSpanValue = TimeSpan.FromHours(2),
            EnumValue = DayOfWeek.Friday,
            ByteArrayValue = new byte[] { 1, 2, 3, 4, 5 },
            StringList = new List<string> { "item1", "item2", "item3" },
            DictionaryValue = new Dictionary<string, object>(),
            HashSetValue = new HashSet<int> { 1, 2, 3 },
            NestedValue = new TestData.RichDataEntity.NestedObject
            {
                NestedName = "Nested Test",
                NestedValue = 42
            }
        };
        
        // Act
        var stream = serializer.ToStream(originalEntity);
        var deserializedEntity = serializer.FromStream<TestData.RichDataEntity>(stream);
        
        // Assert
        deserializedEntity.Should().BeEquivalentTo(originalEntity, options => options
            .Using<byte[]>(ctx => ctx.Subject.Should().Equal(ctx.Expectation))
            .WhenTypeIs<byte[]>()
            .Using<HashSet<int>>(ctx => ctx.Subject.Should().BeEquivalentTo(ctx.Expectation))
            .WhenTypeIs<HashSet<int>>());
    }

    [Fact]
    public void CustomOptions_Should_OverrideDefaultBehavior()
    {
        // Arrange
        var customOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };
        var serializer = new SystemTextJsonCosmosSerializer(customOptions);
        var entity = new TestData.TestEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Name"
        };
        
        // Act
        var resultStream = serializer.ToStream(entity);
        
        // Assert
        using var reader = new StreamReader(resultStream);
        var json = reader.ReadToEnd();
        
        // Should use snake_case instead of camelCase
        json.Should().Contain("\"id\":");
        json.Should().Contain("\"name\":");
    }
}
