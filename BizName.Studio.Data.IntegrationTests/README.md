# BizName.Studio.Data Integration Tests

This project contains integration tests for the BizName.Studio.Data layer, specifically testing the Cosmos DB implementation.

## Prerequisites

- Cosmos DB Emulator running locally on `https://localhost:8081/`
- .NET 9 SDK installed

## Running Tests

```bash
# Run all integration tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=TenantSeedingTests"

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"
```

## Test Categories

### DatabaseInitializationTests
- Tests database and container creation
- Verifies service registration and dependency injection

### TenantSeedingTests
- Creates sample tenant data for development
- Tests basic CRUD operations on application-partitioned entities
- Includes a "Development Tenant" that can be used for UI/API testing

### RepositoryIntegrationTests
- Comprehensive tests for all entity types (Tenant, Website, Experience)
- Tests both application-level and tenant-level partitioning
- Verifies tenant isolation works correctly
- Tests full data lifecycle (Create, Read, Update, Delete)

## Configuration

The tests use `appsettings.json` to configure:
- Cosmos DB connection string (defaults to emulator)
- Database name: `BizNameTests` 
- Application partition key: `APPLICATION`

## Test Data

Running these tests will create:
- A test database called `BizNameTests`
- Sample tenant data including a "Development Tenant"
- Test websites and experiences for validation

The test database persists after test runs for inspection and development use.