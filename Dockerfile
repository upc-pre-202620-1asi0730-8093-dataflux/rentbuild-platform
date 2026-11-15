FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY RentBuild.Platform/RentBuild.Platform.csproj RentBuild.Platform/
COPY Directory.Build.props ./
RUN dotnet restore RentBuild.Platform/RentBuild.Platform.csproj
COPY RentBuild.Platform/ RentBuild.Platform/
RUN dotnet publish RentBuild.Platform/RentBuild.Platform.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "RentBuild.Platform.dll"]
