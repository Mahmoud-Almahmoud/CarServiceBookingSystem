using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Infrastructure.Services;
using CarServiceBookingSystem.UnitTests.TestHelpers;
using FluentAssertions;

namespace CarServiceBookingSystem.UnitTests.Services;

public class ApiKeyServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_ApiKey_And_Return_RawKey()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var service = new ApiKeyService(context);

        var result = await service.CreateAsync("Test Key", "owner@example.com", DateTime.UtcNow.AddDays(1));

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.RawKey.Should().NotBeNullOrWhiteSpace();

        var keys = context.ApiKeys.ToList();
        keys.Should().HaveCount(1);
        keys[0].Name.Should().Be("Test Key");
        keys[0].Owner.Should().Be("owner@example.com");
        keys[0].IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_Should_Return_True_For_Valid_Key_And_Update_LastUsed()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var service = new ApiKeyService(context);

        var createResult = await service.CreateAsync("Test Key", null, DateTime.UtcNow.AddDays(1));

        createResult.Success.Should().BeTrue();
        var rawKey = createResult.Data!.RawKey;

        var isValid = await service.ValidateAsync(rawKey, "127.0.0.1");

        isValid.Should().BeTrue();

        var apiKey = context.ApiKeys.First();
        apiKey.LastUsedAt.Should().NotBeNull();
        apiKey.LastUsedIp.Should().Be("127.0.0.1");
    }

    [Fact]
    public async Task ValidateAsync_Should_Return_False_For_Invalid_Key()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var service = new ApiKeyService(context);

        var isValid = await service.ValidateAsync("invalid-key", null);

        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task RevokeAsync_Should_Mark_Key_As_Inactive_And_Validation_Fails()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var service = new ApiKeyService(context);

        var createResult = await service.CreateAsync("Revoke Key", null, DateTime.UtcNow.AddDays(1));

        createResult.Success.Should().BeTrue();
        var rawKey = createResult.Data!.RawKey;
        var id = createResult.Data.Id;

        var revokeResult = await service.RevokeAsync(id);

        revokeResult.Success.Should().BeTrue();

        var isValid = await service.ValidateAsync(rawKey, null);

        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_List_Of_Keys()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var service = new ApiKeyService(context);

        await service.CreateAsync("Key1", null, null);
        await service.CreateAsync("Key2", "owner", null);

        var result = await service.GetAllAsync();

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data!.Select(x => x.Name).Should().Contain(new[] { "Key1", "Key2" });
    }
}
