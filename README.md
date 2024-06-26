# Camapaign Management System
## Brief Description
Server side of an application for managing election campaigns.\
Contains an ASP.NET Core Web API project, working with a MS SQL Server hosted on Azure.\
Developed as part of a final project of for a bachelor's degree at Bar-Ilan University, 2023.\
Meant to work in tandem with the web client, which can be found at https://github.com/YuvalUner/Campaign-Management-System-Frontend 

## Features
The server is meant to be used by a client application, which is not included in this repository.\
Among the features provided by the server are:
- User authentication
- Campaign creation, editing and deletion
- Useful functions for managing campaigns, such as voter and ballot management, campaign member management (adding and removing, roles, permissions), campaign event management jobs management, etc.
- Sending emails and SMS messages to voters
- Getting campaign advice via the AI assistant, based on analysis of online activity on social media platforms.

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
5. Replace the example connection string in the API/appsettings.json.
6. Replace all of the (revoked) credentials in the same appsettings.json file. Specifically, replace the Google credentials, Gmail account for email sending, OpenAIApiKey, NewsApiKey and Telesign. 
7. Run the server
