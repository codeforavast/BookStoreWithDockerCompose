# BookStore-With-DockerCompose

# Database setup
Run the script located at DBScripts/Script.sql on your SQL Server.

# Setup your connection string
On file located at src\BookStoreApi\appsettings.json, change database server IP connection string with your database params.

# JWT is implemented and required for POST, PUT, DELETE 
1. UserName : user
2. Password : 1234

# Request Validation
Request validation is doen using Fluent Validator

# Healthcheck
Healthcheck is implemented for database

# Swagger
Swagger is implemented for development env

# Steps
1. Clone the code.
2. Run script.sql to generate database and tables.
3. Build the code.
4. Run docker-compose up
5. you should be able to access api on http://localhost:32779/swagger/index.html or http://localhost:8082/swagger/index.html


# Thirdpary logging
nlog.production.config is logging into sql server

# Dev env logging
nlog.development.config is logging in file

# Application flow-diagram
BookStoreAPI_WorkFlow.pdf

# Author
## Kautilya Shukla


