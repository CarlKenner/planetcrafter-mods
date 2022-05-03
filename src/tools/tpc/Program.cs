using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Doublestop.Tpc;
using Doublestop.Extensions.CommandLine;

return await new CommandLineBuilder(CommandUtil.CreateRootCommand<ProgramOpts>())
    .ConfigureDependencies()
    .AddCommandHandlers()
    .UseDefaults()
    .Build()
    .InvokeAsync(Environment.CommandLine);