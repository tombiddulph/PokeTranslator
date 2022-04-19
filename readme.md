# PokeTranslator

## Instructions


## Development dependencies

- .NET 6.0 SDK - [download](https://dotnet.microsoft.com/download)
- Docker Desktop - [download](https://www.docker.com/get-started)

## Building locally

[Cake](https://cakebuild.net/) is used to automate the build processes.
The build targets/steps can be found in `build.cake`, the tool doesn't need to be manually installed.

To build locally :

* Ensure .NET 6.x.x SDK is installed

1. Run `dotnet tool restore`  
2. Run `dotnet cake --target Build`

Note that step 1. only needs to be run once as the tool will be persisted, each subsequent build can be done by running `dotnet cake --target build`

## Running locally

[Docker Compose](https://docs.docker.com/compose/) can be used to run the project locally.  
The application will be hosted on http://localhost:5005/ note that the configuration will be pointing at the real Pokemon/Translation APIs which have usage limits

1. Run `docker-compose up --build` 

### Run pointing at mock server
1. 



### Docker Compose files

| File                                | Description                                                         | Local Urls                            |
| ----------------------------------- | ------------------------------------------------------------------- | ------------------------------------- |
| `docker-compose.yml`                | PokeTranslator API                                                  | http://localhost:5005/swagger         |
| `docker-compose-mocks.yml`          | Local server to mock Pokemon/Translation Apis                       | http://localhost:3000/                |
| `docker-compose-logging.yml`        | Seq: Logging tool                                                   | http://localhost:3001/                | 
</br>
