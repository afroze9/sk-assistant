using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddElsa(elsa =>
{
    elsa
        .UseWorkflowManagement(management => management.UseEntityFrameworkCore())
        .UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore())
        .UseIdentity(identity =>
        {
            identity.TokenOptions = options =>
                options.SigningKey = "random_secret_956a4d362as1d32as1d6as4d65a4ds6a5s4d56as4d6as46d4as6da";

            identity.UseAdminUserProvider();
        })
        .UseDefaultAuthentication(auth => auth.UseAdminApiKey())
        .UseWorkflowsApi()
        .UseRealTimeWorkflows()
        .UseCSharp()
        .UseHttp()
        .UseScheduling()
        .AddActivitiesFrom<Program>()
        .AddWorkflowsFrom<Program>();
});

builder.Services.AddCors(cors => cors
    .AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("x-elsa-workflow-instance-id")));

builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

app
    .UseCors()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseWorkflowsApi()
    .UseWorkflows()
    .UseWorkflowsSignalRHubs();

app.Run();