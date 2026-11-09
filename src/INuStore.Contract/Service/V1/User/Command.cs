namespace INuStore.Contract.Service.V1.User
{
    public static class Command
    {
        public record CreateUser(string UserName, string Email, string Password);
        public record UpdateUser(int UserId, string UserName, string Email);
        public record DeleteUser(int UserId);

        /// <summary>Assigns a role using identifiers bound from the request route.</summary>
        public sealed record AssignRoleToUser(int UserId, int RoleId);

        /// <summary>Removes a role assignment using identifiers bound from the request route.</summary>
        public sealed record RemoveRoleFromUser(int UserId, int RoleId);
    }
}
