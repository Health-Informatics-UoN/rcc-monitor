using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Monitor.Commands.Helpers;
using Monitor.Startup.Cli;
using Monitor.Startup.EfCoreMigrations;
using Monitor.Startup.Web;

// Enable EF Core tooling to get a DbContext configuration
EfCoreMigrationsEntrypoint.BootstrapDbContext(args);

// Global App Startup stuff here

// Initialise the command line parser and run the appropriate entrypoint
await new CommandLineBuilder(new CliEntrypoint())
    .UseDefaults()
    .UseRootCommandBypass(args, WebEntrypoint.Run)
    .UseCliHostDefaults(args)
    .Build()
    .InvokeAsync(args);
