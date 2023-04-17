# asp-net-core-identity-v2

To run in development using docker images make sure to have a dotnet certificate for ssl:
https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0

Running the below should do the trick:
```
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p devlopment
dotnet dev-certs https --trust
```
