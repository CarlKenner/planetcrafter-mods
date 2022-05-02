using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Somethangs.Extensions.CommandLine;
using Thangs.Tpc;
using Thangs.Tpc.Commands;
using Thangs.Tpc.Config;
using Thangs.Tpc.Handlers;

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