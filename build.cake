var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");
var dockerRunning = false;

const string integrationTestName = "poke-integration-test:latest";

//////////////////////////////////////////////////////////////////////
// addins
//////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.Docker&version=1.1.2

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
{
    CleanDirectory($"./src/PokeTranslator/bin/{configuration}");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetBuild("./PokeTranslator.sln", new DotNetBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetTest("./PokeTranslator.sln", new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
    });
});

Task("DockerBuild")
    .Does(() => 
    {
        var dockerSettings = new DockerImageBuildSettings
        {
            File = "./src/PokeTranslator/Dockerfile",
        };
        DockerBuild(dockerSettings, ".");
    });

Task("DockerBuildIntegrationTest")
    .Does(() =>
    {
        var dockerSettings = new DockerImageBuildSettings
        {
            // File = "./DockerFile",
            Tag = new [] { integrationTestName },
            Target = "integrationTest"
        };
        DockerBuild(dockerSettings, ".");
    });

Task("DockerRunIntegrationTest")
    .Does(() => 
    {
        var settings = new DockerContainerRunSettings
        {
            Network = "pokenetwork",
            Rm = true
        };

        DockerRunWithoutResult(settings, integrationTestName, null);
    });

Task("DockerComposeTest")
    .Does(() => 
    {
        dockerRunning = true;
        var dockerComposeSettings = new DockerComposeUpSettings 
        {
            Files = new[] {"./docker-compose-mocks.yml"},
            Build = true,
            DetachedMode = true
        };

        DockerComposeUp(dockerComposeSettings);
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Teardown(ctx =>
{
    if (dockerRunning)
    {
        Information("Stopping docker containers");
        DockerComposeDown(new DockerComposeDownSettings
        {
            Files = new[] {"./docker-compose-mocks.yml"},
            RemoveOrphans  = true
        });
    }
});

Task("RunIntegration")
    .IsDependentOn("DockerBuildIntegrationTest")
    .IsDependentOn("DockerComposeTest")
    .IsDependentOn("DockerRunIntegrationTest");

RunTarget(target);