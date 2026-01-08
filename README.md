Just execute 

```
docker compose up --build
```
I had some issues with the Database resulting in various errors.

If some weird errors occur on startup, consinder running these commands on the host first:

```
dotnet restore
dotnet build
dotnet ef migrations add InitialCreate
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.0
dotnet ef migrations add InitialCreate

```
I'd love to have these in the Docker, but apparently it's not encouraged to do so. 
I'll be looking for a better solution. 

# Swagger

Access to Swagger: http://localhost:5251/swagger/index.html
