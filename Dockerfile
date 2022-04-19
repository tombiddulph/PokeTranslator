FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/published-app

# FROM build as unitTestRunner
# WORKDIR /app/test/PokeTranslator.Tests.Unit
# CMD ["dotnet", "test", "--logger:trx"]

FROM build as unitTest
WORKDIR /app/test/PokeTranslator.Tests.Unit
RUN dotnet test --logger:trx

FROM build as integrationTest
WORKDIR /app/test/PokeTranslator.Tests.Integration
#override localhost endpoint
ENV PokemonService__PokemonApi=http://172.17.0.1:3000/pokemon/
ENV PokemonService__Translations__Shakespeare=http://172.17.0.1:3000/translate/shakespeare.json
ENV PokemonService__Translations__Yoda=http://172.17.0.1:3000/translate/yoda.json
CMD ["dotnet", "test", "--logger:trx"]



FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/published-app /app

EXPOSE 5005
ENTRYPOINT ["dotnet", "PokeTranslator.dll"]
