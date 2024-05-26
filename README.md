# Camapaign Management System
## Brief Description
Server side of an application for managing election campaigns.\
Contains an ASP.NET Core Web API project, working with a MS SQL Server hosted on Azure.\
Developed as part of my final project of my bachelor's degree at Bar-Ilan University, 2023.

## Features
The server is meant to be used by a client application, which is not included in this repository.\
Among the features provided by the server are:
- User authentication
- Campaign creation, editing and deletion
- Useful functions for managing campaigns, such as voter and ballot management, campaign member management, campaign event management, etc.
- Sending emails and SMS messages to voters

And more.

## Technologies used
- ASP.NET Core Web API
- Dapper (micro ORM)
- Azure SQL Database

## Installation
1. Clone the repository
2. Install the required NuGet packages
3. Create a new Azure SQL Database
4. Use the provided campaign_database.sql file to create the required tables and stored procedures
5. Update the connection string in the appsettings.json file
6. Run the server