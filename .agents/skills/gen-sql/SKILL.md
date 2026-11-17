---
name: gen-sql
description: Generate EF Core SQL Server migration deployment scripts for INuStore. Use when asked to generate or regenerate a migration SQL file under deployment/sql.
---

# Generate INuStore migration SQL

Run from the INuStore repository root. Inspect the ordered migration IDs in `src/INuStore.Infrastructure/Migrations` and check `dotnet ef --version` against the project's EF Core packages.

- Use the requested target migration. If omitted, use the latest migration. Use the requested starting migration or the immediate predecessor of the target if omitted. Use `0` when generating from an empty database or when the target is the first migration.
- Use `deployment/sql/ddMMyyyy_MigrationName.sql`, with the current user-local date and the target migration name without its EF timestamp. Follow the naming convention in `deployment/sql/13112026_SeedPermissions.sql`.
- Generate an idempotent script by default, matching the migration history checks in the existing deployment scripts. If the output file exists, keep it unless the user requests regeneration or replacement. Use the user's requested filename or a free numeric suffix when a separate copy is requested.
- Run the command below with the resolved values. Pass `--no-build` only when the startup project has already been built with the selected migrations.

```powershell
dotnet ef migrations script $fromMigration $toMigration `
    --idempotent `
    --project src/INuStore.Infrastructure `
    --startup-project src/INuStore.API `
    --context ApplicationDbContext `
    --output $outputPath `
    -- --environment Development
```

Check the exit code, confirm the output file exists, and inspect the SQL and migration history entries for the selected range. Report the file path, migration range, and any warnings. A script starting after `0` assumes that the target database already has the starting migration. This workflow generates the file only. Applying it to a database requires a separate request.
