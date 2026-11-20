namespace Shortly.Setup;

public static class SetupWebHostProvider
{
    public static void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseKestrelCore()
            .ConfigureKestrel((context, options) =>
            {
                var kestrelConfig = context.Configuration
                    .GetSection("Host");

                options.Configure(kestrelConfig);
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
        app.UseShortlyEndpoints(context);
    }
}
