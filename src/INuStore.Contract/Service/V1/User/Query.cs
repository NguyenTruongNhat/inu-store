namespace INuStore.Contract.Service.V1.User
{
    public static class Query
    {
        public record GetUserById(int UserId);
        public record GetAllUsers(int PageIndex, int PageSize);
    }
}
