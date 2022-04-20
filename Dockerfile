FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /app
COPY ./src ./src
RUN dotnet restore /app/src/PokeTranslator/PokeTranslator.csproj
RUN dotnet publish  /app/src/PokeTranslator/PokeTranslator.csproj -o /app/published-app

FROM build as unitTest
COPY ./test ./test
WORKDIR /app/test/PokeTranslator.Tests.Unit
RUN dotnet restore PokeTranslator.Tests.Unit.csproj
RUN dotnet publish PokeTranslator.Tests.Unit.csproj -o /app/unit-test-app
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM build as integrationTest
COPY ./test ./test
WORKDIR /app/test/PokeTranslator.Tests.Integration
RUN dotnet restore PokeTranslator.Tests.Integration.csproj
RUN dotnet publish PokeTranslator.Tests.Integration.csproj -o /app/test-app
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

