namespace INuStore.Contract.Service.V1.Auth
{
    public static class Query
    {
        public record Login(string UserName, string Password);
    }
}
