# Food Shelter Management Application

A comprehensive C# ASP.NET Core application for managing food shelter resources, inventory, and volunteer coordination.

## Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 (recommended) or Visual Studio Code
- SQL Server (LocalDB is configured by default)

## Getting Started

1. Clone the repository
2. Open the solution file `FoodShelterAppC#.sln` in Visual Studio

### Database Setup

The application uses Entity Framework Core with SQL Server. The connection string is configured in:
csharp:FoodShelterAppC#/appsettings.json
startLine: 8
endLine: 10


LocalDB is used by default. No additional configuration is needed if you have SQL Server LocalDB installed.

### Running the Application

In Visual Studio:
- Right-click on the project in Solution Explorer
- Select "Set as Startup Project"
- Run ```Update-Database``` in the package manager console
- Press F5 or click the "Run" button

The application will start and be available at:
- HTTPS: https://localhost:7140

### Email Configuration

The application uses SMTP for email notifications. If you want, you can update the email settings in appsettings.json(a burner email it being used for testing):

json:FoodShelterAppC#/appsettings.json
startLine: 11
endLine: 17


Replace the credentials with your own SMTP server details if you want to use another email.

## Features

- Food inventory management
- Volunteer management
- Donation tracking
- Shelter location mapping
- Meal planning
- User authentication and authorization
- Email notifications
- Interactive dashboard

## License

This project is licensed under the MIT License - see the LICENSE.txt file for details.
