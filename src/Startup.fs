namespace FormulaTeamManager

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open StackExchange.Redis
open System
open Microsoft.AspNetCore.Cors.Infrastructure

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services.AddControllers() |> ignore

        let corsAction (options: CorsOptions) =
            let buildPolicy (builder: CorsPolicyBuilder) =
                builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin() |> ignore

            options.AddDefaultPolicy(new Action<CorsPolicyBuilder>(buildPolicy))

        services.AddCors(new Action<CorsOptions>(corsAction)) |> ignore

        let redisConnection = ConnectionMultiplexer.Connect("localhost")

        services.AddSingleton(redisConnection) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseCors() |> ignore

        app.UseRouting() |> ignore

        app.UseEndpoints(fun endpoints ->
            endpoints.MapControllers() |> ignore
            ) |> ignore

    member val Configuration: IConfiguration = null with get, set
