namespace INuStore.Contract.Service.V1.ApplicationParameter;

public static class Response
{
    public sealed record ApplicationParameterResponse(string Key, string Value);
    public sealed record GetByKeyResponse(ApplicationParameterResponse Parameter);
}
