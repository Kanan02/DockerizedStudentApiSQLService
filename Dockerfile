# Get Base Image (Full .NET Core SDK)
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY MyMicroservice.csproj .
RUN dotnet restore


# Copy everything else and build
COPY . .
RUN dotnet publish -c release -o /app

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
EXPOSE 80
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyMicroservice.dll"]
