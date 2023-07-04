namespace AspNetCore.Examples.Auth.Api;

public record class Claim
{
    public required string Type { get; init; }
    public string Value { get; init; } = string.Empty;
}