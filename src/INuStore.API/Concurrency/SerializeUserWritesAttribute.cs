namespace INuStore.API.Concurrency;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SerializeUserWritesAttribute : Attribute
{
}
