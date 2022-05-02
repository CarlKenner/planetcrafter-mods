using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Doublestop.Tpc;
using Doublestop.Tpc.Commands;
using Doublestop.Tpc.Config;
using Doublestop.Tpc.Handlers;
using Somethangs.Extensions.CommandLine;

return await new CommandLineBuilder(CommandUtil.CreateRootCommand<TpcOpts>())
    .UseDefaults()
    .AddHandler<InstallModHandler>()
    .AddMiddleware(context =>
    {
        var rootCommand = context.BindRootCommandResult<TpcOpts>();
        var config = new ConfigBuilder()
            .AddDefaults()
            .AddConfigFile(rootCommand.ConfigFile)
            .AddValue(ConfigKeys.GameDir, rootCommand.GameDir)
            .Build();
        var game = new PlanetCrafterGame(string.IsNullOrWhiteSpace(config.GameDir)
            ? new Steam().GetGameDirectory()
            : config.GameDir);

        context.BindingContext.AddDependency(config);
        context.BindingContext.AddDependency(game);
    })
    .Build()
    .InvokeAsync(Environment.CommandLine);