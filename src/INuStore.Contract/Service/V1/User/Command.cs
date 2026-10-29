namespace INuStore.Contract.Service.V1.User
{
    public static class Command
    {
        public record CreateUser(string UserName, string Email, string Password);
        public record UpdateUser(int UserId, string UserName, string Email);
        public record DeleteUser(int UserId);
    }
}
