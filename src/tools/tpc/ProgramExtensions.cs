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
            // Configure a bunch of shared types (not gonna say services)
            var commandLineOpts = context.BindRootCommandResult<ProgramOpts>();
            var config = new ConfigBuilder()
                .AddDefaults()
                .AddConfigFile(commandLineOpts.ConfigFile)
                .AddValue(ConfigKeys.GameDir, commandLineOpts.GameDir)
                .Build();

            var game = new ThePlanetCrafter(config.GameDir);

            // Put em into the binding context's services, they'll get injected as needed into handler ctors.
            context.BindingContext.AddSingleton(config);
            context.BindingContext.AddSingleton(game);
            context.BindingContext.AddSingleton(game.Plugins);
        });
    }

    #endregion
}