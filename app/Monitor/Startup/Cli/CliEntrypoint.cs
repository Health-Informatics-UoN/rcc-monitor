namespace Monitor.Startup.Cli;
using System.CommandLine;

public class CliEntrypoint : RootCommand
{
    public CliEntrypoint() : base("RedCap Monitor CLI")
    {
        AddGlobalOption(new Option<string>(new[] { "--environment", "-e" }));

        // Add Commands here
    }
}
