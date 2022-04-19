using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace PokeTranslator.Tests.Integration;

// ReSharper disable once ClassNeverInstantiated.Global
public class PokeTranslator : WebApplicationFactory<Program>
{
    protected override TestServer CreateServer(IWebHostBuilder builder)
    {   
        /*
         * This override is needed for running in docker, the environment variables don't seem to be applied
         * in IHost CreateHost(IHostBuilder builder)
         */
        builder
            .UseConfiguration(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build())
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((hostContext, config) =>
                config.AddJsonFile("appsettings.test.json", optional: false).AddEnvironmentVariables());


        return base.CreateServer(builder);
    }
    

    protected override IHost CreateHost(IHostBuilder builder)
    {
        /*
         * This override is needed when running locally
         */
        builder
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseEnvironment("test")
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.Sources.Clear();
                config.AddJsonFile("appsettings.test.json", optional: false);
                config.AddEnvironmentVariables();
            });
    
        
        return base.CreateHost(builder);
    }
}