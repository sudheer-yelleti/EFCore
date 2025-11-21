# Use SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /EFCore

# Install EF Core CLI globally
RUN dotnet tool install --global dotnet-ef --version 8.0.0

# Ensure dotnet tools are on PATH
ENV PATH="$PATH:/root/.dotnet/tools"

COPY ["./EFCore.csproj", "EFCore/"]
RUN dotnet restore "EFCore/EFCore.csproj"

COPY . .
WORKDIR "/EFCore"
RUN dotnet build "EFCore.csproj" -c $BUILD_CONFIGURATION -o /EFCore/bin/build /p:UseAppHost=false

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EFCore.csproj" -c $BUILD_CONFIGURATION -o /EFCore/bin/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /EFCore/bin/publish .
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "EFCore.dll"]
