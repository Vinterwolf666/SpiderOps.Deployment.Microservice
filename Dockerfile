#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Deployment.Microservice.API/Deployment.Microservice.API.csproj", "Deployment.Microservice.API/"]
COPY ["Customer.Deployment.Microservice/Deployment.Microservice.Domain.csproj", "Customer.Deployment.Microservice/"]
COPY ["Deployment.Microservice.APP/Deployment.Microservice.APP.csproj", "Deployment.Microservice.APP/"]
COPY ["Deployment.Microservice.Infrastructure/Deployment.Microservice.Infrastructure.csproj", "Deployment.Microservice.Infrastructure/"]
RUN dotnet restore "Deployment.Microservice.API/Deployment.Microservice.API.csproj"
COPY . .
WORKDIR "/src/Deployment.Microservice.API"
RUN dotnet build "Deployment.Microservice.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Deployment.Microservice.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Deployment.Microservice.API.dll"]