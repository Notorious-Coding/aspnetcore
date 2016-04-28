using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Server;

namespace MusicStore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();

            var builder = new WebHostBuilder()
                .UseConfiguration(config)
                .UseIISIntegration()
                .UseStartup("MusicStore");

            if (string.Equals(builder.GetSetting("server"), "Microsoft.AspNetCore.Server.WebListener", System.StringComparison.Ordinal) &&
                (string.Equals(builder.GetSetting("environment") ??
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                "NtlmAuthentication",
                System.StringComparison.Ordinal)))
            {
                // Set up NTLM authentication for WebListener like below.
                // For IIS and IISExpress: Use inetmgr to setup NTLM authentication on the application vDir or
                // modify the applicationHost.config to enable NTLM.
                builder.UseWebListener(options =>
                {
                    // Set up NTLM authentication for WebListener as follows.
                    // For IIS and IISExpress use inetmgr to setup NTLM authentication on the application or
                    // modify the applicationHost.config to enable NTLM.
                    builder.UseWebListener(options =>
                    {
                        options.Listener.AuthenticationManager.AuthenticationSchemes = AuthenticationSchemes.NTLM;
                    });
                }
                else
                {
                    builder.UseWebListener();
                }
            }
            else
            {
                builder.UseKestrel();
            }

            var host = builder.Build();

            host.Run();
        }
    }
}
