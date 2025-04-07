# Stage 1: Build and publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EmployeeLeaveManagement.sln ./
COPY nuget.config ./
COPY UserService/UserService.csproj ./UserService/

RUN dotnet restore ./UserService/UserService.csproj

COPY UserService/. ./UserService/
WORKDIR /src/UserService
RUN dotnet publish -c Release -o /app/out

# ✅ Stage 2: Runtime with SDK for EF support
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# ✅ Install dotnet ef CLI
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "UserService.dll"]
