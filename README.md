# Petrol Pump Management System (C# / WPF / PostgreSQL)

A rebuild of the original Python (Tkinter + SQLite) Petrol Pump Management
System as a .NET 8 WPF desktop application with a PostgreSQL backend, EF Core
Code-First migrations, MVVM (CommunityToolkit.Mvvm), dependency injection, and
a redesigned, higher-contrast UI.

## Solution layout

```
PetrolPumpMS.sln
├── PetrolPumpMS.Models/     Plain entity classes (User, FuelType, Tank, Nozzle, Employee, Customer, Purchase, Sale, Expense, CreditPayment)
├── PetrolPumpMS.Data/       AppDbContext, EF Core entity configurations, DbSeeder, design-time factory, migrations
├── PetrolPumpMS.Services/   Business logic (auth, sales, fuel/tank/nozzle, employees, customers, expenses, reports, backup) — no UI references, unit-testable
└── PetrolPumpMS.App/        WPF app: Views (XAML), ViewModels (MVVM), Resources (theme/styles), Converters, appsettings.json
```

## Prerequisites

- **.NET 8 SDK** (Windows — WPF requires the Windows desktop runtime): https://dotnet.microsoft.com/download/dotnet/8.0
- **PostgreSQL** installed and running locally (or reachable over the network), e.g. via the official installer or `winget install PostgreSQL.PostgreSQL`.
- Optional: `pg_dump` on your `PATH` (ships with the PostgreSQL installer) if you want to use the **Backup Database** button in Settings.

> **Note on how this was built:** this solution was generated in a Linux sandbox without a Windows/.NET runtime or access to `nuget.org`, so it has **not been compiled here**. Every file was written by hand against EF Core 8 / WPF / CommunityToolkit.Mvvm APIs I'm confident in, and reviewed layer by layer (Models → Data → Services → ViewModels → Views), but please build it once locally before relying on it, and let me know if anything doesn't compile so I can fix it.

## 1. Configure the connection string

Edit `PetrolPumpMS.App/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=petrol_pump_db;Username=postgres;Password=postgres"
  }
}
```

Create the database itself first, e.g.:

```sh
createdb -U postgres petrol_pump_db
```

## 2. Generate and apply the EF Core migration

Migrations weren't generated in this sandbox (no SDK/NuGet access — see note above),
but the app is fully wired for Code-First migrations. From the solution root:

```sh
dotnet tool install --global dotnet-ef      # once, if you don't already have it
dotnet restore
dotnet ef migrations add InitialCreate --project PetrolPumpMS.Data --startup-project PetrolPumpMS.App
dotnet ef database update --project PetrolPumpMS.Data --startup-project PetrolPumpMS.App
```

This reads every entity configuration in `PetrolPumpMS.Data/Configurations` and
creates all ten tables (`users`, `fuel_types`, `tanks`, `nozzles`, `employees`,
`customers`, `purchases`, `sales`, `expenses`, `credit_payments`) with the
`Restrict` delete behavior specified in the brief (you can't delete a tank,
nozzle, or customer that has linked sales/purchase records).

You only need to do this once. After that, `PetrolPumpMS.App` calls
`Database.Migrate()` automatically on every startup, so any future migrations
you add will apply themselves.

## 3. Run the app

```sh
dotnet run --project PetrolPumpMS.App
```

or open `PetrolPumpMS.sln` in Visual Studio / Rider and run `PetrolPumpMS.App`.

On first run, the app seeds:
- **Default admin login:** `admin` / `Admin@123` — please change this immediately from **Settings → Change Password** after your first sign-in.
- **Default fuel types:** Petrol (Rs. 106.50/L) and Diesel (Rs. 92.75/L) — edit prices from **Fuel & Tanks → Fuel Types**.

## What's implemented

- **Authentication** — login screen, BCrypt password hashing, three roles (Admin/Manager/Cashier).
- **Dashboard** — today's sales total, transaction count, today's expenses, total credit outstanding, tank stock levels with a "LOW STOCK" badge, recent sales.
- **Sales Entry** — select an active nozzle → its last reading becomes the opening reading → enter closing reading → quantity/amount auto-calculate from the fuel's current price → select attendant → select payment mode (Credit requires a customer) → saving atomically updates the nozzle's last reading, deducts tank stock, and (for Credit) increases the customer's balance.
- **Fuel & Tank Management** — CRUD for fuel types, tanks, nozzles (tabbed), plus a "Record Fuel Purchase" flow that adds stock to a tank and blocks over-capacity deliveries.
- **Employees** — CRUD with active/inactive status; deleting an employee with sales history is blocked (set them Inactive instead).
- **Customers / Credit Accounts** — CRUD, an "OVER LIMIT" badge, and a "Record Payment" flow that reduces the balance and logs the payment.
- **Expenses** — log and delete by date/category/description/amount.
- **Reports** — date-range filterable sales-by-fuel-type, sales-by-payment-mode, employee performance, and a daily sales trend line chart (LiveCharts2).
- **Settings** — change your own password; Admins can create/deactivate users and assign roles; "Backup Database" shells out to `pg_dump`.

## Design notes

- **Business logic lives in `PetrolPumpMS.Services`**, not in code-behind or ViewModels — e.g. `SalesService.RecordSaleAsync` and `FuelService.RecordPurchaseAsync` wrap their stock/balance updates in an EF Core transaction. These are plain classes taking an `AppDbContext`, so they're straightforward to unit test against an in-memory or SQLite EF Core provider.
- **DI + DbContext lifetime:** `AppDbContext` and the services are registered `Scoped`. Because ViewModels are long-lived (Transient/Singleton) in a desktop app rather than per-request like a web app, `ViewModelBase` takes an `IServiceScopeFactory` and opens a **fresh DI scope for every command** (`RunAsync`/`Resolve<T>`), so every operation gets its own short-lived `AppDbContext` instance. This avoids the classic WPF+EF Core pitfall of one long-lived `DbContext` being used across threads/operations.
- **UI theme:** light theme, single accent blue (`#2563EB`), semantic success/warning/danger colors used consistently for badges (LOW STOCK, OVER LIMIT), card-based layout with soft shadows, zebra-striped/hover-highlighted DataGrids, non-blocking toast notifications (bottom-right) instead of `MessageBox` popups, and inline validation text under forms instead of dialog boxes. Sidebar navigation uses text labels; swap in an icon library (Material Design Icons / FontAwesome, both available via `MahApps.Metro.IconPacks`, already referenced in the `.csproj`) by adding `<iconPacks:PackIconMaterial Kind="..."/>` next to each `RadioButton`'s content if you'd like icons too.

## Known follow-ups

- The `InitialCreate` migration needs to be generated locally (step 2 above) — I couldn't run `dotnet ef` in this environment.
- `pg_dump`-based backup assumes the tool is on `PATH`; there's no restore UI (restore via `pg_restore` manually).
- No automated test project is included yet; the service layer's constructor-injected `AppDbContext` makes it easy to add one (e.g. `PetrolPumpMS.Services.Tests` using the EF Core InMemory or SQLite provider) if you'd like — happy to add it next.
