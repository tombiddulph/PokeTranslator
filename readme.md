# PokeTranslator


PokeTranslator is an API that exposes 2 endpoints:

* Endpoint 1: `/GET /pokemon/<pokemon name>`
    * Returns basic pokemon information (name, description, habitat & legendary status)
* Endpoint 2: `/GET /pokemon/translated/<pokemon name>`
    * Returns basic pokemon information (name, translated description, habitat & legendary status)
    * If the pokemons habitat is `cave` or the pokemon is `legendary` then the description is translated using a yoda translation otherwise the description is translated using shakespeare translation. If the translation fails then the standard description will be called.

Both endpoints return the following schemas for successful and unsuccessful calls.


The endpoint will return one of the following status codes

* 200 OK 
```
{
    "name": "string",
    "description": "string"
    "habitat": "string"
    "is_legendary": "boolean"
}
```



For any non success status code;

* 400 Bad request 
* 404 Not Found
* 5xx Server error

the body will be as follows

```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "descriptive title ",
    "status": status_code,
    "detail": "problem detail"
}
```

Example `/GET /pokemon/<pokemon name>` request and response.
```
// request
curl --location --request GET 'http://localhost:5005/pokemon/<pokemon name>'

//response
200 OK
{
    "name": "ivysaur",
    "description": "When the bulb on\nits back grows\nlarge, it appears\fto lose the\nability to stand\non its hind legs.",
    "habitat": "grassland",
    "is_legendary": false
}
```

Example `/GET /pokemon/translated/<pokemon name>` request and response.
```
// request
curl --location --request GET 'http://localhost:5005/pokemon/translated/<pokemon name>'

//response
200 OK
{
    "name": "ivysaur",
    "description": "When the bulb on\nits back grows\nlarge, it appears\fto lose the\nability to stand\non its hind legs.",
    "habitat": "grassland",
    "is_legendary": false
}
```


____

## Development dependencies

- .NET 6.0 SDK - [download](https://dotnet.microsoft.com/download)
- Docker Desktop - [download](https://www.docker.com/get-started)
___
## Building locally

[Cake](https://cakebuild.net/) is used to automate the build processes.
The build targets/steps can be found in `build.cake`, the tool doesn't need to be manually installed.

To build locally :

* Ensure .NET 6.x.x SDK is installed

1. Run `dotnet tool restore`  
2. Run `dotnet cake --target Build`

Note that step 1. only needs to be run once as the tool will be persisted, each subsequent build can be done by running `dotnet cake --target build`. If a new version of `cake.tool` is needed then run step 1 again. 
___
## Running locally

[Docker Compose](https://docs.docker.com/compose/) can be used to run the project locally.  
The application will be hosted on http://localhost:5005/ note that the configuration will be pointing at the real Pokemon/Translation APIs which have usage limits

1. Run `docker-compose up --build` 

### Run pointing at mock server
1. //todo

### Run With Seq
1. `docker compose -f docker-compose.yml -f docker-compose-logging.yml up -d` <br> The Seq ui will be available at `localhost:3001`, it can be used to view enriched application logs. 

### Run using dotnet cli
The project can also be run locally using the dotnet cli, to do so ensure that you are in the root directory, then run:
1. `dotnet run --project src/PokeTranslator/`

### Run using ide
The project can be run and debugged locally using an ide such as VisualStudio or Jetbrains Rider. Instructions on how to run in either of these ide's can be found easily on the internet. 

### Docker Compose files

| File                                | Description                                                         | Local Urls                            |
| ----------------------------------- | ------------------------------------------------------------------- | ------------------------------------- |
| `docker-compose.yml`                | PokeTranslator API                                                  | http://localhost:5005/swagger         |
| `docker-compose-mocks.yml`          | Local server to mock Pokemon/Translation Apis                       | http://localhost:3000/                |
| `docker-compose-logging.yml`        | Seq Logging tool                                                    | http://localhost:3001/                | 
</br>

___ 
### Testing
There are 2 TestProjects included with the project;


* [PokeTranslator.Tests.Unit](https://github.com/tombiddulph/PokeTranslator/tree/main/test/PokeTranslator.Tests.Unit) - Unit test project utilizing the [xUnit](https://xunit.net/)  testing framework.
 * [PokeTranslator.Tests.Integration](https://github.com/tombiddulph/PokeTranslator/tree/main/test/PokeTranslator.Tests.Integration) Integration test project using an in memory test server. [VerifyTests](https://github.com/VerifyTests/Verify) is used validate Http response messages and content.


The tests can be run in a number of ways
1. Unit tests
    * To run the unit tests using cake run `dotnet cake --target UnitTests`
    * To run the unit tests using the dotnet cli  `dotnet test test/PokeTranslator.Tests.Unit/`
    * The tests are also runnable via an ide test explorer
2. Integration Tests
    * The integration tests have a dependency on the mock server, cake automates the process of building and running the server.
    * To run the integration tests using cake run `dotnet cake --target IntegrationTests`, this will bring up and tear down all the dependencies for the test to run
    * To run the tests via the dotnet cli
        * You must first bring up the mock server by running `docker compose -f docker-compose-mocks.yml up -d`
        * Invoke the tests by running `dotnet test test/PokeTranslator.Tests.Integration`
        * Bring down the mock server by running `docker compose -f docker-compose-mocks.yml down`
    * To run the tests via an ide, bring up the mock server as above before running.


#### MockServer
A small mock server project is included in the solution to help with integration testing. The reason being that the translation api's allow for 5 request per hour.
______



### External dependencies

PokeTranslator is dependant on 3 external apis
    
1. https://pokeapi.co/api/v2/pokemon-species/ for information about pokemon
2. https://api.funtranslations.com/translate/shakespeare.json for  shakespeare translations.
4. https://api.funtranslations.com/translate/yoda.json for yoda translations.

