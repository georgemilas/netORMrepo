This is a custom ORM framework I built in C# and .NET and used in various capacities in the past when Entity Framework or NHibernate were not as well known or if there were constraints that would render those tools not ideal, such as:
* Data models already exist or are built by data architects and are out of the control of application developers
* All changes to the database must be done through stored procedures
* All stored procedures must be checked in and code reviewed & validated by the database teams
* Tight control over transaction semantics exerted by the database teams 

It contains features like:
* a scaffolding system to auto-generate and render a basic CRUD app
* fully introspect a database to generate .NET classes, including tables, views, constraints etc.
* supports SQL Server, Oracle, and any ODBC-compliant database  
* code generation to create stored procedure artifacts to be included in source control
* a REST API layer for accessing the data
* security with JWT and OAuth
* a plugin system 

