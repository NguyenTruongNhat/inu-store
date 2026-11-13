BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112082137_SeedPermissions'
)
BEGIN
    INSERT INTO [dbo].[Permissions] ([Name], [Description])
    SELECT [seed].[Name], [seed].[Description]
    FROM (VALUES
        (N'User::GetUserById', N'View a user by ID'),
        (N'User::GetAllUsers', N'List users'),
        (N'User::CreateUser', N'Create a user'),
        (N'User::UpdateUser', N'Update a user'),
        (N'User::DeleteUser', N'Delete a user'),
        (N'User::GetUserRoles', N'View a user''s roles'),
        (N'User::ReplaceUserRoles', N'Replace a user''s roles'),
        (N'User::AssignRoleToUser', N'Assign a role to a user'),
        (N'User::RemoveRoleFromUser', N'Remove a role from a user'),
        (N'Role::GetAllRoles', N'List roles'),
        (N'Role::GetRoleById', N'View a role by ID'),
        (N'Role::CreateRole', N'Create a role'),
        (N'Role::UpdateRole', N'Update a role'),
        (N'Role::DeleteRole', N'Delete a role'),
        (N'Permission::GetAllPermissions', N'List permissions'),
        (N'Permission::GetPermissionById', N'View a permission by ID'),
        (N'Permission::CreatePermission', N'Create a permission'),
        (N'Permission::UpdatePermission', N'Update a permission'),
        (N'Permission::DeletePermission', N'Delete a permission')
    ) AS [seed] ([Name], [Description])
    WHERE NOT EXISTS (
        SELECT 1
        FROM [dbo].[Permissions] AS [existing]
        WHERE [existing].[Name] = [seed].[Name]
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112082137_SeedPermissions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261112082137_SeedPermissions', N'10.0.14');
END;

COMMIT;
GO

