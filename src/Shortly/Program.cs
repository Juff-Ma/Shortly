using Serilog;

Log.Logger = new LoggerConfiguration()
    .WithCommonConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateSlimBuilder(args);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        throw new NotImplementedException("Development environment is not supported.");
    }

    await app.RunAsync();
}
catch (Exception e)
{
    Log.Fatal(e, "An unrecoverable error occured during application startup.");
#if DEBUG
    throw;
#endif
}
finally
{
    await Log.CloseAndFlushAsync();
}

static class CommonExtensions
{
    public static LoggerConfiguration WithCommonConfiguration(this LoggerConfiguration config)
    {
        return config
#if DEBUG
            .WriteTo.Debug()
#endif
            .Enrich.FromLogContext();
    }
}