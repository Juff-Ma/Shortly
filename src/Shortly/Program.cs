using JasperFx;
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

    builder.ApplyHostConfiguration();
    builder.ApplyShortlyConfiguration();

    builder.ApplyHostLogging();

    builder.ApplyJasperFxExtensions();

    builder.ConfigureServices(SetupServicesProvider.ConfigureServices);
    builder.ConfigureSlimWebHost(SetupWebHostProvider.ConfigureWebHost, 
        _ => { });

    await builder.RunJasperFxCommands(args);
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