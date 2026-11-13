using System.ComponentModel;

namespace INuStore.Domain.Constants;

public static class Permissions
{
    public static class User
    {
        [Description("View a user by ID")]
        public const string GetUserById = "User::GetUserById";

        [Description("List users")]
        public const string GetAllUsers = "User::GetAllUsers";

        [Description("Create a user")]
        public const string CreateUser = "User::CreateUser";

        [Description("Update a user")]
        public const string UpdateUser = "User::UpdateUser";

        [Description("Delete a user")]
        public const string DeleteUser = "User::DeleteUser";

        [Description("View a user's roles")]
        public const string GetUserRoles = "User::GetUserRoles";

        [Description("Replace a user's roles")]
        public const string ReplaceUserRoles = "User::ReplaceUserRoles";

        [Description("Assign a role to a user")]
        public const string AssignRoleToUser = "User::AssignRoleToUser";

        [Description("Remove a role from a user")]
        public const string RemoveRoleFromUser = "User::RemoveRoleFromUser";
    }

    public static class Role
    {
        [Description("List roles")]
        public const string GetAllRoles = "Role::GetAllRoles";

        [Description("View a role by ID")]
        public const string GetRoleById = "Role::GetRoleById";

        [Description("Create a role")]
        public const string CreateRole = "Role::CreateRole";

        [Description("Update a role")]
        public const string UpdateRole = "Role::UpdateRole";

        [Description("Delete a role")]
        public const string DeleteRole = "Role::DeleteRole";
    }

    public static class Permission
    {
        [Description("List permissions")]
        public const string GetAllPermissions = "Permission::GetAllPermissions";

        [Description("View a permission by ID")]
        public const string GetPermissionById = "Permission::GetPermissionById";

        [Description("Create a permission")]
        public const string CreatePermission = "Permission::CreatePermission";

        [Description("Update a permission")]
        public const string UpdatePermission = "Permission::UpdatePermission";

        [Description("Delete a permission")]
        public const string DeletePermission = "Permission::DeletePermission";
    }
}
