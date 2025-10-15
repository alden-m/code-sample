using BizName.Studio.Data.Serialization;

namespace BizName.Studio.Data.UnitTests.Repositories.ApplicationRepo;

public class ApplicationRepositoryImplPrepareCosmosDocumentSimpleTests
{
    private readonly ApplicationRepositoryImpl<TestData.TestEntity> _repository = new(CreateMockDatabaseContext());

    [Fact]
    public void Should_Add_PartitionKey_To_Document()
    {
        // Arrange
        var entity = TestData.NewTestEntity;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        document.Should().NotBeNull();
        document.Should().ContainKey(DatabaseConstants.PartitionKeysNames.Application);
        document[DatabaseConstants.PartitionKeysNames.Application]?.ToString().Should().Be("APPLICATION");
    }

    [Fact]
    public void Should_Preserve_Core_Entity_Properties()
    {
        // Arrange
        var entity = TestData.NewTestEntity;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        document.Should().NotBeNull();
        document[GetJsonPropertyName(nameof(entity.Id))]?.ToString().Should().Be(entity.Id.ToString());
        document[GetJsonPropertyName(nameof(entity.Name))]?.ToString().Should().Be(entity.Name);
        document[GetJsonPropertyName(nameof(entity.Description))]?.ToString().Should().Be(entity.Description);
        document[GetJsonPropertyName(nameof(entity.Priority))]?.GetValue<int>().Should().Be(entity.Priority);
    }

    [Fact]
    public void Should_Not_Modify_Original_Entity()
    {
        // Arrange
        var entity = TestData.NewTestEntity;
        var originalId = entity.Id;
        var originalName = entity.Name;
        var originalDescription = entity.Description;
        var originalPriority = entity.Priority;
        var originalCreatedAt = entity.CreatedAt;
        
        // Act
        var document = _repository.PrepareCosmosDocument(entity);
        
        // Assert - Verify the original entity remains unchanged
        entity.Id.Should().Be(originalId);
        entity.Name.Should().Be(originalName);
        entity.Description.Should().Be(originalDescription);
        entity.Priority.Should().Be(originalPriority);
        entity.CreatedAt.Should().Be(originalCreatedAt);
        
        // Verify document was created
        document.Should().NotBeNull();
        document.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_Create_New_Document_Instance_Each_Time()
    {
        // Arrange
        var entity = TestData.NewTestEntity;
        
        // Act
        var document1 = _repository.PrepareCosmosDocument(entity);
        var document2 = _repository.PrepareCosmosDocument(entity);
        
        // Assert
        document1.Should().NotBeSameAs(document2);
        
        // But both should have the same content
        document1[DatabaseConstants.PartitionKeysNames.Application]?.ToString().Should().Be(document2[DatabaseConstants.PartitionKeysNames.Application]?.ToString());
        document1[GetJsonPropertyName(nameof(TestData.TestEntity.Id))]?.ToString().Should().Be(document2[GetJsonPropertyName(nameof(TestData.TestEntity.Id))]?.ToString());
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
