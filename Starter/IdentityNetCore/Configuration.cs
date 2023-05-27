using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace IdentityNetCore;

public class Configuration
{
    readonly private string _identityDbContextKey = "IDENTITY_CONTEXT";

    public string? IdentityDbContextConnectionString { get; private set; }
    
    public Configuration(WebApplicationBuilder builder)
    {
        IdentityDbContextConnectionString = Environment.GetEnvironmentVariable(_identityDbContextKey);
        if(IdentityDbContextConnectionString == null)
        {
            IdentityDbContextConnectionString = builder.Configuration.GetConnectionString("IdentityContext");
        }
    }
}