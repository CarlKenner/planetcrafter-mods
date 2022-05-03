using System.CommandLine.Builder;
using Doublestop.Tpc.Config;
using Doublestop.Tpc.Handlers;
using Doublestop.Extensions.CommandLine;

namespace Doublestop.Tpc;

public static class ProgramExtensions
{
    #region Public Methods

    public static CommandLineBuilder AddCommandHandlers(this CommandLineBuilder builder)
    {
        return builder
            .AddHandler<AddPluginHandler>()
            .AddHandler<RemovePluginHandler>()
            .AddHandler<ListPluginsHandler>();
    }

    public static CommandLineBuilder ConfigureDependencies(this CommandLineBuilder builder)
    {
        return builder.AddMiddleware(context =>
        {
            // Configure a bunch of types shared by all the commands.
            var commandLineOpts = context.BindRootCommandResult<ProgramOpts>();
            var config = new ConfigBuilder()
                .AddDefaults()
                .AddConfigFile(commandLineOpts.ConfigFile)
                .AddValue(ConfigKeys.GameDir, commandLineOpts.GameDir)
                .Build();
            var game = new ThePlanetCrafter(string.IsNullOrWhiteSpace(config.GameDir)
                ? new Steam().GetGameDirectory()
                : config.GameDir);

            // Put em into the binding context's services, they'll get injected as needed into handler ctors.
            context.BindingContext.AddService(_ => config);
            context.BindingContext.AddService(_ => game);
            context.BindingContext.AddService(_ => game.BepInEx);
        });
    }

    #endregion
}