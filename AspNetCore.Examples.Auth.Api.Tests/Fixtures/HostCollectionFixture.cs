namespace AspNetCore.Examples.Auth.Api.Tests.Fixtures;

[CollectionDefinition(nameof(HostCollectionFixture))]
public class HostCollectionFixture : ICollectionFixture<HostFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}