namespace INuStore.Domain.Entities.Identity;

public class Permission : DomainEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
