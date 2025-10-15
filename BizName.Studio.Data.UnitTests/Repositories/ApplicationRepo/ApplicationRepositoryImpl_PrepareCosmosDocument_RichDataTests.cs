using BizName.Studio.Data.Serialization;

namespace BizName.Studio.Data.UnitTests.Repositories.ApplicationRepo;

public class ApplicationRepositoryImplPrepareCosmosDocumentRichDataTests
{
    private readonly ApplicationRepositoryImpl<TestData.RichDataEntity> _repository = new(CreateMockDatabaseContext());

    [Fact]
    public void Should_Serialize_All_Core_Properties()
    {
        // Arrange
        var entity = TestData.NewRichTestEntity;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        document.Should().NotBeNull();
        
        // Core properties that should always be present
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.Id)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.Name)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.IntValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.LongValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.BoolValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.DecimalValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.DoubleValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.FloatValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.CharValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.DateTimeValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.DateTimeOffsetValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.TimeSpanValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.EnumValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.ByteArrayValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.StringList)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.DictionaryValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.HashSetValue)));
        document.Should().ContainKey(GetJsonPropertyName(nameof(entity.NestedValue)));
    }

    [Fact]
    public void Should_Preserve_String_And_Guid_Values()
    {
        // Arrange
        var entity = TestData.NewRichTestEntity;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        document[GetJsonPropertyName(nameof(entity.Id))]?.ToString().Should().Be(entity.Id.ToString());
        document[GetJsonPropertyName(nameof(entity.Name))]?.ToString().Should().Be(entity.Name);
        
        if (entity.NullableString != null)
        {
            document[GetJsonPropertyName(nameof(entity.NullableString))]?.ToString().Should().Be(entity.NullableString);
        }
    }

    [Fact]
    public void Should_Preserve_Numeric_Values()
    {
        // Arrange
        var entity = TestData.NewRichTestEntity;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        document[GetJsonPropertyName(nameof(entity.IntValue))]?.GetValue<int>().Should().Be(entity.IntValue);
        document[GetJsonPropertyName(nameof(entity.LongValue))]?.GetValue<long>().Should().Be(entity.LongValue);
        document[GetJsonPropertyName(nameof(entity.DecimalValue))]?.GetValue<decimal>().Should().Be(entity.DecimalValue);
        document[GetJsonPropertyName(nameof(entity.DoubleValue))]?.GetValue<double>().Should().Be(entity.DoubleValue);
        document[GetJsonPropertyName(nameof(entity.FloatValue))]?.GetValue<float>().Should().Be(entity.FloatValue);
        
        if (entity.NullableInt.HasValue)
        {
            document[GetJsonPropertyName(nameof(entity.NullableInt))]?.GetValue<int>().Should().Be(entity.NullableInt.Value);
        }
    }

    [Fact]
    public void Should_Preserve_Boolean_And_Character_Values()
    {
        // Arrange
        var entity = TestData.NewRichTestEntity;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        document[GetJsonPropertyName(nameof(entity.BoolValue))]?.GetValue<bool>().Should().Be(entity.BoolValue);
        document[GetJsonPropertyName(nameof(entity.CharValue))]?.ToString().Should().Be(entity.CharValue.ToString());
    }

    [Fact]
    public void Should_Preserve_DateTime_And_TimeSpan_Values()
    {
        // Arrange
        var entity = TestData.NewRichTestEntity;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        var dateTimeString = document[GetJsonPropertyName(nameof(entity.DateTimeValue))]?.ToString();
        dateTimeString.Should().NotBeNullOrEmpty();
        DateTime.Parse(dateTimeString!).Should().BeCloseTo(entity.DateTimeValue, TimeSpan.FromSeconds(1));
        
        var dateTimeOffsetString = document[GetJsonPropertyName(nameof(entity.DateTimeOffsetValue))]?.ToString();
        dateTimeOffsetString.Should().NotBeNullOrEmpty();
        DateTimeOffset.Parse(dateTimeOffsetString!).Should().BeCloseTo(entity.DateTimeOffsetValue, TimeSpan.FromSeconds(1));
        
        var timeSpanString = document[GetJsonPropertyName(nameof(entity.TimeSpanValue))]?.ToString();
        timeSpanString.Should().NotBeNullOrEmpty();
        TimeSpan.Parse(timeSpanString!).Should().Be(entity.TimeSpanValue);
    }

    [Fact]
    public void Should_Preserve_Enum_And_ByteArray_Values()
    {
        // Arrange
        var entity = TestData.NewRichTestEntity;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        var enumString = document[GetJsonPropertyName(nameof(entity.EnumValue))]?.ToString();
        enumString.Should().NotBeNullOrEmpty();
        Enum.Parse<DayOfWeek>(enumString!).Should().Be(entity.EnumValue);
        
        var byteArrayNode = document[GetJsonPropertyName(nameof(entity.ByteArrayValue))];
        byteArrayNode.Should().NotBeNull();
        // ByteArray is serialized as base64 string
        byteArrayNode.ToString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Should_Preserve_Collection_Values()
    {
        // Arrange
        var entity = TestData.NewRichTestEntity;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        var stringListNode = document[GetJsonPropertyName(nameof(entity.StringList))];
        stringListNode.Should().NotBeNull();
        var stringListArray = stringListNode!.AsArray();
        stringListArray.Count.Should().Be(entity.StringList.Count);
        
        var dictionaryNode = document[GetJsonPropertyName(nameof(entity.DictionaryValue))];
        dictionaryNode.Should().NotBeNull();
        var dictionaryObject = dictionaryNode!.AsObject();
        dictionaryObject.Should().ContainKeys(entity.DictionaryValue.Keys);
        
        var hashSetNode = document[GetJsonPropertyName(nameof(entity.HashSetValue))];
        hashSetNode.Should().NotBeNull();
        var hashSetArray = hashSetNode!.AsArray();
        hashSetArray.Count.Should().Be(entity.HashSetValue.Count);
    }

    [Fact]
    public void Should_Preserve_Nested_Object_Values()
    {
        // Arrange
        var entity = TestData.NewRichTestEntity;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        var nestedNode = document[GetJsonPropertyName(nameof(entity.NestedValue))];
        nestedNode.Should().NotBeNull();
        
        if (entity.NestedValue != null)
        {
            var nestedObject = nestedNode!.AsObject();
            nestedObject[GetJsonPropertyName(nameof(entity.NestedValue.NestedName))]?.ToString().Should().Be(entity.NestedValue.NestedName);
            nestedObject[GetJsonPropertyName(nameof(entity.NestedValue.NestedValue))]?.GetValue<int>().Should().Be(entity.NestedValue.NestedValue);
        }
    }

    private static string GetJsonPropertyName(string propertyName)
    {
        // Use the same naming policy as BizNameCosmosSerializationOptions
        return BizNameCosmosSerializationOptions.Default.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;
    }

    private static IDatabaseContext CreateMockDatabaseContext()
    {
        var context = Substitute.For<IDatabaseContext>();
        var client = Substitute.For<CosmosClient>();
        var container = Substitute.For<Container>();
        
        context.CosmosClient.Returns(client);
        context.DatabaseName.Returns("TestDb");
        client.GetContainer(Arg.Any<string>(), Arg.Any<string>()).Returns(container);
        
        return context;
    }
}
