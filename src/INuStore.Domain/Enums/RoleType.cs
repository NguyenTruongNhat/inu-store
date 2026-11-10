using Ardalis.SmartEnum;

namespace INuStore.Domain.Enums;

public sealed class RoleType : SmartEnum<RoleType>
{
    public static readonly RoleType Admin =
        new(nameof(Admin), 1, "Admin for the Store");

    public static readonly RoleType Manager =
        new(nameof(Manager), 2, "Manager for the Store");

    public static readonly RoleType Staff =
        new(nameof(Staff), 3, "Staff for the Store");

    public static readonly RoleType Customer =
        new(nameof(Customer), 4, "Customer for the Store");

    public static readonly RoleType Shipper =
        new(nameof(Shipper), 5, "Shipper for the Store");

    public string Description { get; }

    private RoleType(string name, int value, string description)
        : base(name, value)
    {
        Description = description;
    }
}
