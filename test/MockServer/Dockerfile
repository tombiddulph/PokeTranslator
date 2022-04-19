FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /testserver
COPY . .
RUN dotnet restore
RUN dotnet publish -o /testserver/published-testserver

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /testserver
COPY --from=build /testserver/published-testserver /testserver

ENTRYPOINT ["dotnet", "MockServer.dll"]
