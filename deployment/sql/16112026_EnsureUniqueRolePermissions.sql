BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114094233_EnsureUniqueRolePermissions'
)
BEGIN
    WITH DuplicateAssignments AS (
        SELECT ROW_NUMBER() OVER (PARTITION BY RoleId, PermissionId ORDER BY Id) AS RowNumber
        FROM RolePermissions
    )
    DELETE FROM DuplicateAssignments WHERE RowNumber > 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114094233_EnsureUniqueRolePermissions'
)
BEGIN
    DROP INDEX [IX_RolePermissions_RoleId] ON [RolePermissions];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114094233_EnsureUniqueRolePermissions'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RolePermissions_RoleId_PermissionId] ON [RolePermissions] ([RoleId], [PermissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114094233_EnsureUniqueRolePermissions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261114094233_EnsureUniqueRolePermissions', N'10.0.14');
END;

COMMIT;
GO

