namespace AspNetCore.Examples.Auth.Api.Authorization;

public static class Policies
{
    public const string IsAlice = nameof(IsAlice);
    public const string IsBob = nameof(IsBob);
    public const string IsRecentLogin = nameof(IsRecentLogin);
    public const string IsSameAuthor = nameof(IsSameAuthor);
}
