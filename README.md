# WorkTracker

A simple Blazor Server application for logging your daily work. Record what you
worked on, when, and for how long — then review a clean per-day log showing each
task and the total time you spent each day.

## Features

- **Add work items** with a title, description, and start/end times, validated through a Bootstrap-styled form.
- **Daily log view** that groups entries by day and shows the total time spent per day.
- **Per-entry duration** calculated automatically from start and end times.
- Data persisted to a **PostgreSQL** database using **Dapper**.

## Tech Stack

- [.NET 9.0](https://dotnet.microsoft.com/) (Blazor Server / interactive server components) — *adjust to your version*
- [Dapper](https://github.com/DapperLib/Dapper) for data access
- [PostgreSQL](https://www.postgresql.org/) (hosted free on [Neon](https://neon.tech/))
- [Npgsql](https://www.npgsql.org/) PostgreSQL driver
- [Bootstrap](https://getbootstrap.com/) + [Bootstrap Icons](https://icons.getbootstrap.com/) for styling

## Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download) or later
- A PostgreSQL database — the free tier at [Neon](https://neon.tech/) works well

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/raju-melveetilpurayil/work-tracker.git
cd WorkTracker
```

### 2. Create the database table

Create a database (e.g. on Neon) and run this once in the SQL editor:

```sql
CREATE TABLE work_items (
    id          BIGSERIAL PRIMARY KEY,
    start_time  TIMESTAMP      NOT NULL,
    end_time    TIMESTAMP      NOT NULL,
    title       VARCHAR(120)   NOT NULL,
    description VARCHAR(1000),
    date_time   TIMESTAMP      NOT NULL,   -- when the entry was recorded
    user_id     VARCHAR(450)   NOT NULL
);

CREATE INDEX ix_work_items_user_day ON work_items (user_id, start_time);
```

### 3. Configure the connection string

The connection string is **not** committed to source control. Store it locally
with [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets):

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=YOUR_HOST;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
```

For production, set it as an environment variable instead:

```
ConnectionStrings__DefaultConnection=Host=...;Database=...;Username=...;Password=...
```

### 4. Set the username

The current user is read from `appsettings.json`:

```json
{
  "AppUser": {
    "UserId": "your-username"
  }
}
```

### 5. Run the app

```bash
dotnet run
```

Then open the URL shown in the console (typically `https://localhost:5001`).

## Configuration

| Setting | Location | Description |
| --- | --- | --- |
| `ConnectionStrings:DefaultConnection` | User Secrets / environment variable | PostgreSQL connection string (kept out of git) |
| `AppSettings:UserId` | `appsettings.json` | Identifier for the current user |

## Project Structure

```
WorkTracker/
├── Components/
│   ├── AddWorkItem.razor        # Form for adding work items
│   └── WorkLog.razor            # Per-day log view
├── Data/
│   └── NpgsqlConnectionFactory.cs   # Creates PostgreSQL connections
├── Services/
│   └── WorkLogService.cs        # Add + query work items via Dapper
├── Models/
│   ├── WorkItem.cs              # Work item entity
│   └── WorkItemFormModel.cs     # Form model with validation
├── appsettings.json
└── Program.cs
```

## Usage

1. Go to the **Add Work Item** page and fill in a title, description, and start/end times.
2. Save the entry — it's written to the database.
3. Open the **Work Log** page to see your entries grouped by day, each with its duration and a daily total.

## Security Notes

- The database connection string is kept out of source control via User Secrets / environment variables.
- If a connection string is ever committed, **rotate the database password** — it remains in git history.

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
*(Update this section to match the license you choose.)*
