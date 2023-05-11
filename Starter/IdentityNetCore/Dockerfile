FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IdentityNetCore.csproj", "./"]
RUN dotnet restore "IdentityNetCore.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "IdentityNetCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityNetCore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./aspnet/https/dotnet-devcert.pfx /https/
ENTRYPOINT ["dotnet", "IdentityNetCore.dll"]