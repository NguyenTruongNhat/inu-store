using System.Reflection;

namespace INuStore.UseCase;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
