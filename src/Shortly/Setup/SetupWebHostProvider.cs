namespace Shortly.Setup;

public static class SetupWebHostProvider
{
    public static void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseKestrelCore();

        builder.ConfigureAppConfiguration((context, _) =>
        {
            builder.UseConfiguration(context.Configuration);
        });

        builder.Configure(ConfigureHosting);
    }

    private static void ConfigureHosting(WebHostBuilderContext context, IApplicationBuilder app)
    {
        app.UseRouting();

#if DEBUG
        if (context.HostingEnvironment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
        }
#endif
        app.UseEndpoints(SetupEndpointsProvider.ConfigureEndpoints);
    }
}
