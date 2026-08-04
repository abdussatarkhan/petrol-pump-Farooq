# Migrations folder

This folder is intentionally empty in the delivered source. Generate the initial
migration yourself once you have the .NET 8 SDK and can restore NuGet packages
(this was built in a sandbox without SDK/NuGet access, so migrations couldn't be
generated here). From the solution root:

```
dotnet tool install --global dotnet-ef   # if you don't already have it
dotnet ef migrations add InitialCreate --project PetrolPumpMS.Data --startup-project PetrolPumpMS.App
dotnet ef database update --project PetrolPumpMS.Data --startup-project PetrolPumpMS.App
```

This reads `PetrolPumpMS.Data/DesignTimeDbContextFactory.cs` and every entity/config
in `PetrolPumpMS.Data/Configurations`, and generates a migration that creates all
ten tables with the exact columns, types, and `Restrict` delete behavior described
in the project brief. You do not need to write or hand-edit any migration code —
the App also calls `Database.Migrate()` automatically on first run (see
`App.xaml.cs`), so once this migration exists it applies itself.
