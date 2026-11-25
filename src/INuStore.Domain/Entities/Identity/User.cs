namespace INuStore.Domain.Entities.Identity;

public class User : DomainEntity<int>
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime? CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? FirstLoggedInAtUtc { get; set; }

    public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

}
