# ReactWithASP

[View Hosted App](https://reactwithaspserver20241228211727.azurewebsites.net/)

## Purpose of this project

A project to practice the development of Single Page Apps (SPA) with ASP.NET Core API, React, TypeScript and Azure.

 ## Stack
 - Front-End: React (TS)
 - Back-End: ASP.NET API (Controller-based)
 - DB: Azure Cosmos DB, Azure Blob Storage
 - Vite: Dev server, Build bundling, TS transpilation, API proxying during dev

The React front-end is built during the publish step, and its static files (HTML, JS, CSS) are automatically included in the ASP.NET Core project's wwwroot folder. Therefore the React app is served as static files from the ASP.NET Core application. API requests are handled by the ASP.NET Core controllers. Everything is deployed as a single unit to Azure App Service. The App Service runs the ASP.NET Core application, which handles both serving the frontend and processing API requests.

## Process
- Implemented simple Todo app with Cosmos DB
- Impemented simple Scan uploads with Blob Storage

## Want to do / practice
- Try more Azure features
- 3D viz of OBJs with r3f? investigate whether r3f is still the way to go for 3D in React with recent React changes
- Security & Authentication: review each item of [OWASP Checklist](https://owasp.org/www-project-web-security-testing-guide/assets/archive/OWASP_Web_Application_Penetration_Checklist_v1_1.pdf)
  - AppDOS
  -	AccessControl
  -	Authentication
  -	Authentication User
  -	Authentication Session Management
  -	Configuration Management
  -	Configuration Management Infrastructure
  -	Configuration Management Application
  -	Error Handling
  -	DataProtection
  -	DaaProtection Transport
  -	InputValidation
  -	InputValidation SQL

 ### Creation

 Created with the Visual Studio `React and ASP.NET Core (TS)` template. Project creation details based on [Tutorial: Create an ASP.NET Core app with React in Visual Studio](https://learn.microsoft.com/en-us/visualstudio/javascript/tutorial-asp-net-core-with-react?view=vs-2022).

 ### Notes

 [GDoc](https://docs.google.com/document/d/1aLu8pfwMafERf6gfbknh_8lAZH17xiVm5Q6fB9fNUbA/edit?usp=sharing)
