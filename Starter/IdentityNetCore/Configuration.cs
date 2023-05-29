using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace IdentityNetCore;

public class Configuration
{
    readonly private string _identityDbContextKey = "IDENTITY_CONTEXT";
    readonly private string _toEmailAccountKey = "EMAIL_ACCOUNT";
    readonly private string _toEmailPasswordKey = "EMAIL_PASSWORD";
    readonly private string _toEmailHostKey = "EMAIL_HOST";
    readonly private string _toEmailPortKey = "EMAIL_PORT";

    public string? IdentityDbContextConnectionString { get; private set; }
    public string EmailAccount {get; private set;}
    public string EmailPassword {get; private set;}
    public string EmailHost {get; private set;}
    public int EmailPort {get; private set;}
    
    public Configuration(WebApplicationBuilder builder)
    {
        IdentityDbContextConnectionString = Environment.GetEnvironmentVariable(_identityDbContextKey);
        if(IdentityDbContextConnectionString == null)
        {
            IdentityDbContextConnectionString = builder.Configuration.GetConnectionString("IdentityContext");
        }
        EmailAccount = Environment.GetEnvironmentVariable(_toEmailAccountKey);
        EmailPassword = Environment.GetEnvironmentVariable(_toEmailPasswordKey);
        EmailHost = Environment.GetEnvironmentVariable(_toEmailHostKey);
        EmailPort = 587;
        if(int.TryParse(Environment.GetEnvironmentVariable(_toEmailPortKey), out int port)) {
            EmailPort = port;
        }
    }
}