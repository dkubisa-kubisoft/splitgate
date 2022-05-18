using System;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Splitgate.Api.Startup))]

namespace Splitgate.Api
{
    public class Startup : FunctionsStartup 
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(new TableServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));
        }
    }
}