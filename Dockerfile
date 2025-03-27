# Use the .NET 9.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["MoRent V2.csproj", "."]
RUN dotnet restore "MoRent V2.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy from build stage
COPY --from=build /app .

# Set environment variables (adjust as needed)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5120
EXPOSE 5120

# Entry point
ENTRYPOINT ["dotnet", "MoRent V2.dll"]