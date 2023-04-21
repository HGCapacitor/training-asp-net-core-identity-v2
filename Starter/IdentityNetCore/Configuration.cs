using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace IdentityNetCore;

public class Configuration
{
    readonly private string _identityContextKey = "IDENTITY_CONTEXT";

    public string? FacadeContextConnectionString { get; private set; }
    
    public Configuration(WebApplicationBuilder builder)
    {
        FacadeContextConnectionString = Environment.GetEnvironmentVariable(_identityContextKey);
        if(FacadeContextConnectionString == null)
        {
            FacadeContextConnectionString = builder.Configuration.GetConnectionString("IdentityContext");
        }
    }
}