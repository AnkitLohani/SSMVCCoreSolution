# SSMVCCoreApp (.Net Core 2.2.402)

#### Packages

* Nuget Packages for the Project
  * For Azure Storage
    1. Install-Package WindowsAzure.Storage -Version 9.3.3 or Latest
  * For Azure Application Insights
      1. Install-Package Microsoft.ApplicationInsights.AspNetCore -Version 2.7.1 or (latest)
      2. Install-Package Microsoft.Extensions.Logging.ApplicationInsights -Version 2.10.0 (for logging ILogger user defined logs in ApplicationInsights)

#### Bower  

* Bower Packages for the Project
  1. jquery - 3.4.1
  2. bootstrap - 4.2.1
  3. font-awesome - 4.7.0
  4. jquery-validation - 1.19.1
  5. jquery-validation-unobtrusive - 3.2.11


#### Entity Framework Core Commands
    
1. Create the Entities with the Annotations, DbContext class
2. Add ConnectionStrings to the appsettings.json file
3. Add the service.AddDbContext and pass the connectionString to it and also set the Resilient Entity Framework Core SQL connections (Similar to SqlAzureExecutionStrategy in MVC5)
4. In the CMD  (dotnet tool install --global dotnet-ef this will install the dotnet-ef cli)
  4.1 dotnet ef database update (This will create the database)  
  4.2 dotnet ef migrations add InitialDB -o DataMigrations\Migrations (This will create the class with the table schema)  
  4.3 dotnet ef migrations remove (This will remove the migrations)  
  4.4 dotnet ef database update (This will create the database, if the database exits then it will update the database with the latest changes)  
  4.5 dotnet ef database drop (This will drop the database)

```sql
CREATE TABLE [dbo].[Products] (
    [ProductId]   INT             IDENTITY (1, 1) NOT NULL,
    [ProductName] NVARCHAR (100)  NOT NULL,
    [Description] NVARCHAR (250)  NOT NULL,
    [Price]       DECIMAL (18, 2) NOT NULL,
    [Category]    NVARCHAR (100)  NOT NULL,
    [PhotoUrl]    NVARCHAR (MAX)  NULL
);

Select * from Products
```