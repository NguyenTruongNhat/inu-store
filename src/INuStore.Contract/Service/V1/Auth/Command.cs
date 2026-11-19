namespace INuStore.Contract.Service.V1.Auth;

public static class Command
{
    public sealed record Register(string UserName, string Email, string Password);
    public sealed record Revoke(string AccessToken);

}
