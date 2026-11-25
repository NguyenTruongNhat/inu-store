namespace INuStore.Contract.Service.V1.ApplicationParameter;

public static class Command
{
    public sealed record Create(string Key, string Value);
    public sealed record Update(string Key, string Value);
}
