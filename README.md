# EQC Portal

EQC Portal is an internal HR and employee quality control portal built with ASP.NET Core MVC, Entity Framework Core, SQL Server, Bootstrap, Chart.js, and Cloudinary. It provides a practical employee management workflow with dashboard analytics, employee profiles, attendance tracking, leave requests, performance reviews, responsive UI, and light/dark theme support.

This project was built as a portfolio-ready HR management system that demonstrates full-stack ASP.NET Core development, database modeling, CRUD workflows, file upload integration, dashboard reporting, and clean responsive UI implementation.

## Highlights

- Dashboard with KPI cards, quick actions, and Chart.js visual reports
- Employee management with CRUD, search, filters, pagination, soft delete, and profile photos
- Cloudinary integration for employee avatar storage
- Department and position management
- Leave request workflow with approval and rejection actions
- Attendance tracking with duplicate prevention and monthly summaries
- Performance review management with scoring and radar chart visualization
- Toast notifications, confirmation dialogs, and reusable empty states
- Responsive sidebar layout for desktop and mobile
- Light and dark theme support
- Vietnamese-friendly UI text and date/number formatting

## Tech Stack

| Area | Technology |
| --- | --- |
| Backend | ASP.NET Core 8 MVC |
| Database | SQL Server |
| ORM | Entity Framework Core 8 |
| Frontend | Razor Views, Bootstrap 5, CSS |
| Charts | Chart.js |
| Notifications | Toastr, SweetAlert2 |
| Icons | Font Awesome |
| Image Storage | Cloudinary |
| Tooling | .NET CLI, EF Core Migrations |

## Main Features

### Dashboard

The dashboard gives a quick overview of the HR system with employee, department, attendance, leave, and performance metrics. It includes visual charts for employees by department, gender distribution, attendance status, and leave request status.

### Employee Management

Employees can be created, updated, viewed, searched, filtered, paginated, and deleted using soft delete behavior. Each employee profile supports a Cloudinary-hosted avatar, contact details, job information, department, position, hire date, salary, and employment status.

### Department And Position Management

The system supports full CRUD management for departments and positions. These records are connected to employees and help organize the company structure.

### Leave Request Workflow

Employees can submit leave requests with start date, end date, reason, and leave type. Requests can be reviewed, approved, or rejected, with status reflected across list and detail pages.

### Attendance Tracking

Attendance records support employee check-in, check-out, status tracking, notes, and monthly reporting. The implementation includes duplicate attendance prevention for the same employee and date.

### Performance Reviews

Performance reviews track multiple score categories such as productivity, quality, communication, teamwork, and initiative. Review details include a radar chart to make performance results easier to compare visually.

## Project Structure

```text
eqcportal/
├── Controllers/              # MVC controllers for each module
├── Data/                     # EF Core DbContext
├── Models/                   # Entity models, settings, and view models
├── Services/                 # Application services such as Cloudinary upload
├── Views/                    # Razor views and shared partials
│   ├── Attendance/
│   ├── Department/
│   ├── Employee/
│   ├── Home/
│   ├── LeaveRequest/
│   ├── Performance/
│   ├── Position/
│   └── Shared/
├── wwwroot/
│   ├── css/
│   └── js/
├── Migrations/               # EF Core database migrations
└── Docs/                     # Planning, design, and improvement documents
```

## Database Overview

Main entities:

- `Employee`
- `Department`
- `Position`
- `LeaveRequest`
- `Attendance`
- `PerformanceReview`

Key relationships:

- A department has many employees.
- A position has many employees.
- An employee has many leave requests.
- An employee has many attendance records.
- An employee has many performance reviews.

## Routes

| Route | Description |
| --- | --- |
| `/` | Dashboard |
| `/Employee` | Employee list, search, filters, and actions |
| `/Employee/Create` | Create employee |
| `/Department` | Department management |
| `/Position` | Position management |
| `/LeaveRequest` | Leave request list and workflow |
| `/Attendance` | Attendance records |
| `/Attendance/MonthlySummary` | Monthly attendance report |
| `/Performance` | Performance review list |

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server or SQL Server Express
- EF Core CLI tools
- Cloudinary account for profile photo uploads

Install EF Core tools if needed:

```bash
dotnet tool install --global dotnet-ef
```

### 1. Clone The Repository

```bash
git clone https://github.com/NgKhai/eqcportal.git
cd eqcportal
```

### 2. Configure The Database

Update the connection string in `appsettings.json` if your SQL Server instance is different:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EQCPortalDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Apply database migrations:

```bash
dotnet ef database update
```

### 3. Configure Cloudinary

For local development, user secrets are recommended so API credentials do not need to be committed.

```bash
dotnet user-secrets init
dotnet user-secrets set "Cloudinary:CloudName" "your-cloud-name"
dotnet user-secrets set "Cloudinary:ApiKey" "your-api-key"
dotnet user-secrets set "Cloudinary:ApiSecret" "your-api-secret"
dotnet user-secrets set "Cloudinary:Folder" "eqcportal/avatars"
```

The project expects this configuration shape:

```json
{
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret",
    "Folder": "eqcportal/avatars"
  }
}
```

### 4. Run The Application

```bash
dotnet run
```

Open the application in the browser:

```text
http://localhost:5008
```

## Screenshots

Add screenshots here before publishing the repository on GitHub:

- Dashboard analytics
- Employee list and employee detail page
- Leave request workflow
- Attendance monthly summary
- Performance review radar chart

## What This Project Demonstrates

- Building a modular ASP.NET Core MVC application
- Designing EF Core entities, relationships, migrations, and queries
- Implementing practical CRUD workflows with validation
- Creating reusable Razor partials for layout, notifications, and empty states
- Integrating third-party cloud image storage with Cloudinary
- Building dashboard analytics with Chart.js
- Creating responsive admin-style UI with Bootstrap and custom CSS
- Improving user experience with toasts, confirmation dialogs, filtering, pagination, and theme support

## Status

Core portfolio features are implemented through the main HR modules: dashboard, employees, departments, positions, leave requests, attendance, performance reviews, Cloudinary avatar uploads, and responsive UI polish.

This project is suitable as a portfolio/CV project and can be extended into a more production-ready internal HR tool with authentication, authorization, automated testing, and deployment hardening.

## Author

Created by **NgKhai**.

- GitHub: `https://github.com/NgKhai`
- LinkedIn: `https://linkedin.com/in/ngkhai`