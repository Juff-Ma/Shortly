using Serilog;
using Shortly.Setup;

Log.Logger = new LoggerConfiguration()
    .WithCommonConfiguration()
#if !DEBUG
    .MinimumLevel.Error()
#endif
    .WriteTo.Console(outputTemplate: CommonConstants.ConsoleLogTemplate)
    .CreateBootstrapLogger();

var logger = Log.ForContext<Program>();

try
{
    logger.Information("Building application");
    logger.Debug("Creating host");
    HostBuilder builder = new();

    logger.Debug("Applying configuration");
    logger.Verbose("Applying host configuration");
    builder.ApplyHostConfiguration();

    logger.Verbose("Applying App configuration");
    builder.ApplyShortlyConfiguration();

    logger.Debug("Configuring logging");
    builder.ApplyHostLogging();

    logger.Debug("Applying CLI extensions");
    builder.ApplyShortlyCliExtensions();

    logger.Debug("Configuring required services");
    builder.ConfigureServices(SetupServicesProvider.ConfigureServices);

    logger.Debug("Configuring web host");
    builder.ConfigureSlimWebHost(SetupWebHostProvider.ConfigureWebHost, 
        _ => { });

    logger.Information("Starting up application");
    await builder.BuildAndRunWithCli(args);
}
catch (Exception e)
{
    logger.Fatal(e, "An unrecoverable error occured during application startup.");
#if DEBUG
    throw;
#endif
}
finally
{
    logger.Verbose("App has shut down.");
    await Log.CloseAndFlushAsync();
}