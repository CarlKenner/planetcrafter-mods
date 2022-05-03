﻿using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Doublestop.Tpc;
using Somethangs.Extensions.CommandLine;

return await new CommandLineBuilder(CommandUtil.CreateRootCommand<ProgramOpts>())
    .ConfigureDependencies()
    .AddCommandHandlers()
    .UseDefaults()
    .Build()
    .InvokeAsync(Environment.CommandLine);